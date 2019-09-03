using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

// パーティクルデータの構造体
public struct ParticleData
{
    public Vector3 Velocity; // 速度
    public Vector3 Position; // 位置
    public Vector3 Acceleration;
    public float LifeTime;
    public float TimeFromRePositioning;
};

public class ParticleBrush : MonoBehaviour
{
    const int NUM_PARTICLES = 32768; // 生成するパーティクルの数

    const int NUM_THREAD_X = 8; // スレッドグループのX成分のスレッド数
    const int NUM_THREAD_Y = 1; // スレッドグループのY成分のスレッド数
    const int NUM_THREAD_Z = 1; // スレッドグループのZ成分のスレッド数
    const int NUM_LINE_POSITIONS = 4000;

    public ComputeShader SimpleParticleComputeShader; // パーティクルの動きを計算するコンピュートシェーダ
    public Shader SimpleParticleRenderShader;  // パーティクルをレンダリングするシェーダ

    public Texture2D ParticleTex;          // パーティクルのテクスチャ
    public float ParticleSize = 0.05f; // パーティクルのサイズ

    public Camera RenderCam; // パーティクルをレンダリングするカメラ（ビルボードのための逆ビュー行列計算に使用）
    public float lifeTime = 1.0f;

    ComputeBuffer particleBuffer;     // パーティクルのデータを格納するコンピュートバッファ 
    Material particleRenderMat;  // パーティクルをレンダリングするマテリアル
    Queue<Vector4> linePositions;

    void Start()
    {
        // パーティクルのコンピュートバッファを作成
        particleBuffer = new ComputeBuffer(NUM_PARTICLES, Marshal.SizeOf(typeof(ParticleData)));
        // パーティクルの初期値を設定
        var pData = new ParticleData[NUM_PARTICLES];
        for (int i = 0; i < pData.Length; i++)
        {
            pData[i].Velocity = Random.insideUnitSphere;
            pData[i].Position = Random.insideUnitSphere;
            pData[i].Acceleration = Random.insideUnitSphere;
            pData[i].LifeTime = lifeTime + Random.Range(-lifeTime * 0.5f, lifeTime * 0.5f);
            pData[i].TimeFromRePositioning = 0;
        }
        // コンピュートバッファに初期値データをセット
        particleBuffer.SetData(pData);

        pData = null;

        // パーティクルをレンダリングするマテリアルを作成
        particleRenderMat = new Material(SimpleParticleRenderShader);
        particleRenderMat.hideFlags = HideFlags.HideAndDontSave;

        linePositions = new Queue<Vector4>();
    }


    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            // Vector3でマウス位置座標を取得する
            var position = Input.mousePosition;
            // Z軸修正
            position.z = 10f;
            // マウス位置座標をスクリーン座標からワールド座標に変換する
            var pos = Camera.main.ScreenToWorldPoint(position);
            if (linePositions.Count >= NUM_LINE_POSITIONS)
            {
                linePositions.Dequeue();
            }
            linePositions.Enqueue(pos);
        }
    }


    void OnRenderObject()
    {
        ComputeShader cs = SimpleParticleComputeShader;
        int kernelId = cs.FindKernel("CSMain");
        if (linePositions.Count <= 0)
        {
            return;
        }

        // スレッドグループ数を計算
        int numThreadGroup = NUM_PARTICLES / NUM_THREAD_X;
        // 各パラメータをセット
        cs.SetFloat("_TimeStep", Time.deltaTime);
        cs.SetInt("_LinePositionsNum", linePositions.Count);
        cs.SetVectorArray("_LinePostions", linePositions.ToArray());
        // コンピュートバッファをセット
        cs.SetBuffer(kernelId, "_ParticleBuffer", particleBuffer);
        // コンピュートシェーダを実行
        cs.Dispatch(kernelId, numThreadGroup, 1, 1);


        // 逆ビュー行列を計算
        var inverseViewMatrix = RenderCam.worldToCameraMatrix.inverse;

        Material m = particleRenderMat;
        m.SetPass(0); // レンダリングのためのシェーダパスをセット
                      // 各パラメータをセット
        m.SetMatrix("_InvViewMatrix", inverseViewMatrix);
        m.SetTexture("_MainTex", ParticleTex);
        m.SetFloat("_ParticleSize", ParticleSize);
        // コンピュートバッファをセット
        m.SetBuffer("_ParticleBuffer", particleBuffer);
        // パーティクルをレンダリング
        Graphics.DrawProceduralNow(MeshTopology.Points, NUM_PARTICLES);
    }

    void OnDestroy()
    {
        if (particleBuffer != null)
        {
            // バッファをリリース（忘れずに！）
            particleBuffer.Release();
        }

        if (particleRenderMat != null)
        {
            // レンダリングのためのマテリアルを削除
            DestroyImmediate(particleRenderMat);
        }
    }
}

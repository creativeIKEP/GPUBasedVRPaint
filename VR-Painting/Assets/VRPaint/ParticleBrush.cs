using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TrailBase;

// パーティクルデータの構造体
public struct ParticleData
{
    public Vector3 Velocity; // 速度
    public Vector3 Position; // 位置
    public Vector3 Acceleration;
    public float LifeTime;
    public float TimeFromRePositioning;
    public int generatedNodeId;
};

public class ParticleBrush : MonoBehaviour
{
    const int NUM_PARTICLES = 32768; // 生成するパーティクルの数

    const int NUM_THREAD_X = 8; // スレッドグループのX成分のスレッド数
    const int NUM_THREAD_Y = 1; // スレッドグループのY成分のスレッド数
    const int NUM_THREAD_Z = 1; // スレッドグループのZ成分のスレッド数

    public ComputeShader SimpleParticleComputeShader; // パーティクルの動きを計算するコンピュートシェーダ
    public Shader SimpleParticleRenderShader;  // パーティクルをレンダリングするシェーダ

    public Texture2D ParticleTex;          // パーティクルのテクスチャ
    public float ParticleSize = 0.05f; // パーティクルのサイズ

    public Camera RenderCam; // パーティクルをレンダリングするカメラ（ビルボードのための逆ビュー行列計算に使用）
    public float lifeTime = 1.0f;
    public Color particleColor;
    public float partticleSpeed = 10.0f;
    public float lineThickness = 1.0f;
    public TrailBrush trailBrush;

    ComputeBuffer particleBuffer;     // パーティクルのデータを格納するコンピュートバッファ 
    Material particleRenderMat;  // パーティクルをレンダリングするマテリアル


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
            pData[i].generatedNodeId = i * trailBrush.nodeNum / NUM_PARTICLES;
        }
        // コンピュートバッファに初期値データをセット
        particleBuffer.SetData(pData);

        pData = null;

        // パーティクルをレンダリングするマテリアルを作成
        particleRenderMat = new Material(SimpleParticleRenderShader);
        particleRenderMat.hideFlags = HideFlags.HideAndDontSave;
    }


    void OnRenderObject()
    {
        ComputeShader cs = SimpleParticleComputeShader;
        int kernelId = cs.FindKernel("CSMain");

        // スレッドグループ数を計算
        //int numThreadGroup = NUM_PARTICLES / trailBrush.nodeNum;
        // 各パラメータをセット
        cs.SetInt("_particleNum", NUM_PARTICLES);
        cs.SetFloat("_TimeStep", Time.deltaTime);
        cs.SetFloat("_Speed", partticleSpeed);
        cs.SetFloat("_Thickness", lineThickness);
        cs.SetInt("_nodeBufferSize", trailBrush.nodeNum);
        cs.SetBuffer(kernelId, "_NodeBuffer", trailBrush.trailDatas[(int)TrailType.Particle].nodeBuffer);
        // コンピュートバッファをセット
        cs.SetBuffer(kernelId, "_ParticleBuffer", particleBuffer);
        // コンピュートシェーダを実行
        cs.Dispatch(kernelId, trailBrush.nodeNum, 1, 1);


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
        m.SetBuffer("_NodeBuffer", trailBrush.trailDatas[(int)TrailType.Particle].nodeBuffer);
        m.SetColor("_ParticleColor", particleColor);
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

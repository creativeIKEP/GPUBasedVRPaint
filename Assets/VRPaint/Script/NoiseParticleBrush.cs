﻿using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TrailBase;



public class NoiseParticleBrush : MonoBehaviour
{
    const int NUM_PARTICLES = 327680; // 生成するパーティクルの数

    const int NUM_THREAD_X = 8; // スレッドグループのX成分のスレッド数
    const int NUM_THREAD_Y = 1; // スレッドグループのY成分のスレッド数
    const int NUM_THREAD_Z = 1; // スレッドグループのZ成分のスレッド数

    public ComputeShader SimpleParticleComputeShader; // パーティクルの動きを計算するコンピュートシェーダ
    public Shader SimpleParticleRenderShader;  // パーティクルをレンダリングするシェーダ

    public Texture2D ParticleTex;          // パーティクルのテクスチャ
    public float ParticleSize = 0.05f; // パーティクルのサイズ

    public Camera RenderCam; // パーティクルをレンダリングするカメラ（ビルボードのための逆ビュー行列計算に使用）
    public float lifeTime = 1.0f;
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
            pData[i].GeneratedNodeId = i * trailBrush.nodeNum / NUM_PARTICLES;
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
        int kernelId = cs.FindKernel("NoiseParticleCalc");

        cs.SetInt("_particleNum_n", NUM_PARTICLES);
        cs.SetFloat("_TimeStep_n", Time.deltaTime);
        cs.SetFloat("_nodeUpdateMin_n", trailBrush.updateDistaceMin);
        cs.SetBuffer(kernelId, "_TrailBuffer_n", trailBrush.trailDatas[(int)TrailType.NoiseParticle].trailBuffer);
        cs.SetBuffer(kernelId, "_NodeBuffer_n", trailBrush.trailDatas[(int)TrailType.NoiseParticle].nodeBuffer);
        // コンピュートバッファをセット
        cs.SetBuffer(kernelId, "_ParticleBuffer_n", particleBuffer);
        cs.SetFloat("_Time_n", Time.time);
        // コンピュートシェーダを実行
        cs.Dispatch(kernelId, trailBrush.nodeNum, 1, 1);


        // 逆ビュー行列を計算
        var inverseViewMatrix = RenderCam.worldToCameraMatrix.inverse;

        Material m = particleRenderMat;
        m.SetPass(0); // レンダリングのためのシェーダパスをセット
                      // 各パラメータをセット
        m.SetMatrix("_InvViewMatrix_n", inverseViewMatrix);
        m.SetTexture("_MainTex_n", ParticleTex);
        m.SetFloat("_ParticleSize_n", ParticleSize);
        // コンピュートバッファをセット
        m.SetBuffer("_ParticleBuffer_n", particleBuffer);
        m.SetBuffer("_NodeBuffer_n", trailBrush.trailDatas[(int)TrailType.NoiseParticle].nodeBuffer);
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

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/NoiseParticleBrush"
{
	CGINCLUDE
	#include "UnityCG.cginc"
	#include "GPUTrailsUtil.cginc"

	// パーティクルデータの構造体
	struct ParticleData
	{
		float3 velocity;
		float3 position;
		float3 acceleration;
		float lifeTime;
		float timeFromRePositioning;
		int generatedNodeId;
	};
	// VertexShaderからGeometryShaderに渡すデータの構造体
	struct v2g
	{
		float3 position : TEXCOORD0;
		float4 color    : COLOR;
	};
	// GeometryShaderからFragmentShaderに渡すデータの構造体
	struct g2f
	{
		float4 position : POSITION;
		float2 texcoord : TEXCOORD0;
		float4 color    : COLOR;
	};

	// パーティクルデータ
	StructuredBuffer<ParticleData> _ParticleBuffer_n;

	StructuredBuffer<Node> _NodeBuffer_n;

	// パーティクルのテクスチャ
	sampler2D _MainTex_n;
	float4    _MainTex_ST;
	// パーティクルサイズ
	float     _ParticleSize_n;
	// 逆ビュー行列
	float4x4  _InvViewMatrix_n;

	// Quadプレーンの座標
	static const float3 g_positions[4] =
	{
		float3(-1, 1, 0),
		float3( 1, 1, 0),
		float3(-1,-1, 0),
		float3( 1,-1, 0),
	};
	// QuadプレーンのUV座標
	static const float2 g_texcoords[4] =
	{
		float2(0, 0),
		float2(1, 0),
		float2(0, 1),
		float2(1, 1),
	};

	// --------------------------------------------------------------------
	// Vertex Shader
	// --------------------------------------------------------------------
	v2g vert(uint id : SV_VertexID) // SV_VertexID:頂点ごとの識別子
	{
		v2g o = (v2g)0;

		// パーティクルの位置
		o.position = _ParticleBuffer_n[id].position;
		// パーティクルの速度を色に反映
		//o.color = _ParticleColor;

		if (!IsValid(_NodeBuffer_n[_ParticleBuffer_n[id].generatedNodeId])) {
			o.color.a = 0.0;
		}
		else {
			o.color = _NodeBuffer_n[_ParticleBuffer_n[id].generatedNodeId].color;
		}
		return o;
	}

	// --------------------------------------------------------------------
	// Geometry Shader
	// --------------------------------------------------------------------
	[maxvertexcount(4)]
	void geom(point v2g In[1], inout TriangleStream<g2f> SpriteStream)
	{
		g2f o = (g2f)0;
		if (In[0].color.a != 0.0) {
			[unroll]
			for (int i = 0; i < 4; i++)
			{
				float3 position = g_positions[i] * _ParticleSize_n;
				position = mul(_InvViewMatrix_n, position) + In[0].position;
				o.position = UnityObjectToClipPos(float4(position, 1.0));

				o.color = In[0].color;
				o.texcoord = g_texcoords[i];
				// 頂点追加
				SpriteStream.Append(o);
			}
		}
		// ストリップを閉じる
		SpriteStream.RestartStrip();
	}

	// --------------------------------------------------------------------
	// Fragment Shader
	// --------------------------------------------------------------------
	fixed4 frag(g2f i) : SV_Target
	{
		return tex2D(_MainTex_n, i.texcoord.xy) * i.color;
	}
	ENDCG
	
	SubShader
	{
		Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 100

		ZWrite Off
		Blend One One

		Pass
		{
			CGPROGRAM
			#pragma target   5.0
			#pragma vertex   vert
			#pragma geometry geom
			#pragma fragment frag
			ENDCG
		}
	}
}

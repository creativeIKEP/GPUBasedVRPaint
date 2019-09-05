Shader "GPUTrails/Trails" {

Properties {
	
}
   
SubShader {
Pass{
	Cull Off Fog { Mode Off } ZWrite Off 
	Blend One One

	CGPROGRAM
	#pragma target 5.0
	#pragma vertex vert
	#pragma geometry geom
	#pragma fragment frag

	#include "UnityCG.cginc"
	#include "GPUTrailsUtil.cginc"

	StructuredBuffer<Node> _NodeBuffer;
	int _currentNodeIdx;
	int _nodeBufferSize;
	float _Width;

	struct vs_out {
		float4 pos : POSITION0;
		float3 dir : TANGENT0;
		float4 col : COLOR0;
		float4 posNext: POSITION1;
		float3 dirNext : TANGENT1;
		float4 colNext : COLOR1;
	};

	struct gs_out {
		float4 pos : SV_POSITION;
		float4 col : COLOR;
	};

	Node GetNode(int id) {
		return _NodeBuffer[id % _nodeBufferSize];
	}

	vs_out vert (uint id : SV_VertexID)
	{
		vs_out Out;

		Node node0 = GetNode(id-1);
		Node node1 = GetNode(id); // current
		Node node2 = GetNode(id + 1);
		Node node3 = GetNode(id + 2);

		bool isLastNode = (_currentNodeIdx == (int)id);

		if ( isLastNode || !IsValid(node1) || node1.trailId != node2.trailId)
		{
			node0 = node1 = node2 = node3 = _NodeBuffer[_currentNodeIdx];
		}

		float3 pos1 = node1.position;
		float3 pos0 = IsValid(node0) ? node0.position : pos1;
		float3 pos2 = IsValid(node2) ? node2.position : pos1;
		float3 pos3 = IsValid(node3) ? node3.position : pos2;

		Out.pos = float4(pos1, 1);
		Out.posNext = float4(pos2, 1);

		Out.dir = normalize(pos2 - pos1);
		Out.dirNext = normalize(pos3 - pos2);

		Out.col = node1.color;
		Out.colNext = node2.color;

		return Out;
	}

	[maxvertexcount(4)]
	void geom (point vs_out input[1], inout TriangleStream<gs_out> outStream)
	{
		gs_out output0, output1, output2, output3;
		float3 pos = input[0].pos; 
		float3 dir = input[0].dir;
		float3 posNext = input[0].posNext; 
		float3 dirNext = input[0].dirNext;

		float3 camPos = _WorldSpaceCameraPos;
		float3 toCamDir = normalize(camPos - pos);
		float3 sideDir = normalize(cross(toCamDir, dir));

		float3 toCamDirNext = normalize(camPos - posNext);
		float3 sideDirNext = normalize(cross(toCamDirNext, dirNext));
		float width = _Width * 0.5;

		output0.pos = UnityWorldToClipPos(pos + (sideDir * width));
		output1.pos = UnityWorldToClipPos(pos - (sideDir * width));
		output2.pos = UnityWorldToClipPos(posNext + (sideDirNext * width));
		output3.pos = UnityWorldToClipPos(posNext - (sideDirNext * width));

		output0.col =
		output1.col = input[0].col;
		output2.col =
		output3.col = input[0].colNext;

		outStream.Append (output0);
		outStream.Append (output1);
		outStream.Append (output2);
		outStream.Append (output3);
	
		outStream.RestartStrip();
	}

	fixed4 frag (gs_out In) : COLOR
	{
		return In.col;
	}

	ENDCG
   
   }
}
}



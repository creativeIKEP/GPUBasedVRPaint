using UnityEngine;
using TrailBase;


public class TrailRender : MonoBehaviour
{
    public Material _material;
    public TrailBase.TrailBrush _trails;


    void OnRenderObject()
    {
        _material.SetInt(TrailBrush.CSPARAM.NODE_NUM_PER_TRAIL, _trails.nodeNum);
        _material.SetInt("_currentNodeIdx", _trails.trailDatas[(int)TrailType.Trail].currentNodeIdx);
        _material.SetInt("_nodeBufferSize", _trails.nodeNum);
        _material.SetFloat("_Width", _trails.width);
        _material.SetBuffer(TrailBrush.CSPARAM.NODE_BUFFER, _trails.trailDatas[(int)TrailType.Trail].nodeBuffer);
        _material.SetPass(0);

        var nodeNum = _trails.nodeNum;
        var trailNum = _trails.trailNum;
        Graphics.DrawProceduralNow(MeshTopology.Points, nodeNum);
    }
}
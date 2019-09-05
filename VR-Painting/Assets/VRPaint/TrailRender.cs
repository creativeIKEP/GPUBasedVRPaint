using UnityEngine;
using GPUBasedTrails;


public class TrailRender : MonoBehaviour
{
    public Material _material;
    GPUBasedTrails.TrailBrush _trails;


    private void Start()
    {
        _trails = GetComponent<TrailBrush>();
    }

    void OnRenderObject()
    {
        _material.SetInt(TrailBrush.CSPARAM.NODE_NUM_PER_TRAIL, _trails.nodeNum);
        _material.SetBuffer(TrailBrush.CSPARAM.TRAIL_BUFFER, _trails.trailDatas[(int)TrailType.Trail].trailBuffer);
        _material.SetBuffer(TrailBrush.CSPARAM.NODE_BUFFER, _trails.trailDatas[(int)TrailType.Trail].nodeBuffer);
        _material.SetPass(0);

        var nodeNum = _trails.nodeNum;
        var trailNum = _trails.trailNum;
        Graphics.DrawProceduralNow(MeshTopology.Points, nodeNum, trailNum);
    }
}

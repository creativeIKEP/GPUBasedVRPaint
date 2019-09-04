using UnityEngine;


public class TrailRender : MonoBehaviour
{
    public Material _material;
    GPUBasedTrails.TrailBrush _trails;


    private void Start()
    {
        _trails = GetComponent<GPUBasedTrails.TrailBrush>();
    }

    void OnRenderObject()
    {
        _material.SetInt(GPUBasedTrails.TrailBrush.CSPARAM.NODE_NUM_PER_TRAIL, _trails.nodeNum);
        _material.SetBuffer(GPUBasedTrails.TrailBrush.CSPARAM.TRAIL_BUFFER, _trails.trailBuffer);
        _material.SetBuffer(GPUBasedTrails.TrailBrush.CSPARAM.NODE_BUFFER, _trails.nodeBuffer);
        _material.SetPass(0);

        var nodeNum = _trails.nodeNum;
        var trailNum = _trails.trailNum;
        Graphics.DrawProceduralNow(MeshTopology.Points, nodeNum, trailNum);
    }
}

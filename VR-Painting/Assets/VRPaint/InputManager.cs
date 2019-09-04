using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GPUBasedTrails;

public class InputManager : MonoBehaviour
{
    public TrailBrush trailBrush;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            // Vector3でマウス位置座標を取得する
            var position = Input.mousePosition;
            // Z軸修正
            position.z = 10f;
            // マウス位置座標をスクリーン座標からワールド座標に変換する
            var pos = Camera.main.ScreenToWorldPoint(position);
            TrailBrush.Input nodeInput = new TrailBrush.Input { pos = pos };
            trailBrush.InputPoint(new List<TrailBrush.Input> { nodeInput });
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using GPUBasedTrails;

public class InputManager : MonoBehaviour
{
    public GPUBasedTrails.TrailBrush trailBrush;
    bool isNewTrailInput;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isNewTrailInput = true;
        }

        if (Input.GetMouseButton(0))
        {
            // Vector3でマウス位置座標を取得する
            var position = Input.mousePosition;
            // Z軸修正
            position.z = 10f;
            // マウス位置座標をスクリーン座標からワールド座標に変換する
            var pos = Camera.main.ScreenToWorldPoint(position);
            GPUBasedTrails.Input nodeInput = new GPUBasedTrails.Input { pos = pos };
            trailBrush.InputPoint(nodeInput, isNewTrailInput);
            isNewTrailInput = false;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isNewTrailInput = false;
        }
    }
}

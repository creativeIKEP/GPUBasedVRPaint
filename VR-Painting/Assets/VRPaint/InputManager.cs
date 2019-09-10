using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputManager : MonoBehaviour
{
    public Camera camera;
    public TrailBase.TrailBrush trailBrush;
    bool isNewTrailInput;

    public GameObject hand;
    SteamVRControllerInput controllerInput;


    private void Start()
    {
        controllerInput = GetComponent<SteamVRControllerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controllerInput.trrigerClick.GetStateDown(controllerInput.hand))
        {
            isNewTrailInput = true;
        }

        if (controllerInput.trrigerClick.GetState(controllerInput.hand))
        {
            // Vector3でマウス位置座標を取得する
            var position = Input.mousePosition;
            // Z軸修正
            position.z = 10f;
            // マウス位置座標をスクリーン座標からワールド座標に変換する
            var pos = camera.ScreenToWorldPoint(position);

            pos = hand.transform.position;

            TrailBase.Input nodeInput = new TrailBase.Input { pos = pos };
            trailBrush.InputPoint(nodeInput, isNewTrailInput);
            isNewTrailInput = false;
        }

        if (controllerInput.trrigerClick.GetStateUp(controllerInput.hand))
        {
            isNewTrailInput = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SteamVRControllerInput))]
public class InputManager : MonoBehaviour
{
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
            var pos = hand.transform.position;

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

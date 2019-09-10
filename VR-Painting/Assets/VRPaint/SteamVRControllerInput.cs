using UnityEngine;
using Valve.VR;


public class SteamVRControllerInput : MonoBehaviour
{
    public SteamVR_Input_Sources hand;
    public SteamVR_Action_Boolean gripAction;

    public SteamVR_Action_Boolean trrigerClick;

    public SteamVR_Action_Boolean padClick;
    public SteamVR_Action_Boolean padTouch;
    public SteamVR_Action_Vector2 padPosition;


    void Update()
    {
        if (gripAction.GetState(hand))
        {
            Debug.Log("gripAction");
        }

        if (trrigerClick.GetState(hand))
        {
            Debug.Log("trrigerClick");
        }
        

        if (padClick.GetState(hand))
        {
            Debug.Log("padClick");
        }
        if (padTouch.GetState(hand))
        {
            Debug.Log("padTouch");
            Debug.Log(padPosition.GetAxis(hand));
            Debug.Log(padPosition.GetAxisDelta(hand));
        }
    }
}

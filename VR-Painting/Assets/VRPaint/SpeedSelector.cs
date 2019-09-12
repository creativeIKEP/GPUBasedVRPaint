using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedSelector : MonoBehaviour
{
    public SteamVRControllerInput controllerInput;
    public TextMesh textMesh;
    public float speedDeltaMax = 0.001f;
    public float maxSpeed = 100.0f;
    public float minSpeed = 0.0f;

    float currentSpeed;
    bool isSelecting;
    EndEdit endEditCallBack;

    // Start is called before the first frame update
    void Start()
    {
        isSelecting = false;
        gameObject.SetActive(false);
    }


    private void Update()
    {
        if (isSelecting && controllerInput.padTouch.GetState(controllerInput.hand))
        {
            var padPos = controllerInput.padPosition.GetAxis(controllerInput.hand);
            if (padPos.y >= 0)
            {
                currentSpeed += padPos.y * speedDeltaMax;
                if (currentSpeed > maxSpeed)
                {
                    currentSpeed = maxSpeed;
                }
                textMesh.text = currentSpeed.ToString();
            }
            else if (padPos.y <= 0)
            {
                currentSpeed += padPos.y * speedDeltaMax;
                if (currentSpeed < minSpeed)
                {
                    currentSpeed = minSpeed;
                }
                textMesh.text = currentSpeed.ToString();
            }
        }

        if (isSelecting && controllerInput.padClick.GetStateUp(controllerInput.hand))
        {
            isSelecting = false;
            endEditCallBack(currentSpeed);
            gameObject.SetActive(false);
        }
    }


    public delegate void EndEdit(float speed);

    public void StartSelect(float initspeed, EndEdit callback)
    {
        currentSpeed = initspeed;
        textMesh.text = currentSpeed.ToString();
        isSelecting = true;
        gameObject.SetActive(true);
        endEditCallBack = callback;
    }
}

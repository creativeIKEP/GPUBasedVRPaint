using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WidthSelector : MonoBehaviour
{
    public SteamVRControllerInput controllerInput;
    public TextMesh textMesh;
    public float widthDeltaMax = 0.001f;
    public float maxWidth = 0.1f;
    public float minWidth = 0.01f;

    float currentWidth;
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
                currentWidth += padPos.y * widthDeltaMax;
                if (currentWidth > maxWidth)
                {
                    currentWidth = maxWidth;
                }
                textMesh.text = currentWidth.ToString();
            }
            else if (padPos.y <= 0)
            {
                currentWidth += padPos.y * widthDeltaMax;
                if (currentWidth < minWidth)
                {
                    currentWidth = minWidth;
                }
                textMesh.text = currentWidth.ToString();
            }
        }

        if (isSelecting && controllerInput.padClick.GetStateUp(controllerInput.hand))
        {
            isSelecting = false;
            endEditCallBack(currentWidth);
            gameObject.SetActive(false);
        }
    }


    public delegate void EndEdit(float width);

    public void StartSelect(float initWidth, EndEdit callback)
    {
        currentWidth = initWidth;
        textMesh.text = currentWidth.ToString();
        isSelecting = true;
        gameObject.SetActive(true);
        endEditCallBack = callback;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SteamVRControllerInput))]
public class LineParamSelector : MonoBehaviour
{
    public TrailBase.TrailBrush trailBrush;
    public GameObject padUi;
    public LineTypeSelector lineTypeSelector;
    public ColorPickerTriangle_VR colorPickerTriangle_VR;
    public WidthSelector widthSelector;
    public SpeedSelector speedSelector;

    SteamVRControllerInput controllerInput;
    bool isEditing;

    private void Start()
    {
        controllerInput = GetComponent<SteamVRControllerInput>();
        isEditing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isEditing)
        {
            return;
        }

        if (controllerInput.padClick.GetStateUp(controllerInput.hand))
        {
            var padPos = controllerInput.padPosition.GetAxis(controllerInput.hand);
            if (padPos.x < 0 && padPos.y >= 0)
            {
                ChangeType();
            }
            if (padPos.x >= 0 && padPos.y >= 0)
            {
                ChangeColor();
            }
            if (padPos.x < 0 && padPos.y < 0)
            {
                ChangeWidth();
            }
            if(padPos.x>=0 && padPos.y < 0)
            {
                ChangeSpeed();
            }
        }
    }


    void ChangeType()
    {
        isEditing = true;
        padUi.SetActive(false);
        lineTypeSelector.StartSelect(trailBrush.currentTrailType, (TrailBase.TrailType type) =>
        {
            isEditing = false;
            padUi.SetActive(true);
            trailBrush.currentTrailType = type;
        });
    }


    void ChangeColor()
    {
        isEditing = true;
        padUi.SetActive(false);
        colorPickerTriangle_VR.StartColorSelect(trailBrush.color, (Color selectedColor) =>
        {
            isEditing = false;
            trailBrush.color = selectedColor;
            padUi.SetActive(true);
        });
    }

    void ChangeWidth()
    {
        isEditing = true;
        padUi.SetActive(false);
        widthSelector.StartSelect(trailBrush.width, (float width) =>
        {
            isEditing = false;
            trailBrush.width = width;
            padUi.SetActive(true);
        });
    }


    void ChangeSpeed()
    {
        isEditing = true;
        padUi.SetActive(false);
        speedSelector.StartSelect(trailBrush.particleSpeed, (float speed) =>
        {
            isEditing = false;
            trailBrush.particleSpeed = speed;
            padUi.SetActive(true);
        });
    }
}

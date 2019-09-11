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
}

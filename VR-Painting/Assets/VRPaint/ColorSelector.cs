using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SteamVRControllerInput))]
public class ColorSelector : MonoBehaviour
{
    public TrailBase.TrailBrush trailBrush;
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
            isEditing = true;
            colorPickerTriangle_VR.StartColorSelect((Color selectedColor) =>
            {
                isEditing = false;
                trailBrush.color = selectedColor;
            });
        }
    }
}

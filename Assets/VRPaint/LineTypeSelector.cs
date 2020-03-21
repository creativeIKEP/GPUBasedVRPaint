using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTypeSelector : MonoBehaviour
{
    public SteamVRControllerInput controllerInput;
    public SpriteRenderer trailSprite;
    public SpriteRenderer particleSprite;
    public SpriteRenderer noiseParticleSprite;
    public Color selectedColor;
    public Color notSelectedColor;

    TrailBase.TrailType currentType;
    bool isSelecting;
    EndEdit endEditCallBack;

    // Start is called before the first frame update
    void Start()
    {
        currentType = TrailBase.TrailType.Trail;
        ChangeSpriteColor();
        isSelecting = false;
        gameObject.SetActive(false);
    }


    private void Update()
    {
        if (isSelecting && controllerInput.padTouch.GetState(controllerInput.hand))
        {
            var padPos = controllerInput.padPosition.GetAxis(controllerInput.hand);
            if (padPos.y >= 2.0f / 3)
            {
                currentType = TrailBase.TrailType.Trail;
                ChangeSpriteColor();
            }
            else if(padPos.y < 2.0f / 3 && padPos.y > -2.0f / 3)
            {
                currentType = TrailBase.TrailType.Particle;
                ChangeSpriteColor();
            }
            else
            {
                currentType = TrailBase.TrailType.NoiseParticle;
                ChangeSpriteColor();
            }
        }

        if(isSelecting && controllerInput.padClick.GetStateUp(controllerInput.hand))
        {
            isSelecting = false;
            endEditCallBack(currentType);
            gameObject.SetActive(false);
        }
    }


    public delegate void EndEdit(TrailBase.TrailType type);

    public void StartSelect(TrailBase.TrailType initType, EndEdit callback)
    {
        currentType = initType;
        ChangeSpriteColor();
        isSelecting = true;
        gameObject.SetActive(true);
        endEditCallBack = callback;
    }


    void ChangeSpriteColor()
    {
        trailSprite.color = (currentType == TrailBase.TrailType.Trail) ? selectedColor : notSelectedColor;
        particleSprite.color = (currentType == TrailBase.TrailType.Particle) ? selectedColor : notSelectedColor;
        noiseParticleSprite.color = (currentType == TrailBase.TrailType.NoiseParticle) ? selectedColor : notSelectedColor;
    }
}

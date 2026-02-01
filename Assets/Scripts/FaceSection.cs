using UnityEngine;
using UnityEngine.UI;

public class FaceSection : MonoBehaviour
{
    public Image background; // The 1x1 grid square
    public Image feature;    // The Eye/Nose/Mouth sprite

    public void SetColor(Color newColor) => background.color = newColor;

    public void SetFeature(Sprite newSprite)
    {
        feature.sprite = newSprite;
        // Hide the image if there is no sprite
        feature.enabled = (newSprite != null);
    }
}
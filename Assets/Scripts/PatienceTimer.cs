using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PatienceTimer : MonoBehaviour
{
    public event Action OnPatienceExpired;

    [Header("Patience Timer Settings")]
    [SerializeField] public Color startColor = Color.green;
    [SerializeField] public Color endColor = Color.red;
    [SerializeField] public float patienceDuration = 5.0f;

    private Image fillImage; // The front clock face that depletes

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {      
        // Set up the fill image for radial fill
        fillImage = GetComponent<Image>();
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Radial360;
        fillImage.fillOrigin = (int)Image.Origin360.Top; // Start from top
        fillImage.fillClockwise = false; // Fill counter-clockwise
        fillImage.fillAmount = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        Billboard();
    }

    public IEnumerator StartPatienceTimer(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            
            // Change color from start to end
            if (fillImage != null)
            {
                fillImage.color = Color.Lerp(startColor, endColor, t);
                fillImage.fillAmount = 1f - t; // Deplete the clock
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the color is set to the end color at the end
        if (fillImage != null)
        {
            fillImage.color = endColor;
            fillImage.fillAmount = 0f;
        }
        
        Debug.Log("Patience timer expired.");
        OnPatienceExpired?.Invoke();

        gameObject.SetActive(false); // Hide the timer when done
    }

    private void Billboard()
    {
        transform.LookAt(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z));
    }
}

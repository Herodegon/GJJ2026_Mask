using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceGridManager : MonoBehaviour
{
    [Header("Grid Sections (Assign 9)")]
    public RectTransform[] sections; // Assign your 9 grid squares here (row-major: 0=UL, 8=LR)

    [Header("Section Images (Assign 9)")]
    public Image[] sectionImages; // Assign 9 UI Images for color control (same order as sections)

    [Header("Feature Prefabs")]
    public GameObject[] featurePrefabs; // Eye, nose, mouth, etc.

    // Track features in each section
    private List<GameObject>[] sectionFeatures = new List<GameObject>[9];

    void Awake()
    {
        for (int i = 0; i < sectionFeatures.Length; i++)
            sectionFeatures[i] = new List<GameObject>();
    }

    /// <summary>
    /// Place a feature prefab in a section at a local position.
    /// </summary>
    /// <param name="sectionIndex">0-8 (Upper Left to Lower Right)</param>
    /// <param name="featurePrefab">The Eye/Nose/Mouth prefab</param>
    /// <param name="localPosition">Offset within that section</param>
    public void PlaceFeature(int sectionIndex, GameObject featurePrefab, Vector2 localPosition)
    {
        if (sectionIndex < 0 || sectionIndex >= sections.Length) return;
        GameObject newPart = Instantiate(featurePrefab, sections[sectionIndex]);
        RectTransform rt = newPart.GetComponent<RectTransform>();
        rt.anchoredPosition = localPosition;
        sectionFeatures[sectionIndex].Add(newPart);
    }

    /// <summary>
    /// Remove all features from a section.
    /// </summary>
    public void ClearSectionFeatures(int sectionIndex)
    {
        if (sectionIndex < 0 || sectionIndex >= sectionFeatures.Length) return;
        foreach (var go in sectionFeatures[sectionIndex])
            if (go) Destroy(go);
        sectionFeatures[sectionIndex].Clear();
    }

    /// <summary>
    /// Set the color of a section (UI Image).
    /// </summary>
    public void SetSectionColor(int sectionIndex, Color color)
    {
        if (sectionIndex < 0 || sectionIndex >= sectionImages.Length) return;
        sectionImages[sectionIndex].color = color;
    }

    /// <summary>
    /// Procedurally generate a random face (colors/features/positions).
    /// </summary>
    public void ProcedurallyGenerateFace()
    {
        for (int i = 0; i < 9; i++)
        {
            SetSectionColor(i, Random.ColorHSV());
            ClearSectionFeatures(i);
            if (featurePrefabs.Length > 0 && Random.value > 0.5f)
            {
                int featureIdx = Random.Range(0, featurePrefabs.Length);
                // Place feature at random position within section (assuming 100x100 size)
                Vector2 randPos = new Vector2(Random.Range(-30, 30), Random.Range(-30, 30));
                PlaceFeature(i, featurePrefabs[featureIdx], randPos);
            }
        }
    }

    /// <summary>
    /// Compare this face to another FaceGridManager (color/features per section).
    /// </summary>
    public bool CompareFace(FaceGridManager other)
    {
        for (int i = 0; i < 9; i++)
        {
            if (sectionImages[i].color != other.sectionImages[i].color)
                return false;
            if (sectionFeatures[i].Count != other.sectionFeatures[i].Count)
                return false;
            // Optionally compare feature types/positions for stricter matching
        }
        return true;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TextRender : MonoBehaviour
{
    [SerializeField] private float shakeIntensity = 2f;
    [SerializeField] private float shakeSpeed = 5f; // Controls how fast the shake is

    private TMP_Text tmpText;

    void Start()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        tmpText.ForceMeshUpdate();

        TMP_TextInfo textInfo = GetComponent<TMP_Text>().textInfo;
        
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
                continue;
            Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            // Get the index of the first vertex of the character
            int vertexIndex = charInfo.vertexIndex;

            // Generate random offsets for shake effect
            float offsetX = Mathf.PerlinNoise(Time.time * shakeSpeed + i, 0) * shakeIntensity;
            float offsetY = Mathf.Sin(Time.time * shakeSpeed + i) * shakeIntensity;

            // Apply transformation to each vertex of the character
            vertices[vertexIndex + 0] += new Vector3(offsetX, offsetY, 0); // Bottom Left
            vertices[vertexIndex + 1] += new Vector3(offsetX, offsetY, 0); // Top Left
            vertices[vertexIndex + 2] += new Vector3(offsetX, offsetY, 0); // Top Right
            vertices[vertexIndex + 3] += new Vector3(offsetX, offsetY, 0); // Bottom Right
        }
        
        // Update the mesh with the modified vertex positions
        tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }
}

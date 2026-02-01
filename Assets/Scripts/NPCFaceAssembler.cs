
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Assembles an NPC face by mixing and matching pieces from multiple face sets.
/// Each face set contains 9 sprites (3x3 grid).
/// </summary>
/// <summary>
/// Represents a set of 9 face piece sprites for one face.
/// </summary>
[System.Serializable]
public class FacePieceSet
{
    [Tooltip("Assign 9 sprites for this face (row-major order)")]
    public List<Sprite> pieces = new List<Sprite>(9);
}

public class NPCFaceAssembler : MonoBehaviour
{
    [Header("Face Piece Sets (Each: 9 Sprites)")]
    public List<FacePieceSet> facePieceSets; // Each FacePieceSet contains 9 sprites

    [Header("Grid Settings")]
    public int pieceSize = 1; // Size of each piece in pixels
    public int gridRows = 3;
    public int gridCols = 3;

    [Header("Parent for SpriteRenderers")]
    public Transform faceParent; // Where to instantiate SpriteRenderers

    private List<GameObject> facePieces = new List<GameObject>();

    /// <summary>
    /// Generate a new NPC face by randomly mixing pieces from all face sets.
    /// </summary>
    public void GenerateRandomFace()
    {
        ClearFace();
        if (facePieceSets == null || facePieceSets.Count == 0)
        {
            Debug.LogWarning($"[NPCFaceAssembler] No facePieceSets assigned.");
            return;
        }
        if (faceParent == null)
        {
            Debug.LogWarning($"[NPCFaceAssembler] faceParent is not assigned.");
            return;
        }
        for (int i = 0; i < gridRows * gridCols; i++)
        {
            int faceIdx = Random.Range(0, facePieceSets.Count);
            var set = facePieceSets[faceIdx];
            if (set.pieces.Count <= i || set.pieces[i] == null)
            {
                Debug.LogWarning($"[NPCFaceAssembler] Missing sprite for piece {i} in set {faceIdx}.");
                continue;
            }
            Sprite piece = set.pieces[i];
            GameObject go = new GameObject($"FacePiece_{i}");
            go.transform.SetParent(faceParent, false);
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = piece;
            sr.sortingOrder = i; // Ensure each piece is drawn above the previous
            int row = i / gridCols;
            int col = i % gridCols;
            float zOffset = -i * 0.01f;
            go.transform.localPosition = new Vector3(col * pieceSize, -row * pieceSize, zOffset);
            facePieces.Add(go);
            Debug.Log($"[NPCFaceAssembler] Created piece {i} from set {faceIdx} at ({col},{row}) with sprite '{piece.name}', sortingOrder {i}, localPos {go.transform.localPosition}, worldPos {go.transform.position}. Pixels per unit: {piece.pixelsPerUnit}.");
        }
    }

    /// <summary>
    /// Clear all face pieces from the parent.
    /// </summary>
    public void ClearFace()
    {
        foreach (var go in facePieces)
            if (go) Destroy(go);
        facePieces.Clear();
    }

    /// <summary>
    /// Generate a face from a specific selection of pieces.
    /// </summary>
    /// <param name="pieceIndices">Array of length 9: each value is the face set index to use for that piece.</param>
    public void GenerateFaceFromSelection(int[] pieceIndices)
    {
        ClearFace();
        if (facePieceSets == null || facePieceSets.Count == 0 || pieceIndices.Length != gridRows * gridCols) return;
        for (int i = 0; i < gridRows * gridCols; i++)
        {
            int faceIdx = Mathf.Clamp(pieceIndices[i], 0, facePieceSets.Count - 1);
            var set = facePieceSets[faceIdx];
            if (set.pieces.Count <= i) continue;
            Sprite piece = set.pieces[i];
            GameObject go = new GameObject($"FacePiece_{i}");
            go.transform.SetParent(faceParent, false);
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = piece;
            sr.sortingOrder = i;
            int row = i / gridCols;
            int col = i % gridCols;
            float zOffset = -i * 0.01f;
            go.transform.localPosition = new Vector3(col * pieceSize, -row * pieceSize, zOffset);
            facePieces.Add(go);
            Debug.Log($"[NPCFaceAssembler] Created piece {i} from set {faceIdx} at ({col},{row}) with sprite '{piece.name}', sortingOrder {i}, localPos {go.transform.localPosition}, worldPos {go.transform.position}. Pixels per unit: {piece.pixelsPerUnit}.");
        }
    }

    public void Start()
    {
        Debug.Log("[NPCFaceAssembler] Start() called. Generating random face.");
        GenerateRandomFace();
    }
}

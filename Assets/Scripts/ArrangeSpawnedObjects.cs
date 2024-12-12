using UnityEngine;
using System.Collections.Generic;

public class ArrangeSpawnedObjects : MonoBehaviour
{
    public enum ObjectTag
    {
        DataBlock,
        ServerPlatform
        
    }

    public enum ArrangeSide
    {
        Left,
        Right
    }

    [Header("Configuration")]
    public ObjectTag objectTag = ObjectTag.DataBlock;
    public ArrangeSide arrangeSide = ArrangeSide.Left;
    
    [Header("Layout Settings")]
    [SerializeField] private int _maxObjectsPerRow = 3;
    [SerializeField] private float _laptopSpaceWidth = 0.5f; // Configurable space for laptop
    [SerializeField] private float _insetFromEdge = 0.25f; // How far from edge to start arranging

    private List<GameObject> objectsToArrange = new List<GameObject>();

    public void FindAndArrangeObjects()
    {
        objectsToArrange.Clear();
        string tagToSearch = objectTag.ToString();
        GameObject[] foundObjects = GameObject.FindGameObjectsWithTag(tagToSearch);
        objectsToArrange.AddRange(foundObjects);

        GameObject table = GameObject.Find("TABLE");
        if (table == null)
        {
            Debug.LogWarning("No table named 'TABLE' found in the scene!");
            return;
        }

        Renderer tableRenderer = table.GetComponent<Renderer>();
        if (tableRenderer == null)
        {
            Debug.LogWarning("Table does not have a Renderer component!");
            return;
        }

        ArrangeObjectsOnTable(tableRenderer.bounds);
    }

    private void ArrangeObjectsOnTable(Bounds tableBounds)
    {
        if (objectsToArrange.Count == 0)
        {
            Debug.LogWarning("No objects to arrange!");
            return;
        }

        // Calculate rows needed
        int totalRows = Mathf.CeilToInt((float)objectsToArrange.Count / _maxObjectsPerRow);
        
        // Calculate spacing
        float objectSpacingX = tableBounds.size.x * 0.15f; // Space between objects horizontally
        float objectSpacingZ = tableBounds.size.z * 0.15f; // Space between rows
        
        // Calculate start position
        float startX = arrangeSide == ArrangeSide.Left 
            ? tableBounds.center.x - _laptopSpaceWidth - _insetFromEdge
            : tableBounds.center.x + _insetFromEdge;

        float startZ = tableBounds.center.z + (objectSpacingZ * (totalRows - 1) / 2);

        for (int i = 0; i < objectsToArrange.Count; i++)
        {
            int row = i / _maxObjectsPerRow;
            int col = i % _maxObjectsPerRow;

            // Calculate position for this object
            float xPos = startX + (col * objectSpacingX);
            float zPos = startZ - (row * objectSpacingZ);
            
            Vector3 newPosition = new Vector3(
                xPos,
                tableBounds.center.y,
                zPos
            );

            objectsToArrange[i].transform.position = newPosition;
        }
    }
}
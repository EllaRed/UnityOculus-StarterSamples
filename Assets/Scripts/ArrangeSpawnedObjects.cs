using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

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

    private void OnEnable()
    {
        ObjectSpawnManager.Instance.OnObjectSpawned.AddListener(HandleNewObjectSpawned);
    }

    private void OnDisable()
    {
        ObjectSpawnManager.Instance.OnObjectSpawned.RemoveListener(HandleNewObjectSpawned);
    }

    public void HandleNewObjectSpawned(GameObject newObject)
    {
        // Only handle objects with matching tag
        if (newObject.CompareTag(objectTag.ToString()) && !objectsToArrange.Contains(newObject))
        {
            objectsToArrange.Add(newObject);
            ArrangeAllObjects();
        }
    }

    [Button("Arrange All Objects")]
    public void ArrangeAllObjects()
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

        Renderer tableRenderer = table.GetComponentInChildren<Renderer>();
        if (tableRenderer == null)
        {
            Debug.LogWarning("Table does not have a Renderer component!");
            return;
        }

        ArrangeObjectsOnTable(tableRenderer.bounds);
    }

    private void ArrangeObjectsOnTable(Bounds tableBounds)
    {
        if (objectsToArrange.Count == 0) return;

        float objectSpacing = 0.3f;
        int objectsPerRow = 3;

        Dictionary<GameObject, (Rigidbody rb, bool wasKinematic)> rigidbodyStates = new();

        foreach (var obj in objectsToArrange)
        {
            obj.transform.SetParent(null);
            if (obj.TryGetComponent<Rigidbody>(out var rb))
            {
                rigidbodyStates[obj] = (rb, rb.isKinematic);
                rb.isKinematic = true;
            }
        }

        try
        {
            // Calculate starting Z position based on side
            float startZ = arrangeSide == ArrangeSide.Left
                ? tableBounds.min.z + 0.3f                     // Left side
                : tableBounds.center.z + (objectSpacing * 0.5f); // Right side

            for (int i = 0; i < objectsToArrange.Count; i++)
            {
                int row = i / objectsPerRow;
                int col = i % objectsPerRow;

                Vector3 newPosition = new Vector3(
                    tableBounds.min.x + 0.3f + (row * objectSpacing),  // X: Back to front
                    tableBounds.max.y + 0.05f,                         // Y: Slightly above table
                    startZ + (col * objectSpacing)                     // Z: Based on side + column offset
                );

                objectsToArrange[i].transform.position = newPosition;
            }
        }
        finally
        {
            foreach (var kvp in rigidbodyStates)
            {
                kvp.Value.rb.isKinematic = kvp.Value.wasKinematic;
            }
        }
    }
}
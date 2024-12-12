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
    [SerializeField] private float _insetFromEdge = 0.25f;

    [Header("Spacing Settings")]
    [SerializeField] private float _spacingMultiplier = 1.2f; // Multiplier for space between objects
    [SerializeField] private float _minimumSpacing = 0.3f; // Minimum space between objects
    [SerializeField] private float _heightOffset = 0.05f; // Height above table

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
        if (newObject.CompareTag(objectTag.ToString()) && !objectsToArrange.Contains(newObject))
        {
            objectsToArrange.Add(newObject);
            ArrangeAllObjects();
        }
    }

    private Bounds GetObjectBounds(GameObject obj)
    {
        // Get all renderers (including children)
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            // Fallback to object's transform if no renderers found
            return new Bounds(obj.transform.position, Vector3.one * 0.2f);
        }

        // Start with the first renderer's bounds
        Bounds bounds = renderers[0].bounds;

        // Encapsulate all other renderers
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds;
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

        // Calculate maximum bounds for spacing
        float maxWidth = 0f;
        float maxDepth = 0f;
        Dictionary<GameObject, Bounds> objectBounds = new Dictionary<GameObject, Bounds>();

        foreach (var obj in objectsToArrange)
        {
            var bounds = GetObjectBounds(obj);
            objectBounds[obj] = bounds;
            maxWidth = Mathf.Max(maxWidth, bounds.size.x);
            maxDepth = Mathf.Max(maxDepth, bounds.size.z);
        }

        // Calculate spacing based on largest object
        float objectSpacing = Mathf.Max(_minimumSpacing,
            Mathf.Max(maxWidth, maxDepth) * _spacingMultiplier);

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
                ? tableBounds.min.z + _insetFromEdge + (maxDepth / 2)
                : tableBounds.center.z + (objectSpacing * 0.5f);

            for (int i = 0; i < objectsToArrange.Count; i++)
            {
                int row = i / _maxObjectsPerRow;
                int col = i % _maxObjectsPerRow;

                GameObject obj = objectsToArrange[i];
                Bounds objBounds = objectBounds[obj];

                // Calculate position with offset based on object's own bounds
                Vector3 newPosition = new Vector3(
                    tableBounds.min.x + _insetFromEdge + (row * objectSpacing) + (objBounds.size.x / 2),
                    tableBounds.max.y + _heightOffset + (objBounds.size.y / 2),
                    startZ + (col * objectSpacing)
                );

                // Smoothly move object to new position
                obj.transform.position = newPosition;
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

    private void OnDrawGizmosSelected()
    {
        // Visualize the arrangement area
        if (objectsToArrange.Count > 0)
        {
            GameObject table = GameObject.Find("TABLE");
            if (table != null)
            {
                Renderer tableRenderer = table.GetComponentInChildren<Renderer>();
                if (tableRenderer != null)
                {
                    Bounds tableBounds = tableRenderer.bounds;
                    Gizmos.color = Color.yellow;

                    // Draw the arrangement area
                    float width = _maxObjectsPerRow * _minimumSpacing;
                    float depth = (objectsToArrange.Count / _maxObjectsPerRow + 1) * _minimumSpacing;

                    Vector3 center = arrangeSide == ArrangeSide.Left
                        ? new Vector3(tableBounds.min.x + depth / 2, tableBounds.max.y + _heightOffset, tableBounds.min.z + width / 2)
                        : new Vector3(tableBounds.min.x + depth / 2, tableBounds.max.y + _heightOffset, tableBounds.center.z + width / 2);

                    Gizmos.DrawWireCube(center, new Vector3(depth, 0.1f, width));
                }
            }
        }
    }
}
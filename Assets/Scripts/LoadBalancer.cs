using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadBalancer : MonoBehaviour
{
    [SerializeField] private List<ServerPlatform> _servers;
    [SerializeField] private GameObject serverPrefab;
    [SerializeField] private Transform _cameraTransform; // Reference to VR camera/head
    [SerializeField] private float _textDistance = 2f; // How far in front of player to show text
    private bool _isBalanced;
    private bool _hasWon = false;

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
        if (newObject.CompareTag("ServerPlatform") && !newObject.TryGetComponent(out ServerPlatform server))
        {
            Debug.LogWarning("added server");
            AddServer(newObject);
        }
    }

    void Start()
    {
        // Get the VR camera if not set in inspector
        if (_cameraTransform == null)
        {
            _cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        CheckBalance();
    }

    private void CheckBalance()
    {
        //isbalanced if all servers are balanced
        if (_servers == null || _servers.Count == 0)
        {
            GetServers();
            return;
        }
        _isBalanced = _servers.All(server => server.isBalanced);
        if (_isBalanced && !_hasWon)
        {
            ShowWinText();
            _hasWon = true;
        }
    }

    private void ShowWinText()
    {
        // Create TextMesh GameObject
        GameObject winTextObj = new GameObject("WinText");
        TextMesh textMesh = winTextObj.AddComponent<TextMesh>();
        
        // Set text properties
        textMesh.text = "YOU WIN";
        textMesh.fontSize = 100;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = Color.green;

        // Position text in front of player
        Vector3 textPosition = _cameraTransform.position + (_cameraTransform.forward * _textDistance);
        winTextObj.transform.position = textPosition;
        
        // Make text face the player
        winTextObj.transform.LookAt(_cameraTransform);
        winTextObj.transform.Rotate(0, 180, 0); // Flip it so it faces the correct way
    }

    public void GetServers()
    {
        _servers = FindObjectsByType<ServerPlatform>(FindObjectsSortMode.None).ToList();
    }

    public void AddServer(GameObject server)
    {
        if (server.TryGetComponent(out ServerPlatform serverPlatform))
        {
            _servers.Add(serverPlatform);
        }
        
    }
}
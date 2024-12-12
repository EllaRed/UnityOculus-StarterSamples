using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class LoadBalancer : MonoBehaviour
{
    [SerializeField] private List<ServerPlatform> _servers;
    [SerializeField] private GameObject serverPrefab;
    private bool _isBalanced;

    void Update() {
        CheckBalance();
    }

    private void CheckBalance() {
        // Win condition logic
    }

    public void GetServers(){
        // Get all servers
        _servers = FindObjectsByType<ServerPlatform>(FindObjectsSortMode.None).ToList();
        
    }
}
using UnityEngine;
using System.Collections.Generic;
public class ServerPlatform : MonoBehaviour
{
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private Color _normalColor, _overloadedColor;
    private List<DataBlock> _dataBlocks = new();

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<DataBlock>(out var block)) {
            _dataBlocks.Add(block);
            UpdateVisuals();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.TryGetComponent<DataBlock>(out var block)) {
            _dataBlocks.Remove(block);
            UpdateVisuals();
        }
    }

    private void UpdateVisuals() {
        _renderer.material.color = _dataBlocks.Count > 3 ? _overloadedColor : _normalColor;
    }
}
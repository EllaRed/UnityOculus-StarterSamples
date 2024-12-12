using UnityEngine;
using UnityEngine.Events;
public class ObjectSpawnManager : MonoBehaviour
{
    public static ObjectSpawnManager Instance { get; private set; }
    public UnityEvent<GameObject> OnObjectSpawned;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NotifyObjectSpawned(GameObject obj)
    {
        OnObjectSpawned?.Invoke(obj);
    }
}
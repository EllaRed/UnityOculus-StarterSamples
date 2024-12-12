using UnityEngine;
using UnityEngine.SceneManagement;
public class DebugHelpers : MonoBehaviour
{
    int _sceneIndex = 0;
    public int _sceneCount = 3;
    [SerializeField] private Transform _player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _sceneCount = SceneManager.sceneCountInBuildSettings;
        _sceneIndex = SceneManager.GetActiveScene().buildIndex;
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetSpatialData(){
         _ = OVRScene.RequestSpaceSetup();
    }

    public void SwitchScene(){
        if(_sceneIndex == _sceneCount-1){
            _sceneIndex = 0; 
            SceneManager.LoadSceneAsync(_sceneIndex); 
            return;}
            
        SceneManager.LoadSceneAsync(++_sceneIndex);
    }

    public void SpawnCube(){
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.AddComponent<Rigidbody>();
        cube.AddComponent<BoxCollider>();
        //spawn near player
        cube.transform.position = _player.position + new Vector3(0, 0, 2);
    }
}

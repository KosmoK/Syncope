using UnityEngine;

public class PhoenixRoomCamera : MonoBehaviour
{
    Camera camera;
    void Start()
    {
        camera = GetComponent<Camera>();
        camera.aspect = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

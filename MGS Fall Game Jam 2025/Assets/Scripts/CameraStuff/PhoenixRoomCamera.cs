using Cinemachine;
using UnityEngine;

public class PhoenixRoomCamera : MonoBehaviour
{
    CinemachineVirtualCamera camera;
    void Start()
    {
        camera = GetComponent<CinemachineVirtualCamera>();
        // camera.m_Lens.Aspect = 1f; // No idea how to make this non-read only.
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

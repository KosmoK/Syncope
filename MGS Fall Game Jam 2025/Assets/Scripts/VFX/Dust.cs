using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.VFX;

public class Dust : MonoBehaviour
{
    [SerializeField] GameObject camObject;
    private Camera camera;
    private Transform cameraTransform;

    [SerializeField] VisualEffect dust;
    void Start()
    {
        camera = camObject.GetComponent<Camera>();
        cameraTransform = camObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float height = camera.orthographicSize * 2f;
        float width = height * camera.aspect;
        dust.SetFloat("CameraX", width);
        dust.SetFloat("CameraY", height);
        dust.SetVector2("CameraPOS", cameraTransform.position);
        
    }
}

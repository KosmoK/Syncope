using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    // Player Gameobject
    public GameObject followObject;
    public Vector3 offset = Vector3.zero;

    public bool changeSize;
    Camera camera;

    void Start()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (changeSize)
        {
            changeSizeFunc();
        }
        if(followObject != null)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(followObject.transform.position.x, followObject.transform.position.y, transform.position.z) + offset, 4f*Time.deltaTime);
        }
    }

    private void changeSizeFunc()
    {
        if (camera.orthographicSize < 25f)
        {
            camera.orthographicSize += 2f*Time.deltaTime;
        } else
        {
            camera.orthographicSize = 25f;
            changeSize = false;
        }
    }
}

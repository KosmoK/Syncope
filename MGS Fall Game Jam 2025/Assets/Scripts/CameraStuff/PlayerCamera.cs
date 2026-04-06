using System.Collections;
using Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    // Player Gameobject
    public GameObject followObject;
    public Vector3 offset = Vector3.zero;

    public bool changeSize;
    public CinemachineVirtualCamera v_cam;

    public bool shakeCamera;

    void Start()
    {
        v_cam = GetComponent<CinemachineVirtualCamera>();
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
        if (shakeCamera)
        {
            shakeCamera = false;
            ShakeCamera(3f, 3f, .5f);
        }
    }

    private void changeSizeFunc()
    {
        if (v_cam.m_Lens.OrthographicSize < 25f)
        {
            v_cam.m_Lens.OrthographicSize += 2f*Time.deltaTime;
        } else
        {
            v_cam.m_Lens.OrthographicSize = 25f;
            changeSize = false;
        }
    }

    public void ShakeCamera(float intensity, float frequency, float duration)
    {
        StartCoroutine(ShakeCoroutine(intensity, frequency, duration));
    }

    private IEnumerator ShakeCoroutine(float intensity, float frequency, float duration)
    {
        CinemachineBasicMultiChannelPerlin noise = v_cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin> ();

        noise.m_AmplitudeGain = intensity;
        noise.m_FrequencyGain = frequency;  

        yield return new WaitForSeconds(duration);
        noise.m_AmplitudeGain = 0;
        noise.m_FrequencyGain = 0;  
    }
}

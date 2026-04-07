using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Map : MonoBehaviour
{
    [SerializeField] float retractedVal;
    [SerializeField] float extendedVal;
    [SerializeField] AnimationCurve curve;
    float dist;
    private RectTransform rt;
    private InputAction mapAction;
    float rectY;
    bool toggled = false;
    float duration = 1f;

    void Start()
    {
        mapAction = InputSystem.actions.FindAction("Map");
        rt = GetComponent<RectTransform>();
        rectY = rt.anchoredPosition.y;
        rt.anchoredPosition = new Vector2(retractedVal, rectY);
    }

    // Update is called once per frame
    void Update()
    {
        if (mapAction.WasPressedThisFrame() && !toggled)
        {
            StartCoroutine(extendCoroutine());
        }
    }

    IEnumerator extendCoroutine()
    {
        toggled = !toggled;

        float t = 0;
        yield return null;
    }
}

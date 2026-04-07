using System.Collections;
using NUnit.Framework;
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
    private InputAction tutAction;
    float rectY;
    bool toggled = false;
    float duration = 1f;
    [SerializeField] bool isTut;

    void Start()
    {
        mapAction = InputSystem.actions.FindAction("Map");
        tutAction = InputSystem.actions.FindAction("Tutorial");
        rt = GetComponent<RectTransform>();
        rectY = rt.anchoredPosition.y;
        rt.anchoredPosition = new Vector2(retractedVal, rectY);

        dist = Mathf.Abs(extendedVal-retractedVal);
    }

    // Update is called once per frame
    void Update()
    {
        if (mapAction.WasPressedThisFrame() && !isTut)
        {
            StartCoroutine(extendCoroutine());
        }
        if (tutAction.WasPressedThisFrame() && isTut)
        {
            StartCoroutine(extendCoroutine());
        }
    }

    IEnumerator extendCoroutine()
    {
        toggled = !toggled;

        float t = 0;
        
        while (t < duration)
        {
            t += Time.deltaTime;
            float n = t/duration;
            float scale = curve.Evaluate(isTut ? (toggled ? n : 1-n) : (toggled ? 1-n : n));
            rt.anchoredPosition = new Vector2((isTut ? retractedVal : extendedVal) + dist * scale, rectY);

            yield return new WaitForEndOfFrame();
        }

        rt.anchoredPosition = new Vector2(toggled ? extendedVal : retractedVal, rectY);
    }
}

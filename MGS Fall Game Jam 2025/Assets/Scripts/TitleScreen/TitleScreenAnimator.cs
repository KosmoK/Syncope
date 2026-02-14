using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Image))]
public class TitleScreenAnimator : MonoBehaviour
{
    [SerializeField] public Animator animator; // Requires animator to be present in the same object as the script
    [SerializeField] public Image image; 
    [SerializeField] private float loadInTime = 1;
    private bool isSelectionScreen = false;
    [SerializeField] private bool loadingCoroutineFinished = false;
    public bool onlyZoomOut = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Start()
    {
        animator = GetComponent<Animator>();
        image = GetComponent<Image>();

        StartCoroutine(ZoomCoroutine(5, 1, loadInTime));
    }

    void Update()
    {
        if (InputSystem.actions.Any() && loadingCoroutineFinished && !isSelectionScreen && !onlyZoomOut)
        {
            isSelectionScreen = true;
            StartCoroutine(ZoomCoroutine(1, 5, loadInTime, new(0,0.5f,0)));
        }
    }

    IEnumerator ZoomCoroutine(float startScaleMult, float targetScaleMult, float loadInTime)
    { // Zooms UI Element dependent on the scale multiplication
        // Set current scale to the start scale
        transform.localScale = Vector3.one * startScaleMult;

        // Set the target scale
        Vector3 targetScale = Vector3.one * targetScaleMult;

        // Loopy
        while (transform.localScale != targetScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, 1/loadInTime * Time.deltaTime);
            if (targetScale.x/transform.localScale.x > 0.98f)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }
        loadingCoroutineFinished = true;
    }

    IEnumerator ZoomCoroutine(float startScaleMult, float targetScaleMult, float loadInTime, Vector3 newLocation)
    { // Zooms UI Element dependent on the scale multiplication and new location
        // Set current scale to the start scale
        transform.localScale = Vector3.one * startScaleMult;

        // Set the target scale
        Vector3 targetScale = Vector3.one * targetScaleMult;

        // Loopy
        while (transform.localScale != targetScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, 1/loadInTime * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, newLocation, 1/loadInTime * Time.deltaTime);

            if (transform.localScale.x/targetScale.x > 0.9f)
            {
                GameObject.FindGameObjectsWithTag("GameManager")[0].GetComponent<GameManager>().sceneTransition = true;
            }

            yield return new WaitForEndOfFrame();
        }
        loadingCoroutineFinished = true;
    }
}

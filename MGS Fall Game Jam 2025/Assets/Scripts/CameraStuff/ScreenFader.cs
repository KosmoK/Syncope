using System.Collections;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    CanvasGroup cg;
    Coroutine fader;
    float fadeDuration = 0.5f;
    public int alphaStatus = 0;

    // testing vars
    public bool fadeInVar = false;
    public bool fadeOutVar = false;
    void Start()
    {
        cg = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (fadeInVar)
        {
            fadeInVar = false;
            fadeInFunc();
        }
        if (fadeOutVar)
        {
            fadeOutVar = false;
            fadeOutFunc();
        }
    }

    public void fadeInFunc()
    {
        if (fader == null)
        {
            fader = StartCoroutine(fadeIn());
        }
    }

    public void fadeOutFunc()
    {
        if (fader == null)
        {
            fader = StartCoroutine(fadeOut());
        }
    }

    public IEnumerator fadeIn()
    {
        float time = 0;
        cg.alpha = 0;
        alphaStatus = 2;

        while (time < fadeDuration)
        {
            cg.alpha = time/fadeDuration;

            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        cg.alpha = 1;
        alphaStatus = 1;
        fader = null;
    }

    public IEnumerator fadeOut()
    {
        float time = 0;
        cg.alpha = 1;
        alphaStatus = 2;

        while (time < fadeDuration)
        {
            cg.alpha = 1-time/fadeDuration;

            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        cg.alpha = 0;
        alphaStatus = 0;
        fader = null;
    }

    public void setFadeDuration(float fd)
    {
        fadeDuration = fd;
    }

    
}

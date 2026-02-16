using System.Collections;
using UnityEngine;

public class Trader : MonoBehaviour
{
    [SerializeField] RectTransform primaryPanel;
    [SerializeField] AnimationCurve panelCurve;
    [SerializeField] RectTransform traderImage;
    [SerializeField] AnimationCurve traderCurve;
    [SerializeField] float animLength;

    private float initPosPanel = 845;
    private float initPosTrader = 1010;
    private float panelDist = 845;
    private float traderDist = 1010 - 610;
    private bool activated = false;
    
    public bool toggle = false;
    void Start()
    {
        primaryPanel.gameObject.SetActive(false);
        traderImage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (toggle)
        {
            toggle = false;
            activated = !activated;

            if (activated)
            {
                StartCoroutine(activateCoroutine());
            } else
            {
                StartCoroutine(deactivateCoroutine());
            }
        }
    }

    IEnumerator activateCoroutine()
    {
        primaryPanel.gameObject.SetActive(true);
        traderImage.gameObject.SetActive(true);

        float time = 0;

        float panelX = primaryPanel.anchoredPosition.x;
        float traderY = traderImage.anchoredPosition.y;

        while (time < animLength)
        {
            time += Time.deltaTime;

            primaryPanel.anchoredPosition = new Vector2(panelX, initPosPanel-panelCurve.Evaluate(time/animLength)*panelDist);
            traderImage.anchoredPosition = new Vector2(initPosTrader-traderCurve.Evaluate(time/animLength)*traderDist, traderY);

            yield return null;
        }
        
        primaryPanel.anchoredPosition = new Vector2(panelX, 0);
        traderImage.anchoredPosition = new Vector2(610, traderY);
    }

    IEnumerator deactivateCoroutine()
    {
        float time = 0;

        float panelX = primaryPanel.anchoredPosition.x;
        float traderY = traderImage.anchoredPosition.y;

        while (time < animLength)
        {
            time += Time.deltaTime;

            primaryPanel.anchoredPosition = new Vector2(panelX, initPosPanel-panelCurve.Evaluate(1-time/animLength)*panelDist);
            traderImage.anchoredPosition = new Vector2(initPosTrader-traderCurve.Evaluate(1-time/animLength)*traderDist, traderY);

            yield return null;
        }
        
        primaryPanel.anchoredPosition = new Vector2(panelX, initPosPanel);
        traderImage.anchoredPosition = new Vector2(initPosTrader, traderY);

        primaryPanel.gameObject.SetActive(false);
        traderImage.gameObject.SetActive(false);
    }
}

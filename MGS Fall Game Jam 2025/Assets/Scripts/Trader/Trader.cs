using System.Collections;
using TMPro;
using UnityEngine;

public class Trader : MonoBehaviour
{
    [SerializeField] RectTransform primaryPanel;
    [SerializeField] AnimationCurve panelCurve;
    [SerializeField] RectTransform traderImage;
    [SerializeField] AnimationCurve traderCurve;
    [SerializeField] float animLength;
    [SerializeField] GameObject moneyText;
    private TextMeshProUGUI tmp;
    private CurrencyManager currencyManager;

    private float initPosPanel = 990;
    private float initPosTrader = 520;
    private float panelDist = 990; // same as init pos since it just goes to 0
    private float traderDist = 520 - 0; // init - final
    private bool activated = false;
    
    void Start()
    {
        primaryPanel.gameObject.SetActive(false);
        traderImage.gameObject.SetActive(false);

        tmp = moneyText.GetComponent<TextMeshProUGUI>();

        currencyManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CurrencyManager>();
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if (toggle)
    //     {
    //         toggle = false;
    //         activated = !activated;

    //         if (activated)
    //         {
    //             StartCoroutine(activateCoroutine());
    //         } else
    //         {
    //             StartCoroutine(deactivateCoroutine());
    //         }
    //     }
    // }

    public void activateUI()
    {
        StartCoroutine(activateCoroutine());
    }
    public void deactivateUI()
    {
        StartCoroutine(deactivateCoroutine());
    }

    public void setFunds()
    {
        int money = currencyManager.getMoney();
        tmp.text = $"${money}";
    }

    IEnumerator activateCoroutine()
    {
        primaryPanel.gameObject.SetActive(true);
        // traderImage.gameObject.SetActive(true);

        float time = 0;

        float panelX = primaryPanel.anchoredPosition.x;
        // float traderY = traderImage.anchoredPosition.y;

        while (time < animLength)
        {
            time += Time.deltaTime;

            primaryPanel.anchoredPosition = new Vector2(panelX, initPosPanel-panelCurve.Evaluate(time/animLength)*panelDist);
            // traderImage.anchoredPosition = new Vector2(initPosTrader-traderCurve.Evaluate(time/animLength)*traderDist, traderY);

            yield return null;
        }
        
        primaryPanel.anchoredPosition = new Vector2(panelX, initPosPanel-panelDist);
        // traderImage.anchoredPosition = new Vector2(initPosTrader-traderDist, traderY);
    }

    IEnumerator deactivateCoroutine()
    {
        float time = 0;

        float panelX = primaryPanel.anchoredPosition.x;
        // float traderY = traderImage.anchoredPosition.y;

        while (time < animLength)
        {
            time += Time.deltaTime;

            primaryPanel.anchoredPosition = new Vector2(panelX, initPosPanel-panelCurve.Evaluate(1-time/animLength)*panelDist);
            // traderImage.anchoredPosition = new Vector2(initPosTrader-traderCurve.Evaluate(1-time/animLength)*traderDist, traderY);

            yield return null;
        }
        
        primaryPanel.anchoredPosition = new Vector2(panelX, initPosPanel);
        // traderImage.anchoredPosition = new Vector2(initPosTrader, traderY);

        primaryPanel.gameObject.SetActive(false);
        // traderImage.gameObject.SetActive(false);
    }
}

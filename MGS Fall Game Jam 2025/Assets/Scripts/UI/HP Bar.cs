using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    // pretty much how this is gonna work is that it's scaled to full size to start with and then we reduce
    // the masks to a certain percentage and can increment on that

    private float maxWidth = 600; // width of the maxed bar in pixels
    // private float currentMax = 0.5f; // percentage of maxed bar it fills
    private float maxHp = 150; // the hp of the maxed bar
    private float currentHp = 150;
    
    RectMask2D blankMask;
    RectMask2D redMask;
    [SerializeField] AnimationCurve redMaskCurve;
    [SerializeField] AnimationCurve blankMaskCurve;
    [SerializeField] float redMaskAnimLength;
    [SerializeField] float blankMaskAnimLength;
    [SerializeField] float animationPause;

    private bool updatingBar = false;
    private float prevHp;
    private float lagHp;
    private float currentLagHp;
    private float time;
    private float animationPauseTime;
    private bool blankMaskAnimFlag;
    public float takeDamageAmout;

    void Start()
    {
        blankMask = GetComponent<RectMask2D>();
        redMask = transform.GetChild(0).GetComponent<RectMask2D>();

        blankMask.padding = new Vector4(0, 0, maxWidth * (1-currentHp/maxHp), 0);
        redMask.padding = new Vector4(0, 0, maxWidth * (1-currentHp/maxHp), 0);
    }

    public void takeDamage(float amount)
    {
        prevHp = currentHp;
        animationPauseTime = 0;
        if (!blankMaskAnimFlag)
        {
            lagHp = prevHp;
            currentLagHp = lagHp;   
        }
        blankMaskAnimFlag = true;

        StartCoroutine(redMaskAnim(amount));
    }

    IEnumerator redMaskAnim(float amount)
    {
        time = 0;
        updatingBar = true;

        while (time < redMaskAnimLength)
        {
            time += Time.deltaTime;

            float scale = redMaskCurve.Evaluate(time/redMaskAnimLength);
            currentHp = prevHp - amount*scale;
            redMask.padding = new Vector4(0, 0, maxWidth * (1-currentHp/maxHp), 0);

            yield return null;
        }

        updatingBar = false;
    }

    IEnumerator blankMaskAnim(float amount)
    {
        time = 0;

        while (time < blankMaskAnimLength)
        {
            time += Time.deltaTime;

            float scale = blankMaskCurve.Evaluate(time/blankMaskAnimLength);
            currentLagHp = lagHp - amount*scale;
            blankMask.padding = new Vector4(0, 0, maxWidth * (1-currentLagHp/maxHp), 0);

            if (updatingBar)
            {
                break;
            }

            yield return null;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (takeDamageAmout > 0)
        {
            takeDamage(takeDamageAmout);
            takeDamageAmout = 0;
        }

        if (blankMaskAnimFlag)
        {
            animationPauseTime += Time.deltaTime;
        }
        if (blankMaskAnimFlag && animationPauseTime > animationPause+redMaskAnimLength)
        {
            animationPauseTime = 0;
            blankMaskAnimFlag = false;
            StartCoroutine(blankMaskAnim(lagHp-currentHp));
        }
    }
}

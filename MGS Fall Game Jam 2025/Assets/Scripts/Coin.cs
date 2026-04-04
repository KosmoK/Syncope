using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private Vector2 maxVals;
    private float animLength;
    private int amount;
    [SerializeField] AnimationCurve coinAnimCurve; // potentially replace with a function?

    private Vector3 originalLocation;
    private float randomYDeviation;
    private Vector3 maxLocation;
    private float time;
    private bool bouncing = true;

    void Start()
    {
        originalLocation = transform.position;
        randomYDeviation = Random.Range(0, 1f);

        StartCoroutine(BounceCoroutine());
    }

    void Update()
    {
        if (bouncing)
        {
            return;
        }

        transform.position = originalLocation + Vector3.up * 0.2f * (Mathf.Sin(time) + 1);
        time += Time.deltaTime;
    }

    IEnumerator BounceCoroutine()
    {
        time = 0;
        
        while (time < animLength)
        {
            time += Time.deltaTime;
            float xDist = time/animLength * maxVals.x;
            float yDist = coinAnimCurve.Evaluate(time/animLength) * maxVals.y;

            // transform.position = Vector3.Lerp(transform.position, originalLocation + new Vector3(xDist, yDist, 0), Time.deltaTime);
            transform.position = originalLocation + new Vector3(xDist, yDist, 0) + new Vector3(0, time/animLength * randomYDeviation, 0);
            yield return new WaitForEndOfFrame();   
        }

        originalLocation = transform.position;
        bouncing = false;
        time = -Mathf.PI/2;
    }

    public void SetValsAndLenAndAmount(Vector2 maxVals, float animLength, int amount)
    {
        this.maxVals = maxVals;
        this.animLength = animLength;
        this.amount = amount;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerCollider" && !bouncing)
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<CurrencyManager>().addMoney(amount);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.coinGet, this.transform.position);
            Destroy(gameObject);
        }
        if (collision.gameObject.GetComponent<Coin>() != null)
        {
            return;
        }

        if (bouncing)
        {
            maxVals.x = -maxVals.x;
            originalLocation.x = transform.position.x - maxVals.x*time/animLength;   
        }
    }
}

using System.Collections;
using UnityEngine;

public class NeuralImpulseSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject[] prefabs;
    private float duration = 2;
    private float lengthBetween;
    private int spawnNum = 5;
    private Vector2 size = 5 * Vector2.right + 3 * Vector2.up;
    private Vector2 scaleSize = 6 * (5 * Vector2.right + 3 * Vector2.up);
    [SerializeField] AnimationCurve growCurve;
    private float t = 0;
    private float st = 0;
    private int num = 0;
    private bool exitFlag = false;
    void Start()
    {
        transform.localScale = Vector3.zero;
        lengthBetween = duration/(spawnNum+1);
        StartCoroutine(growCoroutine());
    }

    IEnumerator growCoroutine()
    {
        float growTime = duration/5;
        float t = 0;

        while (t < growTime)
        {
            t += Time.deltaTime;
            float tn = t/growTime;
            float s = growCurve.Evaluate(tn);
            transform.localScale = scaleSize * s;

            yield return new WaitForEndOfFrame();
        }

        transform.localScale = scaleSize;
    }

    IEnumerator shrinkCoroutine()
    {
        float growTime = duration/5;
        float t = 0;

        while (t < growTime)
        {
            t += Time.deltaTime;
            float tn = t/growTime;
            float s = growCurve.Evaluate(1-tn);
            transform.localScale = scaleSize * s;

            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (exitFlag)
        {
            return;
        }

        t += Time.deltaTime;
        st += Time.deltaTime;
        if (t > duration)
        {
            StartCoroutine(shrinkCoroutine());
            exitFlag = true;
        }

        if (st > lengthBetween)
        {
            st = 0;
            createAttack();
        }
    }

    private void createAttack()
    {
        float x = Random.value * size.x * ((Random.value > 0.5) ? 1 : -1);
        // x = (num % 2 == 0) ? size.x : -size.x;
        float y = Random.value * size.y * ((Random.value > 0.5) ? 1 : -1);
        // y = (num % 2 == 0) ? size.y : -size.y;
        Vector3 tmpVec = new Vector3(x, y, 0);

        Transform t = Instantiate(prefabs[(int) (Random.value * 3)]).transform;
        t.position = transform.position + tmpVec;
        t.localScale = new Vector3((num % 2 == 0) ? 1 : -1, 1, 1);
        num++;
    }

    public void setSpawnNum(int sn)
    {
        if (sn < 0)
        {
            Debug.LogError("Invalid neural impulse spawner spawn num: {sn}");
            return;
        }

        spawnNum = sn;
    }

    public void setDuration(float d)
    {
        if (d < 0)
        {
            Debug.LogError("Invalid neural impulse spawner duration: {d}");
            return;
        }

        duration = d;
    }

    public void setSize(Vector2 s)
    {
        if (s.x < 0 || s.y < 0)
        {
            Debug.LogError("Invalid neural impulse size: {s}");
            return;
        }

        size = s;
        scaleSize = size * 6;
    }
}

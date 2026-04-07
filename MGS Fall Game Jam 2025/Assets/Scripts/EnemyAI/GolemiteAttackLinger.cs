using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class GolemiteAttackLinger : MonoBehaviour
{
    [SerializeField] AnimationClip animationClip;
    float t;
    bool destroying;
    private int damage;
    void Start()
    {
        transform.GetChild(0).gameObject.GetComponent<Animator>().Play(animationClip.name);
    }
    void Update()
    {
        t -= Time.deltaTime;
        if (t < 0 && !destroying)
        {
            StartCoroutine(DestroyCoroutine());
        }
    }

    public void setLingerDuration(float d)
    {
        t = d;
    }

    public void setDamage(int d)
    {
        damage = d;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gameObject = collision.gameObject;

        if (gameObject.tag == "Player" && !destroying)
        {
            gameObject.GetComponent<EntityStatus>().Hurt(damage);
            StartCoroutine(DestroyCoroutine());
        } else
        {
            return;
        }
    }

    IEnumerator DestroyCoroutine()
    {
        destroying = true;

        float t = 0;
        float d = 0.5f;

        float origScale = transform.localScale.x;
        float maxScale = origScale * 1.5f;
        float diffScale = maxScale - origScale;

        SpriteRenderer sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        AnimationCurve ac = AnimationCurve.EaseInOut(0, 0, 1, 1);

        while (t < d)
        {
            t += Time.deltaTime;
            float n = t/d;

            float scaleNum = ac.Evaluate(n) * diffScale + origScale;
            transform.localScale = new Vector3(scaleNum, scaleNum, scaleNum);

            sr.color = new Vector4(1, 1, 1, ac.Evaluate(1-n));
            
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }
}

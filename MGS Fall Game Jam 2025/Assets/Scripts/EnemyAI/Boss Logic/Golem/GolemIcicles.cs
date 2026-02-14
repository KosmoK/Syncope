using System;
using UnityEngine;
using UnityEngine.Rendering;

public class GolemIcicles : MonoBehaviour
{
    [SerializeField] AnimationClip icicleAnim1;
    [SerializeField] AnimationClip icicleAnim2;
    Animator animator;
    private int icicleAnim;
    private float growDuration;
    private float timeAlive;
    private float duration = 2f;
    private bool die = false;
    private float scale = 4.2f;
    private float scaleFactor = 1f;
    private int layer;
    void Start()
    {
        animator = GetComponent<Animator>();
        icicleAnim = UnityEngine.Random.Range(0, 2);
        if (icicleAnim == 1)
        {
            animator.Play(icicleAnim1.name);
        } else
        {
            animator.Play(icicleAnim2.name);
        }
        scale *= scaleFactor;

        transform.localScale = Vector3.zero;

        GetComponent<SortingGroup>().sortingOrder = layer;
    }


    void Update()
    {
        timeAlive += Time.deltaTime;

        if (timeAlive < growDuration)
        {
            setSize();   
        }

        if (timeAlive > duration)
        {
            die = true;
            setSize();

            if (transform.localScale == Vector3.zero)
            {
                Destroy(gameObject);
            }
        }
    }

    private void setSize()
    {
        float norm = (!die) ? timeAlive/growDuration : (timeAlive-duration)/growDuration;
        norm = Math.Min(1, Math.Max(0, norm)); // clamp in case of rounding errors
        float e = (!die) ? easer(norm) : easer(1 - norm);
        float size = scale * e;

        transform.localScale = new Vector3(size, size, 0);
    }

    // source: https://easings.net/#easeInOutCirc
    private float easer(float x)
    {
        return x < 0.5f ? (1 - (float) Math.Sqrt(1 - (float) Math.Pow(2 * x, 2))) / 2 : (float) (Math.Sqrt(1 - (float) Math.Pow(-2 * x + 2, 2)) + 1) / 2;
    }

    public void setDuration(float d)
    {
        duration = d + growDuration;
    }

    public void setGrowDuration(float d)
    {
        growDuration = d;
    }

    public void setScaleFactor(float scaleFactor)
    {
        this.scaleFactor = scaleFactor;
    }

    public void setRenderLayer(int layer)
    {
        this.layer = layer;
    }
}

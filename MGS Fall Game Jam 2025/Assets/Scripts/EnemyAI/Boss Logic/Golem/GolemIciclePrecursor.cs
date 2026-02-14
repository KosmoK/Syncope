using System;
using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class GolemIciclePrecursor : MonoBehaviour
{
    [SerializeField] GameObject iciclePrefab;
    private float xScale = 1.245f;
    private float yScale = 0.645f;
    [SerializeField] float growDuration = 0.5f;
    [SerializeField] float spawnIcicleTime = 0f;
    [SerializeField] float icicleDuration;
    [SerializeField] float icicleGrowDuration;
    private float scaleFactor = 1f;
    private float timeAlive;
    private float duration;
    private bool spawnedFlag = false;
    private bool die = false;
    private int layer = 0;

    void Start()
    {
        timeAlive = 0;
        xScale *= scaleFactor;
        yScale *= scaleFactor;
        spawnIcicleTime += growDuration;
        duration = spawnIcicleTime + icicleDuration + icicleGrowDuration/2;
        transform.localScale = Vector3.zero;

        // GetComponent<SortingGroup>().sortingOrder = layer;
    }

    // Update is called once per frame
    void Update()
    {
        timeAlive += Time.deltaTime;
        
        if (timeAlive < growDuration)
        {
            setSize();   
        }

        if (!spawnedFlag && timeAlive > spawnIcicleTime)
        {
            spawnedFlag = true;

            Vector3 diff = new Vector3(0, -0.1f, 0);
            GameObject icicle = Instantiate(iciclePrefab, transform.position + diff, Quaternion.identity);
            GolemIcicles gi = icicle.GetComponent<GolemIcicles>();
            gi.setDuration(icicleDuration);
            gi.setGrowDuration(icicleGrowDuration);
            gi.setScaleFactor(scaleFactor);
            gi.setRenderLayer(layer+1);
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
        
        float x = xScale * e;
        float y = yScale * e;

        transform.localScale = new Vector3(x, y, 0);
    }

    // source: https://easings.net/#easeOutBack
    private float easer(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return 1 + c3 * (float) Math.Pow(x - 1f, 3) + c1 * (float) Math.Pow(x - 1, 2);
    }

    // Getters and setters

    public void setGrowDuration(float growDuration)
    {
        this.growDuration = growDuration;
    }

    public void setSpawnIcicleTime(float time)
    {
        spawnIcicleTime = time;
    }

    public void setScaleFactor(float scaleFactor)
    {
        this.scaleFactor = scaleFactor;
    }

    public void setRenderLayer(int layer)
    {
        this.layer = layer;
    }
    public void setIcicleDuration(float icicleDuration)
    {
        this.icicleDuration = icicleDuration;
    }
    public float getDuration()
    {
        return spawnIcicleTime + icicleDuration + icicleGrowDuration/2;
    }
}

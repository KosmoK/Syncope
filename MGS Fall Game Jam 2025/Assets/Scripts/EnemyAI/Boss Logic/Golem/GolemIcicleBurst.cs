using System;
using UnityEngine;

public class GolemIcicleBurst : MonoBehaviour
{
    GameObject player;
    [SerializeField] GameObject iciclePrefab;
    private float icicleCooldown;
    private float icicleBurstCooldown;
    private float icicleBurstIndCooldown;
    private float icicleBurstTime;
    private Vector3 icicleBurstVector;
    private int icicleNum;
    private int icicleLayerMask;
    float burstDuration = 1f;
    float maxBurstOffset = 2f;
    float maxScale = 4.5f;
    float timeBetweenIcicles = 0.05f;
    float length = 45f;
    bool grow = true;

    public void setBurstDuration(float burstDuration)
    {
        this.burstDuration = burstDuration;
    }
    public void setMaxBurstOffset(float maxBurstOffset)
    {
        this.maxBurstOffset = maxBurstOffset;
    }
    public void setMaxScale(float maxScale)
    {
        this.maxScale = maxScale;
    }
    public void setGrow(bool grow)
    {
        this.grow = grow;
    }
    public void setTimeBetweenIcicles(float c)
    {
        timeBetweenIcicles = c;
    }
    public void setLength(float length)
    {
        this.length = length;
    }
    public void setDirectionVector(Vector3 vec)
    {
        icicleBurstVector = vec;
    }

    void Start()
    {
        icicleBurstTime = 0;
        icicleBurstIndCooldown = 0;
        icicleNum = 0;
        icicleLayerMask = LayerMask.GetMask("IceBossPong");

        // icicleBurstVector = player.transform.position - transform.position + Vector3.down;
        icicleBurstVector = icicleBurstVector.normalized * length;
    }

    // Update is called once per frame
    void Update()
    {
        float t = Math.Max(0, Math.Min(1, icicleBurstTime/burstDuration));
        Vector3 position = icicleBurstVector.normalized * icicleBurstVector.magnitude * t;
        Vector3 offsetVector = new Vector3(-icicleBurstVector.normalized.y, icicleBurstVector.normalized.x, 0) * t * maxBurstOffset * UnityEngine.Random.value;
        offsetVector = (UnityEngine.Random.Range(0, 2) == 0) ? offsetVector : -offsetVector;
        
        position += transform.position + offsetVector;

        if (icicleBurstIndCooldown <= 0)
        {
            GameObject g = Instantiate(iciclePrefab, position, Quaternion.identity);
            GolemIciclePrecursor gi = g.GetComponent<GolemIciclePrecursor>();
            gi.setScaleFactor(maxScale * ((grow) ? t : 1-t));
            gi.setRenderLayer(icicleNum);
            // gi.setSpawnIcicleTime(0);
            icicleBurstIndCooldown = timeBetweenIcicles;

            icicleNum++;
        }

        Vector2 myPos = new Vector2(transform.position.x, transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(myPos, position - transform.position, (position-transform.position).magnitude, icicleLayerMask);
        if (icicleBurstTime >= burstDuration || hit)
        {
            Destroy(gameObject);
        }

        icicleBurstTime += Time.deltaTime;
        icicleBurstIndCooldown -= Time.deltaTime;
    }
}

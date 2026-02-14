using System;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEditor.Embree;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GolemIcicleBloom : MonoBehaviour
{
    [SerializeField] GameObject icicleBurstPrefab;
    [SerializeField] GameObject precursorPrefab;
    float timeAlive = 0;
    float spawnBurstTime = 2f;
    float initScale = 4f;
    float burstScale = 2f;
    float burstRadius = 5f;
    Vector3 directionVec = Vector3.right;
    int burstNum = 5;
    Matrix2D rotMatrix;

    void Start()
    {
        GameObject initIcicle = Instantiate(precursorPrefab, transform.position, Quaternion.identity);
        GolemIciclePrecursor gi = initIcicle.GetComponent<GolemIciclePrecursor>();
        gi.setScaleFactor(initScale);
        gi.setGrowDuration(1f);

        if (rotMatrix == Matrix2D.zero)
        {
            constructRotMatrix();
        }
    }
    public void setDirectionVec(Vector3 vec)
    {
        directionVec = vec;
    }
    public void constructRotMatrix()
    {
        rotMatrix = new Matrix2D();
        float angle = ((float) Math.PI * 2f)/burstNum;
        rotMatrix.m00 = (float) Math.Cos(angle);
        rotMatrix.m01 = (float) -Math.Sin(angle);
        rotMatrix.m10 = -rotMatrix.m01;
        rotMatrix.m11 = rotMatrix.m00;

        rotMatrix.m02 = 1;
        rotMatrix.m12 = 1;
    }

    void Update()
    {
        if (timeAlive <= spawnBurstTime)
        {
            timeAlive += Time.deltaTime;
            return;
        }

        Debug.Log("working");
        for (int i = 0; i < burstNum; i++)
        {
            GameObject g = Instantiate(icicleBurstPrefab, transform.position + directionVec.normalized * burstRadius, Quaternion.identity);
            GolemIcicleBurst gb = g.GetComponent<GolemIcicleBurst>();
            gb.setMaxScale(burstScale);
            gb.setGrow(false);
            gb.setLength(5f);
            gb.setDirectionVector(directionVec);

            Debug.Log(directionVec);
            directionVec = rotMatrix.MultiplyVector(directionVec);
        }

        Destroy(gameObject);
    }
}

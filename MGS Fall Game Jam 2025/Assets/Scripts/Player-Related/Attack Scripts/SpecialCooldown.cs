using UnityEngine;

public class SpecialCooldown : MonoBehaviour
{
    float t = 0;

    // Update is called once per frame
    void Update()
    {
        t -= Time.deltaTime;
        if (t < 0)
        {
            Destroy(this);
        }
    }

    public void setT(float time)
    {
        if (t == 0)
        {
            t = time;
        }
    }
}

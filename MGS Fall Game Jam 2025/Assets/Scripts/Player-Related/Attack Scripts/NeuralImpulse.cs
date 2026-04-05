using UnityEngine;

public class NeuralImpulse : MonoBehaviour
{
    [SerializeField] GameObject spawnerPrefab;
    [SerializeField] int spawnNum;
    [SerializeField] float duration;
    [SerializeField] Vector2 size;

    void OnDestroy()
    {
        GameObject spawner = Instantiate(spawnerPrefab);
        spawner.transform.position = transform.position;
        NeuralImpulseSpawner nis = spawner.GetComponent<NeuralImpulseSpawner>();
        nis.setSpawnNum(spawnNum);
        nis.setDuration(duration);
        nis.setSize(size);
    }

    public void setSLS(int sn, float d, Vector2 s)
    {
        spawnNum = sn;
        duration = d;
        size = s;
    }
}

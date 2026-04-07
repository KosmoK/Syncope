using UnityEngine;

public class NeuralImpulse : MonoBehaviour
{
    [SerializeField] GameObject spawnerPrefab;
    [SerializeField] int spawnNum;
    [SerializeField] float duration;
    [SerializeField] Vector2 size;

    void Start()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Movement>().setDontMove(true);
        AudioManager.instance.PlayOneShot(FMODEvents.instance.neuralImpulse, this.transform.position);
    }

    void OnDestroy()
    {
        GameObject spawner = Instantiate(spawnerPrefab);
        spawner.transform.position = transform.position;
        NeuralImpulseSpawner nis = spawner.GetComponent<NeuralImpulseSpawner>();
        nis.setSpawnNum(spawnNum);
        nis.setDuration(duration);
        nis.setSize(size);

        GameObject.FindGameObjectWithTag("Player").GetComponent<Movement>().setDontMove(false);
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlayerCamera>().ShakeCamera(4, 4, 1);
    }

    public void setSLS(int sn, float d, Vector2 s)
    {
        spawnNum = sn;
        duration = d;
        size = s;
    }
}

using UnityEngine;

public class GolemiteAttack : MonoBehaviour
{
    [SerializeField] GameObject lingerPrefab;
    [SerializeField] float lingerDuration;

    void OnDestroy()
    {
        GolemiteAttackLinger gal = Instantiate(lingerPrefab).GetComponent<GolemiteAttackLinger>();
        gal.transform.position = transform.position;
        gal.setLingerDuration(lingerDuration);
    }
}

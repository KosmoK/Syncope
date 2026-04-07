using UnityEngine;

public class GiveSpecialCooldown : MonoBehaviour
{
    [SerializeField] float cooldownDuration;
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        SpecialCooldown sc = player.AddComponent<SpecialCooldown>();
        sc.setT(cooldownDuration);
    }
}

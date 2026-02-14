using UnityEngine;
using UnityEngine.InputSystem;

public class DamageTestScript : MonoBehaviour
{
    private InputAction inp;
    [SerializeField] GameObject enemy;
    EnemyAnimStates eas;
    void Start()
    {
        inp = InputSystem.actions.FindAction("Damage");
        eas = enemy.GetComponent<EnemyAnimStates>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inp.WasPressedThisFrame())
        {
            eas.dealDamage(1);
        }
    }
}

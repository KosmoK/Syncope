using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference playerFootsteps { get; private set; }

    [field: SerializeField] public EventReference playerHurt { get; private set; }
    [field: SerializeField] public EventReference playerAttack { get; private set; }

    [field: Header("Ant SFX")]
    [field: SerializeField] public EventReference antAttack {get; private set;}
    [field: SerializeField] public EventReference antLeaderAttack { get; private set; }
    [field: SerializeField] public EventReference antAttackNew { get; private set; }
    [field: SerializeField] public EventReference antDeath { get; private set; }

    [field: Header("Generic Enemy SFX")]
    [field: SerializeField] public EventReference enemyGenericHit { get; private set; }

    [field: Header("Misc SFX")]
    [field: SerializeField] public EventReference coinGet { get; private set; }
    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null) {
            Debug.LogError("Found more than one FMOD Events instance in the scene.");
        }
        instance = this;
    }
}

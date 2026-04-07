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
    [field: SerializeField] public EventReference gravBall { get; private set; }
    [field: SerializeField] public EventReference icyBreath { get; private set; }
    [field: SerializeField] public EventReference neuralImpulse { get; private set; }
    [field: SerializeField] public EventReference sketchTornado { get; private set; }
    [field: Header("Ant SFX")]
    [field: SerializeField] public EventReference antAttack {get; private set;}
    [field: SerializeField] public EventReference antLeaderAttack { get; private set; }
    [field: SerializeField] public EventReference antAttackNew { get; private set; }
    [field: SerializeField] public EventReference antDeath { get; private set; }

    [field: Header("Snake SFX")]
    [field: SerializeField] public EventReference snakeAttack { get; private set; }
    [field: SerializeField] public EventReference snakeDeath { get; private set; }

    [field: Header("Frog SFX")]
    [field: SerializeField] public EventReference frogDeath { get; private set; }
    [field: SerializeField] public EventReference frogJump { get; private set; }
    [field: SerializeField] public EventReference frogAttack { get; private set; }
    [field: Header("Generic Enemy SFX")]
    [field: SerializeField] public EventReference enemyGenericHit { get; private set; }
    [field: Header("Phoenix SFX")]
    [field: SerializeField] public EventReference flapWing { get; private set; }
    [field: SerializeField] public EventReference slashLoop { get; private set; }
    [field: SerializeField] public EventReference phoenixDeath { get; private set; }
    [field: Header("Sharmadillo SFX")]
    [field: SerializeField] public EventReference sharmadilloWalk { get; private set; }
    [field: SerializeField] public EventReference sharmadilloAttack { get; private set; }
    [field: SerializeField] public EventReference sharmadilloDeath { get; private set; }

    [field: Header("Snole SFX")]
    [field: SerializeField] public EventReference snoleRustle { get; private set; }
    [field: SerializeField] public EventReference snoleAttack { get; private set; }
    [field: SerializeField] public EventReference snoleDeath { get; private set; }

    [field: Header("Golemite SFX")]
    [field: SerializeField] public EventReference golemiteWalk { get; private set; }
    [field: SerializeField] public EventReference golemiteAttack { get; private set; }
    [field: SerializeField] public EventReference golemiteDeath { get; private set; }

    [field: Header("Ice Boss SFX")]
    [field: SerializeField] public EventReference iceBossAwaken { get; private set; }
    [field: SerializeField] public EventReference iceBossAttack { get; private set; }
    [field: SerializeField] public EventReference iceBossDeath { get; private set; }

    [field: Header("Final Boss SFX")]
    [field: SerializeField] public EventReference finalBossStart { get; private set; }
    [field: SerializeField] public EventReference finalBossScream { get; private set; }
    [field: SerializeField] public EventReference finalBossDeath { get; private set; }
    [field: SerializeField] public EventReference finalBossWalk { get; private set; }
    [field: SerializeField] public EventReference finalBossDistortion { get; private set; }
    [field: SerializeField] public EventReference finalBossSlash { get; private set; }

    [field: Header("Misc SFX")]
    [field: SerializeField] public EventReference coinGet { get; private set; }
    [field: SerializeField] public EventReference purchase { get; private set; }
    [field: SerializeField] public EventReference itemSelected { get; private set; }
    [field: SerializeField] public EventReference itemUnaffordable { get; private set; }
    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null) {
            Debug.LogError("Found more than one FMOD Events instance in the scene.");
        }
        instance = this;
    }
}

using System;
using UnityEngine;

public class HazardDamager : MonoBehaviour
{
    [SerializeField] public float knockBackStrength = 0f;
    [SerializeField] private int damage = 1;
    [SerializeField] private StatusEffects applyEffect;
    [SerializeField] private float coefficentOfFriction = 7.5f;
    [SerializeField] public bool doesDamage = true;
    [SerializeField] public bool isPhoenix = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (!(collision.gameObject.tag == "Player"))
        {
            return;
        }
        EntityStatus entity = collision.gameObject.GetComponent<EntityStatus>();
        Movement entityMovement = collision.gameObject.GetComponent<Movement>();
        entityMovement.coefficentOfFriction = coefficentOfFriction;
        if (doesDamage)
        {
            applyHazardDamage(entity, entityMovement);   
        }
    }
    // void OnCollisionStay2D(Collision2D collision)
    // {
    //     EntityStatus entity = collision.gameObject.GetComponent<EntityStatus>();
    //     Movement entityMovement = collision.gameObject.GetComponent<Movement>();
    //     applyHazardDamage(entity, entityMovement);
    // }
    public void applyHazardDamage(EntityStatus entity, Movement entityMovement)
    {
        if (!entity.IsStunned())
        {
            entity.Hurt(damage);
            if(knockBackStrength != 0)
            {
                Vector2 normMovement = entityMovement.velocity.normalized;
                if (isPhoenix && normMovement.y > 0)
                {
                    normMovement.y *= -1;
                }
                entityMovement.velocity = -knockBackStrength * normMovement;
            }
        }
        if(applyEffect != null){entity.ApplyStatus(applyEffect);}
    }
}

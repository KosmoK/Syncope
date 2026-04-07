using System;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    private int attack1Amount;
    private int attack2Amount;
    private int maxHpAmount;
    private float speedAmount;

    private int[] attackCosts = {10, 20, 30, 40};
    private int[] attack1Amounts = {1, 2, 3, 4};
    private int[] attack2Amounts = {2, 4, 5, 6};
    private int[] maxHpCosts = {10, 20, 30, 40};
    private int[] maxHpAmounts = {10, 20, 30, 40};
    private int[] speedCosts = {10, 20, 30, 40};
    private int[] speedAmounts = {10, 20, 30, 40};
    
    private Movement movement;
    private PlayerMovement playerMovement;
    private EntityStatus entityStatus;
    private GameObject player;

    void Start()
    {
        attack1Amount = attack1Amounts[0];
        attack2Amount = attack2Amounts[0];
        setLinks();
    }

    void Update()
    {
        if (player == null)
        {
            setLinks();
        }
    }

    private void setLinks()
    {
        try
        {
            player = GameObject.FindGameObjectWithTag("Player");
        } catch
        {
            return;
        }
        
        movement = player.GetComponent<Movement>();
        playerMovement = player.GetComponent<PlayerMovement>();
        entityStatus = player.GetComponent<EntityStatus>();
    }
    
    public int getNewCost(string item, int currentAmount)
    {
        if (item == "attack")
        {
            return findNewCost(attackCosts, currentAmount);
        } else if (item == "hp")
        {
            return findNewCost(maxHpCosts, currentAmount);
        } else if (item == "speed")
        {
            return findNewCost(speedCosts, currentAmount);
        }

        return -1;
    }

    private int findNewCost(int[] array, int curr)
    {
        for (int i = 0; i < array.Length-1; i++)
        {
            if (array[i] == curr)
            {
                return array[i+1];
            }
        }

        return -1;
    }

    public void setVal(string item, int currentCost)
    {
        int i;
        if (item == "attack")
        {
            i = Array.IndexOf(attackCosts, currentCost);
            if (i == -1)
            {
                return;
            }

            attack1Amount = attack1Amounts[i];
            attack2Amount = attack2Amounts[i];
        } else if (item == "hp")
        {
            i = Array.IndexOf(maxHpCosts, currentCost);
            if (i == -1)
            {
                return;
            }

            entityStatus.health = maxHpAmounts[i];
        } else if (item == "speed")
        {
            i = Array.IndexOf(speedCosts, currentCost);
            if (i == -1)
            {
                return;
            }

            movement.topSpeed = speedAmounts[i];
        }
    }

    public int getDamage1()
    {
        return attack1Amount;
    }
    public int getDamage2()
    {
        return attack2Amount;
    }
    public void setInitCosts(out int atk, out int hp, out int speed)
    {
        atk = attackCosts[0];
        hp = maxHpCosts[0];
        speed = speedCosts[0];
    }

}

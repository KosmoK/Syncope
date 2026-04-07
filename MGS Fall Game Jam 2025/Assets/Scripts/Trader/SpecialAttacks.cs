using System.Collections.Generic;
using UnityEngine;

public class SpecialAttacks : MonoBehaviour
{
    Dictionary<string, bool> unlocks = new Dictionary<string, bool>
    {
        {"Neural Impulse", false},
        {"Gravity Ball", false},
        {"Sketch Tornado", false},
        {"Icy Breath", false},
    };

    Dictionary<string, int> costs = new Dictionary<string, int>
    {
        {"Neural Impulse", 10},
        {"Gravity Ball", 10},
        {"Sketch Tornado", 10},
        {"Icy Breath", 10},
    };

    Dictionary<string, int> playerMap = new Dictionary<string, int>
    {
        {"Neural Impulse", 2},
        {"Gravity Ball", 1},
        {"Sketch Tornado", 0},
        {"Icy Breath", 3},
    };

    CurrencyManager currencyManager;

    void Start()
    {
        currencyManager = GetComponent<CurrencyManager>();
    }

    public bool purchasingAttack(string attack)
    {
        if (!unlocks[attack] && currencyManager.deductMoney(costs[attack]))
        {
            unlocks[attack] = true;
            AudioManager.instance.PlayOneShot(FMODEvents.instance.purchase, this.transform.position);
        } else if (!unlocks[attack])
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.itemUnaffordable, this.transform.position);
            return false;
        }

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().setCurrExtra(playerMap[attack]);

        return true;
    }

    public Dictionary<string, bool> getUnlocks()
    {
        return unlocks;
    }
    public Dictionary<string, int> getCosts()
    {
        return costs;
    }
}

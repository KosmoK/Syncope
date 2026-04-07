using System.Collections.Generic;
using UnityEngine;

public class Drugs : MonoBehaviour
{
    Dictionary<string, bool> unlocks = new Dictionary<string, bool>
    {
        {"Fire", false},
        {"Ice", false},
        {"Dash", false},
        {"Speed", false},
    };

    Dictionary<string, int> costs = new Dictionary<string, int>
    {
        {"Fire", 150},
        {"Ice", 150},
        {"Dash", 150},
        {"Speed", 150},
    };

    CurrencyManager currencyManager;
    DrugManager drugManager;

    void Start()
    {
        currencyManager = GetComponent<CurrencyManager>();
        drugManager = GetComponent<DrugManager>();
    }

    public bool purchasingDrug(string drug)
    {
        if (!unlocks[drug] && currencyManager.deductMoney(costs[drug]))
        {
            unlocks[drug] = true;
        } else if (!unlocks[drug])
        {
            return false;
        }

        drugManager.setActiveDrug(drug);

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

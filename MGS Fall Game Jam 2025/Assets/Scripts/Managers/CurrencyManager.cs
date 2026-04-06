using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] int currency = 0;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addMoney(int amount)
    {
        currency += amount;
    }

    public int getMoney()
    {
        return currency;
    }

    public bool deductMoney(int amount)
    {
        if (currency-amount < 0)
        {
            return false;
        } else
        {
            currency -= amount;
            return true;
        }
    }
}

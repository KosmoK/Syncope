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
}

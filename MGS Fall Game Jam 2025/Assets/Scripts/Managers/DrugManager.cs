using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrugManager : MonoBehaviour
{
    Dictionary<string, float> drugDurations = new Dictionary<string, float>
    {
        {"Fire", 10},
        {"Ice", 10},
        {"Dash", 10},
        {"Speed", 10}
    };

    private string activeDrug;
    private string usedDrug;
    private InputAction drugAction;
    private PlayerMovement playerMovement;
    private Movement movement;
    private bool canUseDrug = true;
    private int bonusLavaDamage;
    private int bonusIceDamage;

    void Start()
    {
        drugAction = InputSystem.actions.FindAction("Use Drug");
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        movement = GameObject.FindGameObjectWithTag("Player").GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canUseDrug && drugAction.WasPressedThisFrame())
        {
            useDrug();
        }
    }

    public void setActiveDrug(string drug)
    {
        if (drug == "Fire")
        {
            Debug.Log("set drug: fire");
        } else if (drug == "Ice")
        {
            Debug.Log("set drug: ice");
        } else if (drug == "Dash")
        {
            Debug.Log("set drug: dash");
        } else if (drug == "Speed")
        {
            Debug.Log("set drug: speed");
        }

        activeDrug = drug;
    }

    private void unsetActiveDrug()
    {
        if (activeDrug == "Fire")
        {
            Debug.Log("unset drug: fire");
        } else if (activeDrug == "Ice")
        {
            Debug.Log("unset drug: ice");
        } else if (activeDrug == "Dash")
        {
            Debug.Log("unset drug: dash");
        } else if (activeDrug == "Speed")
        {
            Debug.Log("unset drug: speed");
        }
    }

    private void useDrug()
    {
        canUseDrug = false;
        usedDrug = activeDrug;

        if (activeDrug == "Fire")
        {
            bonusLavaDamage = 10;
        } else if (activeDrug == "Ice")
        {
            bonusIceDamage = 10;
        } else if (activeDrug == "Dash")
        {
            playerMovement.dashStrength = 0.1f;
        } else if (activeDrug == "Speed")
        {
            movement.topSpeed += 20;
        }

        StartCoroutine(waitForSecs(drugDurations[activeDrug]));
    }
    private void stopUseDrug()
    {
        if (usedDrug == "Fire")
        {
            bonusLavaDamage = 0;
        } else if (usedDrug == "Ice")
        {
            bonusIceDamage = 0;
        } else if (usedDrug == "Dash")
        {
            playerMovement.dashStrength = 0.03f;
        } else if (usedDrug == "Speed")
        {
            movement.topSpeed -= 20;
        }

        canUseDrug = true;
    }

    IEnumerator waitForSecs(float time)
    {
        yield return new WaitForSeconds(time);
        stopUseDrug();
    }

    public int getBonusLavaDamage()
    {
        return bonusLavaDamage;
    }
    public int getBonusIceDamage()
    {
        return bonusIceDamage;
    }
}

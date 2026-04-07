using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TraderScript : MonoBehaviour
{
    [SerializeField] Trader trader;
    [SerializeField] Sprite notPurchasedSprite;
    [SerializeField] Sprite purchasedSprite;
    [SerializeField] Sprite activeSprite;
    [Header("References to all the slots")]
    [SerializeField] Image neuralImpulse;
    [SerializeField] Image sketchKick;
    [SerializeField] Image gravityBall;
    [SerializeField] Image icyBreath;
    Dictionary<string, Image> attackReferences;

    [SerializeField] Image fireDrug;
    [SerializeField] Image iceDrug;
    [SerializeField] Image dashDrug;
    [SerializeField] Image speedDrug;
    Dictionary<string, Image> drugReferences;


    private bool canTrade = false;
    private bool trading = false;
    private InputAction interactAction;
    private GameObject gameManager;
    private SpecialAttacks specialAttacks;
    private Drugs drugs;
    private DrugManager drugManager;
    private Movement playerMovement;
    void Start()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<Movement>();

        specialAttacks = gameManager.GetComponent<SpecialAttacks>();
        drugs = gameManager.GetComponent<Drugs>();
        drugManager = gameManager.GetComponent<DrugManager>();

        attackReferences = new Dictionary<string, Image>
        {
            {"Neural Impulse", neuralImpulse},
            {"Gravity Ball", gravityBall},
            {"Sketch Tornado", sketchKick},
            {"Icy Breath", icyBreath},
        };

        drugReferences = new Dictionary<string, Image>
        {
            {"Fire", fireDrug},
            {"Ice", iceDrug},
            {"Dash", dashDrug},
            {"Speed", speedDrug},
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (canTrade && interactAction.WasPressedThisFrame())
        {
            if (trading)
            {
                endTrade();
            } else
            {
                startTrade();
            }
        }
    }

    void startTrade()
    {
        
        trader.setFunds();
        trader.activateUI();
        
        playerMovement.setDontMove(true);
        trading = true;
    }

    void endTrade()
    {
        trader.deactivateUI();

        playerMovement.setDontMove(false);
        trading = false;
    }

    // All functions having to do with buying stuff
    public void purchaseAttackUpgrade()
    {
        
    }

    public void purchaseHPUpgrade()
    {
        
    }

    public void purchaseSpeedUpgrade()
    {
        
    }

    public void purchaseNI()
    {
        if (specialAttacks.purchasingAttack("Neural Impulse"))
        {
            setActiveAttack("Neural Impulse");
            
        }
        trader.setFunds();
        
    }

    public void purchaseST()
    {
        if (specialAttacks.purchasingAttack("Sketch Tornado"))
        {
            setActiveAttack("Sketch Tornado");
            
        }
        trader.setFunds();
        
    }

    public void purchaseGB()
    {
        if (specialAttacks.purchasingAttack("Gravity Ball"))
        {
            setActiveAttack("Gravity Ball");
            
        }
        trader.setFunds();
        
    }

    public void purchaseIB()
    {
        if (specialAttacks.purchasingAttack("Icy Breath"))
        {
            setActiveAttack("Icy Breath");
            
        }
        trader.setFunds();
        
    }

    public void purchaseFire()
    {
        if (drugs.purchasingDrug("Fire"))
        {
            drugManager.setActiveDrug("Fire");
            setActiveDrug("Fire");
           
        }
        trader.setFunds();
        
    }

    public void purchaseIce()
    {
        if (drugs.purchasingDrug("Ice"))
        {
            drugManager.setActiveDrug("Ice");
            setActiveDrug("Ice");
            
        }
        trader.setFunds();
        
    }

    public void purchaseDash()
    {
        if (drugs.purchasingDrug("Dash"))
        {
            drugManager.setActiveDrug("Dash");
            setActiveDrug("Dash");
            
        }
        trader.setFunds();
        
    }

    public void purchaseMaxSpeed()
    {
        if (drugs.purchasingDrug("Speed"))
        {
            drugManager.setActiveDrug("Speed");
            setActiveDrug("Speed");
            
        }
        trader.setFunds();
        
    }

    private void setActiveAttack(string attackName)
    {
        Dictionary<string, bool> unlocks = specialAttacks.getUnlocks();
        foreach (string s in unlocks.Keys)
        {
            if (s == attackName)
            {
                attackReferences[s].sprite = activeSprite;
                AudioManager.instance.PlayOneShot(FMODEvents.instance.itemSelected, this.transform.position);
            } else if (unlocks[s] == true)
            {
                attackReferences[s].sprite = purchasedSprite;
            } else
            {
                attackReferences[s].sprite = notPurchasedSprite;

            }
        }

    }

    private void setActiveDrug(string drugName)
    {
        Dictionary<string, bool> unlocks = drugs.getUnlocks();
        foreach (string s in unlocks.Keys)
        {
            if (s == drugName)
            {
                drugReferences[s].sprite = activeSprite;
                AudioManager.instance.PlayOneShot(FMODEvents.instance.itemSelected, this.transform.position);
            } else if (unlocks[s] == true)
            {
                drugReferences[s].sprite = purchasedSprite;
            } else
            {
                drugReferences[s].sprite = notPurchasedSprite;
                
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        GameObject gameObject = collision.gameObject;
        if (gameObject.tag != "Player")
        {
            return;
        }

        canTrade = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        GameObject gameObject = collision.gameObject;
        if (gameObject.tag != "Player")
        {
            return;
        }

        canTrade = false;
    }
}

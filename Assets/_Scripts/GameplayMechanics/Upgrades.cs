using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class Upgrades : MonoBehaviour
{
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject distortionEffect;

    [SerializeField] private HandView handView;

    [SerializeField] private UpgradesUI upgradesUI;

    private int upgradePoints = 0;

    private int baseregen = 0; 
    private float baseDamage = 0;
    private int baseUtility = 0;
    private float baseDefense = 0;

    public int UpgradePoints { get => upgradePoints; set => upgradePoints = value; }
    public int Baseregen { get => baseregen; set => baseregen = value; }
    public float BaseDamage { get => baseDamage; set => baseDamage = value; }
    public int BaseUtility { get => baseUtility; set => baseUtility = value; }
    public float BaseDefense { get => baseDefense; set => baseDefense = value; }

    [SerializeField] private Player player;


    public void ToggleUpgrades()
    {
        if (upgradePanel.activeSelf)
        {
            upgradePanel.SetActive(false);
            distortionEffect.SetActive(false);
        }
        else if(upgradePanel.activeSelf == false)
        {
            upgradePanel.SetActive(true);
            distortionEffect.SetActive(true);
        }
        upgradesUI.updateUpgradesUI();
        return;
    }

    public void UpgradeRegen()
    {
        if(baseregen >= 15)
        {
            return;
        }
        if(upgradePoints <= 0)
        {
            return;
        }
        upgradePoints -= 1;
        if(handView.RoomCounter < 5)
        {
            baseregen += 3;
        }
        else if(handView.RoomCounter < 10)
        {
            baseregen += 2;
        }
        else
        {
            baseregen += 1;
        }
        upgradesUI.updateUpgradesUI();
    }

    public void ApplyroomRegen()
    {
        if(player == null)
        {
            return;
        }
        if(player.CurrentPlayerHealth <= 0)
        {
            return;
        }
        player.regenerate(baseregen);
    }

    public void UpgradeDamage(){ // Same as defense, probably will increase a multiplier or something
        if(baseDamage >= 10)
        {
            return;
        }
        if(upgradePoints <= 0)
        {
            return;
        }
        upgradePoints -= 1;
        baseDamage += 0.5f;
        upgradesUI.updateUpgradesUI();
    }

    public void UpgradeUtility()
    { // Basically max mana
        if(baseUtility >= 3)
        {
            return;
        }
        if(upgradePoints <= 0)
        {
            return;
        }
        upgradePoints -= 1;
        baseUtility += 1;
        if(handView != null)
        {
            handView.IncreaseMaxMana();
        }
        upgradesUI.updateUpgradesUI();
    }

    public void UpgradeDefense() // still unsure how i want defense to impact gameplay
    {
        if(baseDefense >= 10)
        {
            return;
        }
        if(upgradePoints <= 0)
        {
            return;
        }
        upgradePoints -= 1;
        baseDefense += 0.5f;
        upgradesUI.updateUpgradesUI();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

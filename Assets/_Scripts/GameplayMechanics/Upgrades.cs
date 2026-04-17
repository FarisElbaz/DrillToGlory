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

    public int UpgradePoints { get => upgradePoints; }
    public int Baseregen { get => baseregen; }
    public float BaseDamage { get => baseDamage; }
    public int BaseUtility { get => baseUtility; }
    public float BaseDefense { get => baseDefense; }

    [SerializeField] private Player player;


    public void ToggleUpgrades()
    {
        if (upgradePanel.activeInHierarchy)
        {
            upgradePanel.SetActive(false);
            distortionEffect.SetActive(false);
        }
        else if(upgradePanel.activeInHierarchy == false)
        {
            upgradePanel.SetActive(true);
            distortionEffect.SetActive(true);
        }
        upgradesUI.updateUpgradesUI();
        return;
    }

    public void AddUpgradePoint()
    {
        upgradePoints += 1;
        upgradesUI.updateUpgradesUI();
    }

    public void UpgradeRegen()
    {
        if(baseregen >= 15 || upgradePoints <= 0)
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
        if(baseDamage >= 10 || upgradePoints <= 0)
        {
            return;
        }
        upgradePoints -= 1;
        baseDamage += 0.5f;
        upgradesUI.updateUpgradesUI();
    }

    public void UpgradeUtility()
    { // Basically max mana
        if(baseUtility >= 3 || upgradePoints <= 0)
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
        if(baseDefense >= 10 || upgradePoints <= 0)
        {
            return;
        }
        upgradePoints -= 1;
        baseDefense += 0.5f;
        upgradesUI.updateUpgradesUI();
    }


    
}
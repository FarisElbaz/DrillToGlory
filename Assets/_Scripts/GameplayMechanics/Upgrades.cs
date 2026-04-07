using UnityEngine;

public class Upgrades : MonoBehaviour
{
    GameObject upgradePanel;
    GameObject distortionEffect;

    public float baseRegen = 1f;
    public float baseDamage = 1f;
    public float baseDefense = 1f;

    public void OpenUpgrades()
    {
        if (upgradePanel.activeSelf)
        {
            return;
        }
        upgradePanel.SetActive(true);
        distortionEffect.SetActive(true);
    }

    public void CloseUpgrades()
    {
        if (!upgradePanel.activeSelf)
        {
            return;
        }
        distortionEffect.SetActive(false);
        upgradePanel.SetActive(false);
    }

    public void UpgradeRegen()
    {
        

    }

    public void UpgradeDamage(){

    }

    public void UpgradeUtility()
    { // Basically max mana
        
    }

    public void UpgradeDefense()
    {

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

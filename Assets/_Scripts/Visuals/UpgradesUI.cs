using UnityEngine;
using TMPro;

public class UpgradesUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI upgradePointsText;
    [SerializeField] private TextMeshProUGUI regenText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI utilityText;
    [SerializeField] private TextMeshProUGUI defenseText;

    [SerializeField] private Upgrades upgrades;

    public void updateUpgradesUI()
    {
        upgradePointsText.text = "Upgrade Points: " + upgrades.UpgradePoints;
        regenText.text = "Regen: " + upgrades.Baseregen;
        damageText.text = "Damage: " + upgrades.BaseDamage;
        utilityText.text = "Utility: " + upgrades.BaseUtility;
        defenseText.text = "Defense: " + upgrades.BaseDefense;
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

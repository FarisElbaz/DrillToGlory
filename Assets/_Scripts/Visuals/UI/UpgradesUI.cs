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
        upgradePointsText.text = upgrades.UpgradePoints.ToString();
        regenText.text = upgrades.Baseregen.ToString();
        damageText.text = upgrades.BaseDamage.ToString();
        utilityText.text = upgrades.BaseUtility.ToString();
        defenseText.text = upgrades.BaseDefense.ToString();
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

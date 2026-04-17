using UnityEngine;

public class CardPlayer : MonoBehaviour
{
    [SerializeField] private float realityDamageToPlayerMultiplier = 1f;
    [SerializeField] private float realityDamageToEnemyMultiplier = 1f;
    [SerializeField] private float voidDamageToPlayerMultiplier = 1.25f;
    [SerializeField] private float voidDamageToEnemyMultiplier = 1.5f;

    private enum DamageTarget
    {
        Player,
        Enemy
    }

    public void CardClasses(CardData cardData, Player player, Enemy targetEnemy, Upgrades upgrades, Dimension currentDimension)
    {
        if (cardData == null || player == null)
        {
            return;
        }

        if (cardData.cardType == CardData.CardType.Heal)
        {
            player.regenerate(cardData.heal);
        }
        else if (cardData.cardType == CardData.CardType.Defense)
        {
            player.defend(cardData.defense);
            player.UpdateDefenseDisplay();
        }
        else if (cardData.cardType == CardData.CardType.Attack)
        {
            int damage = ApplyDimensionDamageModifier(cardData.damage, currentDimension, DamageTarget.Enemy);

            if (upgrades != null && upgrades.BaseDamage > 0)
            {
                damage += Mathf.RoundToInt(upgrades.BaseDamage);
            }

            if (targetEnemy != null)
            {
                Debug.Log("Dealing " + damage + " damage to enemy");
                targetEnemy.DamageTaken(damage);
            }
            else
            {
                Debug.LogWarning("No target enemy selected.");
            }
        }
    }

    private int ApplyDimensionDamageModifier(int baseDamage, Dimension currentDimension, DamageTarget target)
    {
        float multiplier = 1f;

        if (currentDimension == Dimension.Reality)
        {
            if (target == DamageTarget.Player)
            {
                multiplier = realityDamageToPlayerMultiplier;
            }
            else
            {
                multiplier = realityDamageToEnemyMultiplier;
            }
        }
        else if (currentDimension == Dimension.Void)
        {
            if (target == DamageTarget.Player)
            {
                multiplier = voidDamageToPlayerMultiplier;
            }
            else
            {
                multiplier = voidDamageToEnemyMultiplier;
            }
        }

        return Mathf.Max(0, Mathf.RoundToInt(baseDamage * multiplier));
    }
}

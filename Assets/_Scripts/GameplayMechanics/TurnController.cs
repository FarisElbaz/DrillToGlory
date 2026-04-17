using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class TurnController : MonoBehaviour
{
    [SerializeField] private float returnToPlayerTurnDelaySeconds = 3f;

    private HandView handView;
    private Player player;

    public void Initialize(HandView handViewRef, Player playerRef)
    {
        handView = handViewRef;
        player = playerRef;
    }

    public void EndTurn()
    {
        if (handView == null)
        {
            Debug.LogWarning("TurnController is missing HandView reference.");
            return;
        }

        Debug.Log("onEndTurn called, current state: " + handView.CurrentState);

        if (player == null)
        {
            Debug.LogWarning("Player stats reference is missing.");
            handView.SetGameState(HandView.GameState.PlayerTurn);
            return;
        }

        if (handView.CurrentState != HandView.GameState.PlayerTurn)
        {
            Debug.Log("Not player's turn, cannot end turn");
            return;
        }

        handView.ReturnHandToDeckAndShuffle();

        Debug.Log("Transitioning to Enemy Turn");
        handView.SetGameState(HandView.GameState.EnemyTurn);

        Debug.Log("Enemy dealing damage to player");

        var validEnemies = handView.Enemies.Where(e => e != null).ToList();
        foreach (var enemy in validEnemies)
        {
            enemy.dealDamage(player);
        }

        player.Defense = 0;
        player.UpdateDefenseDisplay();

        BackToPlayerTurn();
    }

    public void BackToPlayerTurn()
    {
        if (handView == null)
        {
            return;
        }

        if (handView.CurrentState == HandView.GameState.Victory || handView.CurrentState == HandView.GameState.Defeat)
        {
            return;
        }

        Debug.Log("Transitioning from Enemy Turn to Player Turn - delay starting");
        StartCoroutine(TransitionToPlayerTurnDelayed());
    }

    private IEnumerator TransitionToPlayerTurnDelayed()
    {
        yield return new WaitForSeconds(returnToPlayerTurnDelaySeconds);
        Debug.Log("Transitioning to Player Turn NOW");

        if (handView != null)
        {
            handView.StartPlayerTurn();
        }
    }

    public void HandleEnemyRemoved(Enemy deadEnemy)
    {
        if (handView == null)
        {
            return;
        }

        var enemies = handView.Enemies;
        if (enemies is List<Enemy> enemyList)
        {
            enemyList.RemoveAll(e => e == null || e == deadEnemy || e.CurrentHealth <= 0);
        }

        CheckForVictory();
    }

    private void CheckForVictory()
    {
        if (handView == null || handView.CurrentState == HandView.GameState.Defeat)
        {
            return;
        }


        Enemy[] activeSceneEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        bool hasLivingEnemyInScene = activeSceneEnemies.Any(e => e != null && e.CurrentHealth > 0);

        if (!hasLivingEnemyInScene)
        {
            Debug.Log("All enemies defeated, transitioning to Victory state");
            handView.SetGameState(HandView.GameState.Victory);
        }
    }
}

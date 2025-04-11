using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public PlayerControl player;
    public Enemy enemy;

    private enum TurnPhase { Player, Enemy } // Keeps track of turn
    private TurnPhase currentPhase = TurnPhase.Player;

    void Start()
    {
        ActivatePlayerPhase(); // Starts with player's turn
    }

    public void CompletePlayerTurn()
    {
        currentPhase = TurnPhase.Enemy;
        ActivateEnemyPhase(); // Moves to enemys turn
    }

    public void CompleteEnemyTurn()
    {
        currentPhase = TurnPhase.Player;
        ActivatePlayerPhase(); // Back to player's turn
    }

    private void ActivatePlayerPhase()
    {
        player.enabled = true;
        enemy.enabled = false;
        player.InitializeTurn(); // Player is set up to start
    }

    private void ActivateEnemyPhase()
    {
        player.enabled = false;
        enemy.enabled = true;
        enemy.StartTurn(); // Enemy begins its turn
    }

    public bool IsPlayerActive()
    {
        return currentPhase == TurnPhase.Player; // Checks if its player's turn
    }

    public bool IsEnemyActive()
    {
        return currentPhase == TurnPhase.Enemy; // Checks if its enemys turn
    }
}
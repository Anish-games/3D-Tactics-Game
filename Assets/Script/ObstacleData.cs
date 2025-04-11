using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleData", menuName = "Game/Obstacle Data")]
public class ObstacleData : ScriptableObject
{
   public bool[] blockedCells = new bool[100];
}
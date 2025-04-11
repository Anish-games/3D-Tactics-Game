using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [Header("Obstacle Settings")]
    public ObstacleData obstacleData;
    public GameObject obstaclePrefab;
    public Transform gridParent;

    void Start()
    {
        PlaceObstacles(); // Sets up obstacles at game start
    }

    private void PlaceObstacles()
    {
        for (int i = 0; i < 10; i++) // Loop through the rows
        {
            for (int j = 0; j < 10; j++) // Loop through the colums
            {
                int index = i * 10 + j;

                if (obstacleData.blockedCells[index]) // Check if cell is blocked
                {
                    Vector3 obstaclePosition = new Vector3(i * 2f, 0.5f, j * 2f); // Gets position
                    Instantiate(obstaclePrefab, obstaclePosition, Quaternion.identity, gridParent); // Puts obstacle
                }
            }
        }
    }
}
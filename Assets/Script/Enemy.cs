using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement Settings")]
    private float moveSpeed = 3f;

    [Header("References")]
    public GridManager gridManager;       // Reference to GridManager (which must expose gridNodes, gridSize, tileSpacing, Node)
    public ObstacleData obstacleData;     // Reference to ObstacleData (which must expose public bool[] blockedCells)
    public Transform player;              // Reference to the player's transform

    private Move turnManager;      // Reference to TurnManager to decide turn timing

    private bool isMoving = false;
    private Vector3 targetPosition;       // Next target (tile) position
    private Vector3 currentTilePosition;  // Enemy's current tile-based position
    private List<GridManager.Node> path;  // Computed BFS path (list of nodes)

    private Collider playerCollider;      // Player collider reference
    private Collider enemyCollider;       // Enemy's own collider

    void Start()
    {
        // Initialize positions.
        targetPosition = transform.position;
        currentTilePosition = transform.position;

        // Get TurnManager from the scene.
        turnManager = FindObjectOfType<Move>();

        // Get player's collider.
        if (player != null)
            playerCollider = player.GetComponent<Collider>();

        // Get enemy's own collider.
        enemyCollider = GetComponent<Collider>();
    }

    void Update()
    {
        // Only proceed if it’s the enemy’s turn and we're not already moving.
        if (!turnManager.IsEnemyActive() || isMoving)
            return;

        // Determine the start and target nodes based on positions.
        GridManager.Node startNode = GetNodeFromPosition(currentTilePosition);
        GridManager.Node targetNode = GetNodeFromPosition(player.position);

        // Compute a path using our BFS method.
        List<GridManager.Node> bfsPath = CalculateRouteBFS(startNode, targetNode);
        if (bfsPath != null && bfsPath.Count > 0)
        {
            path = bfsPath;
            StartCoroutine(TraverseRouteBFS());
        }
        else
        {
            // If no path is found, end enemy turn.
            Debug.Log("No route found to the player. Ending enemy's turn.");
            turnManager.CompleteEnemyTurn();
        }
    }

    // Called when enemy's turn begins.
    public void StartTurn()
    {
        Debug.Log("Enemy's Turn Started");
    }

    // Converts a world position into a GridManager.Node.
    private GridManager.Node GetNodeFromPosition(Vector3 pos)
    {
        int x = Mathf.RoundToInt(pos.x / gridManager.tileSpacing);
        int z = Mathf.RoundToInt(pos.z / gridManager.tileSpacing);
        return gridManager.gridNodes[x, z];
    }

    // BFS to calculate a route from start to target.
    private List<GridManager.Node> CalculateRouteBFS(GridManager.Node start, GridManager.Node target)
    {
        Queue<GridManager.Node> queue = new Queue<GridManager.Node>();
        Dictionary<GridManager.Node, GridManager.Node> cameFrom = new Dictionary<GridManager.Node, GridManager.Node>();
        HashSet<GridManager.Node> visited = new HashSet<GridManager.Node>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            GridManager.Node current = queue.Dequeue();
            if (current == target)
            {
                return ReconstructPathBFS(start, current, cameFrom);
            }

            foreach (GridManager.Node neighbor in GetNeighborsBFS(current))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    cameFrom[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }
        }
        return null; // No valid path found.
    }

    // Constructs the path by retracing steps in the cameFrom dictionary.
    private List<GridManager.Node> ReconstructPathBFS(GridManager.Node start, GridManager.Node end, Dictionary<GridManager.Node, GridManager.Node> cameFrom)
    {
        List<GridManager.Node> route = new List<GridManager.Node>();
        GridManager.Node current = end;
        while (current != start)
        {
            route.Add(current);
            current = cameFrom[current];
        }
        route.Reverse();
        return route;
    }

    // Returns neighboring nodes in the four cardinal directions (if within bounds and not blocked).
    private List<GridManager.Node> GetNeighborsBFS(GridManager.Node node)
    {
        List<GridManager.Node> neighbors = new List<GridManager.Node>();
        int[] dx = { -1, 1, 0, 0 };
        int[] dz = { 0, 0, -1, 1 };

        for (int i = 0; i < 4; i++)
        {
            int newX = node.gridX + dx[i];
            int newZ = node.gridZ + dz[i];

            if (newX >= 0 && newX < gridManager.gridSize && newZ >= 0 && newZ < gridManager.gridSize)
            {
                // Compute the index for the one-dimensional obstacle array.
                int index = newX * gridManager.gridSize + newZ;
                // Check that there isn’t an obstacle on the neighbor.
                if (!obstacleData.blockedCells[index])
                {
                    neighbors.Add(gridManager.gridNodes[newX, newZ]);
                }
            }
        }
        return neighbors;
    }

    // Coroutine to traverse the computed path.
    private IEnumerator TraverseRouteBFS()
    {
        isMoving = true;
        foreach (GridManager.Node step in path)
        {
            // Set target position slightly raised (to account for enemy height).
            targetPosition = step.worldPosition + Vector3.up * 0.5f;

            // Optionally, if the enemy is about to run into the player's zone, break early.
            if (IsPlayerNearby(targetPosition))
            {
                break;
            }

            // Move toward the target position.
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
            currentTilePosition = targetPosition;
        }
        isMoving = false;
        turnManager.CompleteEnemyTurn();
        yield return null;
    }

    // Checks if the enemy's next position would be too close to the player's collider.
    private bool IsPlayerNearby(Vector3 targetPos)
    {
        if (playerCollider == null || enemyCollider == null)
            return false;

        float dist = Vector3.Distance(targetPos, player.position);
        // Threshold based on half the widths (adjust as needed)
        float threshold = (playerCollider.bounds.size.x + enemyCollider.bounds.size.x) * 0.25f;
        return dist < threshold;
    }
}
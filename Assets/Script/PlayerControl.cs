using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // Speed for movement

    [Header("References")]
    public GridManager gridManager;   // Reference to the GridManager (provides gridNodes & tileSpacing)
    public ObstacleData obstacleData; // Reference to the obstacle data (with blockedTiles array)

    private bool isMoving = false;         // Prevents input while moving
    private Vector3 currentPos;            // Player's current grid-centered world position
    private Vector3 destination;           // The next target position
    private List<GridManager.Node> path;   // The list of nodes representing the path

    private Move turnManager;       // Reference to the TurnManager

    void Start()
    {
        // Initialize the current and destination positions
        currentPos = transform.position;
        destination = transform.position;
        // Locate the TurnManager from the scene
        turnManager = FindObjectOfType<Move>();
    }

    void Update()
    {
        // Only process input if it's the player's turn and the player isn't already moving
        if (turnManager.IsPlayerActive() && !isMoving)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ProcessMouseClick_BFS();
            }
        }
    }

    // Called when the player's turn starts – resets the movement flag.
    public void InitializeTurn()
    {
        isMoving = false;
        // Additional turn-initialization code can go here.
    }

    // Process a mouse click using a BFS–based pathfinding approach.
    private void ProcessMouseClick_BFS()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject clickedTile = hit.collider.gameObject;
            TileInfo tileComponent = clickedTile.GetComponent<TileInfo>();

            if (tileComponent != null)
            {
                // Convert the clicked tile's world position into grid coordinates
                int gridX = Mathf.RoundToInt(clickedTile.transform.position.x / gridManager.tileSpacing);
                int gridZ = Mathf.RoundToInt(clickedTile.transform.position.z / gridManager.tileSpacing);

                GridManager.Node startNode = GetNodeFromWorld(currentPos);
                GridManager.Node targetNode = gridManager.gridNodes[gridX, gridZ];

                // Compute a path using BFS.
                List<GridManager.Node> bfsPath = FindPath_BFS(startNode, targetNode);
                if (bfsPath != null && bfsPath.Count > 0)
                {
                    path = bfsPath;
                    StartCoroutine(TraversePath_BFS());
                }
            }
        }
    }

    // Converts a world position into a grid node by dividing by the tile spacing.
    private GridManager.Node GetNodeFromWorld(Vector3 pos)
    {
        int x = Mathf.RoundToInt(pos.x / gridManager.tileSpacing);
        int z = Mathf.RoundToInt(pos.z / gridManager.tileSpacing);
        return gridManager.gridNodes[x, z];
    }

    // Uses a breadth-first search (BFS) algorithm to compute a path from start to target.
    private List<GridManager.Node> FindPath_BFS(GridManager.Node start, GridManager.Node target)
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
                // Found the destination – reconstruct the path by following parent links.
                List<GridManager.Node> reconstructedPath = new List<GridManager.Node>();
                GridManager.Node currentPathNode = target;
                while (currentPathNode != start)
                {
                    reconstructedPath.Add(currentPathNode);
                    currentPathNode = cameFrom[currentPathNode];
                }
                reconstructedPath.Reverse();
                return reconstructedPath;
            }

            foreach (GridManager.Node neighbor in GetNeighbors_BFS(current))
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

    // Returns adjacent (cardinal) neighbors for the given node that are traversable.
    private List<GridManager.Node> GetNeighbors_BFS(GridManager.Node node)
    {
        List<GridManager.Node> neighbors = new List<GridManager.Node>();

        // Define four cardinal direction offsets (left, right, down, up)
        int[] dx = { -1, 1, 0, 0 };
        int[] dz = { 0, 0, -1, 1 };

        for (int i = 0; i < 4; i++)
        {
            int newX = node.gridX + dx[i];
            int newZ = node.gridZ + dz[i];

            if (newX >= 0 && newX < gridManager.gridSize && newZ >= 0 && newZ < gridManager.gridSize)
            {
                int index = newX * gridManager.gridSize + newZ;
                // Only consider this neighbor if it’s walkable, not blocked by an obstacle, and does not host an enemy.
                if (!obstacleData.blockedCells[index] && !EnemyExistsAt(gridManager.gridNodes[newX, newZ].worldPosition))
                {
                    neighbors.Add(gridManager.gridNodes[newX, newZ]);
                }
            }
        }
        return neighbors;
    }

    // Checks for an enemy at a given position by using a small overlap sphere.
    private bool EnemyExistsAt(Vector3 pos)
    {
        Collider[] colliders = Physics.OverlapSphere(pos, 0.1f);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Enemy"))
                return true;
        }
        return false;
    }

    // Coroutine to move the player along the computed BFS path.
    private IEnumerator TraversePath_BFS()
    {
        isMoving = true;
        foreach (GridManager.Node step in path)
        {
            // Raise the destination slightly above the ground.
            destination = step.worldPosition + Vector3.up * 0.5f;
            while (Vector3.Distance(transform.position, destination) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
                yield return null;
            }
            currentPos = destination;
        }
        isMoving = false;
        turnManager.CompletePlayerTurn(); // End the player's turn when movement is finished.
        yield return null;
    }
}
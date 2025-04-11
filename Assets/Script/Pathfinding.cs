using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager; // For getting nodes from the grid

    public List<GridManager.Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        var startNode = gridManager.GetNodeFromWorldPosition(startPos);
        var targetNode = gridManager.GetNodeFromWorldPosition(targetPos);

        if (startNode == null || targetNode == null || !targetNode.isWalkable)
        {
            Debug.LogError("Either path is invalid or target node is blocked");
            return null; // If no route is possible
        }

        var openSet = new List<GridManager.Node> { startNode }; // Nodes we need to check
        var closedSet = new HashSet<GridManager.Node>(); // Nodes we alreddy checked

        while (openSet.Count > 0)
        {
            var currentNode = GetLowestFCostNode(openSet);

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode) // Found the target
            {
                return RetracePath(startNode, targetNode); // Builds and returns path
            }

            foreach (var neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor)) continue;

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor); // Calculate cost
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor); // Add neighbor to list
                    }
                }
            }
        }

        return null; // If no path is found
    }

    private GridManager.Node GetLowestFCostNode(List<GridManager.Node> nodes)
    {
        GridManager.Node bestNode = nodes[0]; // Start with first node
        foreach (var node in nodes)
        {
            if (node.fCost < bestNode.fCost || (node.fCost == bestNode.fCost && node.hCost < bestNode.hCost))
            {
                bestNode = node; // Update the best node
            }
        }
        return bestNode;
    }

    private List<GridManager.Node> RetracePath(GridManager.Node startNode, GridManager.Node endNode)
    {
        var path = new List<GridManager.Node>();
        var currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent; // Go backwards from end
        }

        path.Reverse(); // Makes path start from the begining
        return path;
    }

    private List<GridManager.Node> GetNeighbors(GridManager.Node node)
    {
        var neighbors = new List<GridManager.Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (Mathf.Abs(x) == Mathf.Abs(z)) continue;

                int checkX = node.gridX + x;
                int checkZ = node.gridZ + z;

                if (checkX >= 0 && checkX < gridManager.gridSize && checkZ >= 0 && checkZ < gridManager.gridSize)
                {
                    neighbors.Add(gridManager.gridNodes[checkX, checkZ]); // Add valid neighbor
                }
            }
        }

        return neighbors;
    }

    private int GetDistance(GridManager.Node a, GridManager.Node b)
    {
        return Mathf.Abs(a.gridX - b.gridX) + Mathf.Abs(a.gridZ - b.gridZ); // Distance based on steps
    }
}
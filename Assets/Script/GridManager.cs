using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject tilePrefab; 
    public int gridSize = 10;
    public float tileSpacing = 2f; 

    public Node[,] gridNodes; 

    void Start()
    {
        CreateGrid(); // call the grid create fn when game starts
    }

    private void CreateGrid()  // make 2d array to hold grid data
    {
        gridNodes = new Node[gridSize, gridSize];

       
        for (int z = 0; z < gridSize; z++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                Vector3 tilePosition = new Vector3(x * tileSpacing, 0, z * tileSpacing);

                
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);   // giv the tile a name so its easy to find in hierarcy
                tile.name = $"Tile ({z},{x})";

                
                AddTileComponents(tile);   // add extra stuff to tile like collider


                var tileInfo = tile.GetComponent<TileInfo>();
                if (tileInfo != null && tileInfo.UItext != null)
                {
                    tileInfo.UItext.text = $"({z}x{x + 1})";
                }

                // Create a walkable grid node for this tile
                gridNodes[x, z] = new Node(tile.transform.position, true, x, z);
            }
        }
    }

    private void AddTileComponents(GameObject tile)
    {
        if (!tile.GetComponent<TileInfo>())
        {
            tile.AddComponent<TileInfo>();   // add a script to store tile info if not there
        }
        if (!tile.GetComponent<Collider>())
        {
            tile.AddComponent<BoxCollider>();    // add box collider if not alredy there
        }
    }

    [System.Serializable]
    public class Node
    {
        public Vector3 worldPosition; // World position of the node
        public bool isWalkable; // Whether the node is walkable
        public int gridX, gridZ; // Grid coordinates: gridX for column, gridZ for row
        public Node parent; // Parent node (for pathfinding)
        public int gCost; // Movement cost
        public int hCost; // Heuristic cost

       
        public int fCost => gCost + hCost;   // used in path finding (A star stuff)

        public Node(Vector3 worldPosition, bool isWalkable, int gridX, int gridZ)
        {
            this.worldPosition = worldPosition;
            this.isWalkable = isWalkable;
            this.gridX = gridX;
            this.gridZ = gridZ;
        }
    }

    public Node GetNodeFromWorldPosition(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / tileSpacing);
        int z = Mathf.FloorToInt(position.z / tileSpacing);

        if (x >= 0 && x < gridSize && z >= 0 && z < gridSize)
        {
            return gridNodes[x, z];
        }
        return null;
    }

    public void SetObstacle(Vector3 position)
    {
        var node = GetNodeFromWorldPosition(position);
        if (node != null)
        {
            node.isWalkable = false;
        }
    }

    public void SetWalkable(Vector3 position)
    {
        var node = GetNodeFromWorldPosition(position);
        if (node != null)
        {
            node.isWalkable = true;
        }
    }
}
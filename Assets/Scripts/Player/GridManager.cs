using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject pilotPrefab;
    public GameObject gridPrefab;

    private int column = 3;
    private int row = 3;
    private int pilotColumn = 1;
    private int pilotRow = 1;
    // TODO: use dictionary <Grid, GameObject> instead; this thing needs resize!
    private GameObject[,] grids = new GameObject[3, 3];
    private Dictionary<Grid, Collider2D> colliders = new Dictionary<Grid, Collider2D>();

    void Start()
    {
        grids[1, 1] = Instantiate(pilotPrefab, transform.position, transform.rotation, transform);
        grids[1, 2] = Instantiate(gridPrefab, transform.position, transform.rotation, transform);
        CalculateGridHint();
    }

    void Update()
    {
        PositionBlocks();
    }

    void PositionBlocks()
    {
        for (int c = 0; c < column; c++)
        {
            for (int r = 0; r < row; r++)
            {
                if (grids[c, r] != null)
                {
                    GameObject block = grids[c, r];
                    float x = c - pilotColumn;
                    float y = pilotRow - r;
                    block.transform.localPosition = new Vector3(x, y, 0);
                    // block.transform.rotation = transform.rotation;
                }
            }
        }
    }

    void CalculateGridHint()
    {
        for (int c = 0; c < column; c++)
        {
            for (int r = 0; r < row; r++)
            {
                if (grids[c, r] != null && grids[c, r].GetComponent<AssembledBlock>() != null)
                {
                    // left
                    if (c > 0 && grids[c - 1, r] == null)
                    {
                        grids[c - 1, r] = Instantiate(gridPrefab, transform.position, transform.rotation, transform);
                    }
                    // right
                    if (c + 1 < column && grids[c + 1, r] == null)
                    {
                        grids[c + 1, r] = Instantiate(gridPrefab, transform.position, transform.rotation, transform);
                    }
                    // up
                    if (r + 1 < row && grids[c, r + 1] == null)
                    {
                        grids[c, r + 1] = Instantiate(gridPrefab, transform.position, transform.rotation, transform);
                    }
                    // down
                    if (r > 0 && grids[c, r - 1] == null)
                    {
                        grids[c, r - 1] = Instantiate(gridPrefab, transform.position, transform.rotation, transform);
                    }
                }
            }
        }
    }

    Grid WorldPositionToGrid(Vector3 worldPosition)
    {
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);
        int x = (int)Math.Round(localPosition.x);
        int y = (int)Math.Round(localPosition.y);
        return new Grid(x + pilotColumn, pilotRow - y);
    }

    // TODO: Refactor this function, make it calls another one
    public void NotifyDroppedLooseBlock(Vector3 worldPosition, GameObject looseBlock)
    {
        Grid grid = WorldPositionToGrid(worldPosition);
        Debug.Log(grid.x + ", " + grid.y);

        // filter out index out of length
        if (grid.x < 0 || grid.y < 0 || grid.x > grids.GetUpperBound(0) || grid.y > grids.GetUpperBound(1))
        {
            return;
        }

        // check if there is an empty space on that grid
        if (grids[grid.x, grid.y] != null && grids[grid.x, grid.y].GetComponent<GridHint>() != null)
        {
            // Add an assembled block to that space with health taken from the original block
            int newBlockHealth = looseBlock.GetComponent<LooseBlock>().healthPoint;
            GameObject newBlockPrefab = looseBlock.GetComponent<LooseBlock>().assembledPrefab;
            GameObject newBlock = Instantiate(newBlockPrefab, transform.position, transform.rotation, transform);
            newBlock.GetComponent<AssembledBlock>().healthPoint = newBlockHealth;
            // Destroy the original looseBlock
            Destroy(looseBlock);
            Destroy(grids[grid.x, grid.y]);
            grids[grid.x, grid.y] = newBlock;
            // Add a box collider
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>() as BoxCollider2D;
            collider.offset = new Vector2(grid.x - pilotColumn, pilotRow - grid.y);
            colliders.Add(grid, collider);
        }
    }

    // public void AddBlock(int row, int column, GameObject block)
    // {
    //     grids[row, column] = block;
    // }

}

class Grid
{
    public int x;
    public int y;

    public Grid(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

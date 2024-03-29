using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject pilotPrefab;
    public GameObject gridPrefab;

    private Rigidbody2D body;

    private int column = 3;
    private int row = 3;
    private int pilotColumn = 1;
    private int pilotRow = 1;
    // TODO: use dictionary <Grid, GameObject> instead; this thing needs resize!
    private GameObject[,] grids = new GameObject[3, 3];
    private Dictionary<Grid, Collider2D> colliders = new Dictionary<Grid, Collider2D>();

    void Start()
    {
        body = GetComponent<Rigidbody2D>();

        grids[1, 1] = Instantiate(pilotPrefab, transform.position, transform.rotation, transform);
        CalculateGridHint();
        PositionBlocks();
    }

    void Update()
    {
        // PositionBlocks();
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
                }
            }
        }
    }

    bool IsGridOutOfBound(Grid grid)
    {
        return (grid.x < 0 || grid.y < 0 || grid.x > grids.GetUpperBound(0) || grid.y > grids.GetUpperBound(1));
    }

    bool IsGridOnTheEdge(Grid grid)
    {
        return (grid.x == 0 || grid.y == 0 || grid.x == grids.GetUpperBound(0) || grid.y == grids.GetUpperBound(1));
    }

    bool IsEmptyAndAttachableGrid(Grid grid)
    {
        return (grids[grid.x, grid.y] != null && grids[grid.x, grid.y].GetComponent<GridHint>() != null);
    }

    Grid WorldPositionToGrid(Vector3 worldPosition)
    {
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);
        int x = (int)Math.Round(localPosition.x);
        int y = (int)Math.Round(localPosition.y);
        return new Grid(x + pilotColumn, pilotRow - y);
    }

    Grid ResizeForNewGrid(Grid newGrid)
    {
        if (newGrid.x == 0)
        {
            GameObject[,] recalculatedGrids = new GameObject[grids.GetLength(0) + 1, grids.GetLength(1)];
            column += 1;
            pilotColumn += 1;
            for (int c = 1; c < column; c++)
            {
                for (int r = 0; r < row; r++)
                {
                    recalculatedGrids[c, r] = grids[c - 1, r];
                }
            }
            grids = recalculatedGrids;
            return new Grid(newGrid.x + 1, newGrid.y);
        }
        if (newGrid.x == grids.GetUpperBound(0))
        {
            GameObject[,] recalculatedGrids = new GameObject[grids.GetLength(0) + 1, grids.GetLength(1)];
            column += 1;
            for (int c = 0; c < column - 1; c++)
            {
                for (int r = 0; r < row; r++)
                {
                    recalculatedGrids[c, r] = grids[c, r];
                }
            }
            grids = recalculatedGrids;
            return newGrid;
        }
        if (newGrid.y == 0)
        {
            GameObject[,] recalculatedGrids = new GameObject[grids.GetLength(0), grids.GetLength(1) + 1];
            row += 1;
            pilotRow += 1;
            for (int c = 0; c < column; c++)
            {
                for (int r = 1; r < row; r++)
                {
                    recalculatedGrids[c, r] = grids[c, r - 1];
                }
            }
            grids = recalculatedGrids;
            return new Grid(newGrid.x, newGrid.y + 1);
        }
        if (newGrid.y == grids.GetUpperBound(1))
        {
            GameObject[,] recalculatedGrids = new GameObject[grids.GetLength(0), grids.GetLength(1) + 1];
            row += 1;
            for (int c = 0; c < column; c++)
            {
                for (int r = 0; r < row - 1; r++)
                {
                    recalculatedGrids[c, r] = grids[c, r];
                }
            }
            grids = recalculatedGrids;
            return newGrid;
        }
        Debug.LogError("Invalid call: ResizeForNewGrid" + newGrid.ToString());
        return null;
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

    public void NotifyDroppedLooseBlock(Vector3 worldPosition, GameObject looseBlock)
    {
        Grid grid = WorldPositionToGrid(worldPosition);
        if (IsGridOutOfBound(grid) || !IsEmptyAndAttachableGrid(grid))
        {
            return;
        }
        AddNewBlock(grid, looseBlock);
    }

    void AddNewBlock(Grid grid, GameObject looseBlock)
    {
        if (IsGridOnTheEdge(grid))
        {
            grid = ResizeForNewGrid(grid);
        }

        GameObject newBlock = InstantiateAssembledBlock(looseBlock);
        Destroy(looseBlock);
        Destroy(grids[grid.x, grid.y]);
        grids[grid.x, grid.y] = newBlock;
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>() as BoxCollider2D;
        collider.offset = new Vector2(grid.x - pilotColumn, pilotRow - grid.y);
        colliders.Add(grid, collider);
        CalculateGridHint();
        PositionBlocks();
        AddMass(0.2f);
    }

    GameObject InstantiateAssembledBlock(GameObject looseBlock)
    {
        int newBlockHealth = looseBlock.GetComponent<LooseBlock>().healthPoint;
        GameObject newBlockPrefab = looseBlock.GetComponent<LooseBlock>().assembledPrefab;
        GameObject newBlock = Instantiate(newBlockPrefab, transform.position, transform.rotation, transform);
        newBlock.GetComponent<AssembledBlock>().healthPoint = newBlockHealth;
        return newBlock;
    }

    void AddMass(float mass)
    {
        body.mass += mass;
    }

    void TestLogGrid()
    {
        for (int r = 0; r < row; r++)
        {
            string line = "";
            for (int c = 0; c < column; c++)
            {
                line += grids[c, r] + "\t";
            }
            Debug.Log(line);
        }
    }
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

    public override string ToString()
    {
        return "( " + x + ", " + y + ")";
    }
}

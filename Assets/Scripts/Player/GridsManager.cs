using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridsManager : MonoBehaviour
{
    public GameObject gridPrefab;

    private int column = 3;
    private int row = 3;
    private int pilotColumn = 1;
    private int pilotRow = 1;
    private GameObject[,] grids = new GameObject[3, 3];

    void Start()
    {
        grids[1, 1] = gameObject;
        grids[1, 2] = Instantiate(gridPrefab, transform.position, transform.rotation, transform);
        CalculateGrids();
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
                    float x = transform.position.x + (c - pilotColumn);
                    float y = transform.position.y + (r - pilotRow);
                    block.transform.position = new Vector3(x, y, 0);
                    block.transform.rotation = transform.rotation;
                }
            }
        }
    }

    void CalculateGrids()
    {
        for (int c = 0; c < column; c++)
        {
            for (int r = 0; r < row; r++)
            {
                if (grids[c, r] != null && grids[c, r].GetComponent<Block>() != null)
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

    public void AddObject(int row, int column, GameObject block)
    {
        grids[row, column] = block;
    }
}

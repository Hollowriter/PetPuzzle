using System.Collections;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;
    [SerializeField] private GameObject[] gemPrefabs;  // Different gem types
    [SerializeField] private GameObject[,] gridArray;

    private void Start()
    {
        gridArray = new GameObject[width, height];
        FillGrid();
    }

    // Fill the grid with random gems
    private void FillGrid()
    {
        for (int y = 0; y < width; y++)
        {
            for (int x = 0; x < height; x++)
            {
                Vector2 position = new Vector2(x, -y);
                int gemIndex = Random.Range(0, gemPrefabs.Length);
                GameObject newGem = Instantiate(gemPrefabs[gemIndex], position, Quaternion.identity);
                gridArray[x, y] = newGem;
            }
        }
    }

    // Swap two gems in the grid
    public void SwapGems(Vector2 firstPos, Vector2 secondPos)
    {
        GameObject temp = gridArray[(int)firstPos.x, (int)firstPos.y];
        gridArray[(int)firstPos.x, (int)firstPos.y] = gridArray[(int)secondPos.x, (int)secondPos.y];
        gridArray[(int)secondPos.x, (int)secondPos.y] = temp;

        // Move gems visually
        gridArray[(int)firstPos.x, (int)firstPos.y].transform.position = firstPos;
        gridArray[(int)secondPos.x, (int)secondPos.y].transform.position = secondPos;

        StartCoroutine(CheckMatches());
    }

    // Check if there are any matches on the grid
    private IEnumerator CheckMatches()
    {
        yield return new WaitForSeconds(0.2f);

        // Logic to check for 3 or more in a row
        for (int y = 0; y < width; y++)
        {
            for (int x = 0; x < height; x++)
            {
                GameObject gem = gridArray[x, y];
                // Check if there are matching gems (horizontal or vertical)
                if (gem != null && CheckForMatch(x, y))
                {
                    Destroy(gridArray[x, y]);
                    gridArray[x, y] = null;
                }
            }
        }

        // After destroying matches, refill the grid
        yield return new WaitForSeconds(0.2f);
        RefillGrid();
    }

    private bool CheckForMatch(int x, int y)
    {
        // Check horizontal and vertical matches (simple 3 in a row logic)
        // Horizontal
        if (x < width - 2 && gridArray[x, y] != null && gridArray[x + 1, y] != null && gridArray[x + 2, y] != null)
        {
            if (gridArray[x, y].tag == gridArray[x + 1, y].tag && gridArray[x, y].tag == gridArray[x + 2, y].tag)
            {
                return true;
            }
        }

        // Vertical
        if (y < height - 2 && gridArray[x, y] != null && gridArray[x, y + 1] != null && gridArray[x, y + 2] != null)
        {
            if (gridArray[x, y].tag == gridArray[x, y + 1].tag && gridArray[x, y].tag == gridArray[x, y + 2].tag)
            {
                return true;
            }
        }

        return false;
    }

    // Refill grid after matches have been cleared
    private void RefillGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (gridArray[x, y] == null)
                {
                    Vector2 position = new Vector2(x, y);
                    int gemIndex = Random.Range(0, gemPrefabs.Length);
                    GameObject newGem = Instantiate(gemPrefabs[gemIndex], position, Quaternion.identity);
                    gridArray[x, y] = newGem;
                }
            }
        }
    }

    public int GetWidth() 
    {
        return width;
    }

    public int GetHeight() 
    { 
        return height; 
    }
}

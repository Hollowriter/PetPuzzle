using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public enum PieceType 
    {
        NORMAL,
        COUNT
    }
    [System.Serializable]
    public struct PiecePrefab 
    {
        public PieceType Type;
        public GameObject prefab;
    }
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;
    [SerializeField] private PiecePrefab[] gemPrefabs;  // Different gem types
    [SerializeField] private Gem[,] gridArray;
    private Dictionary<PieceType, GameObject> piecePrefabDictionary;

    private void Start()
    {
        gridArray = new Gem[width, height];
        piecePrefabDictionary = new Dictionary<PieceType, GameObject>();
        for (int i = 0; i < gemPrefabs.Length; i++) 
        {
            if (!piecePrefabDictionary.ContainsKey(gemPrefabs[i].Type)) 
            {
                piecePrefabDictionary.Add(gemPrefabs[i].Type, gemPrefabs[i].prefab);
            }
        }
        FillGrid();
    }

    // Fill the grid with random gems
    private void FillGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 position = new Vector2(x, y);
                /*int gemIndex = Random.Range(0, gemPrefabs.Length);*/
                GameObject newGem = (GameObject)Instantiate(piecePrefabDictionary[PieceType.NORMAL], Vector3.zero, Quaternion.identity);
                newGem.transform.parent = transform;
                gridArray[x, y] = newGem.GetComponent<Gem>();
                gridArray[x, y].Create(x, y, PieceType.NORMAL);
                gridArray[x, y].MoveGem.Move(x, y);
                if (gridArray[x, y].IsColored()) 
                {
                    gridArray[x, y].ColorGem.SetColor((ColorGem.ColorType)Random.Range(0, gridArray[x, y].ColorGem.NumColors));
                }
            }
        }
    }

    // Swap two gems in the grid
    /*public void SwapGems(Vector2 firstPos, Vector2 secondPos)
    {
        GameObject temp = gridArray[(int)firstPos.x, (int)firstPos.y];
        gridArray[(int)firstPos.x, (int)firstPos.y] = gridArray[(int)secondPos.x, (int)secondPos.y];
        gridArray[(int)secondPos.x, (int)secondPos.y] = temp;

        // Move gems visually
        gridArray[(int)firstPos.x, (int)firstPos.y].transform.position = firstPos;
        gridArray[(int)secondPos.x, (int)secondPos.y].transform.position = secondPos;

        StartCoroutine(CheckMatches());
    }*/

    // Check if there are any matches on the grid
    /*private IEnumerator CheckMatches()
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
    }*/

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
                    /*int gemIndex = Random.Range(0, gemPrefabs.Length);
                    GameObject newGem = Instantiate(gemPrefabs[gemIndex], position, Quaternion.identity);
                    gridArray[x, y] = newGem;*/
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

    public Vector2 GetWorldPosition(int x, int y) 
    {
        return new Vector2(transform.position.x - width / 2.0f + x, transform.position.y + height / 2.0f - y);
    }
}

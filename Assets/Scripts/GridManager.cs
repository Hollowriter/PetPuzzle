using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public enum PieceType 
    {
        EMPTY,
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

    public Gem SpawnNewGem(int x, int y, PieceType type) 
    {
        GameObject newGem = (GameObject)Instantiate(piecePrefabDictionary[type], GetWorldPosition(x, y), Quaternion.identity);
        newGem.transform.parent = transform;
        gridArray[x, y] = newGem.GetComponent<Gem>();
        gridArray[x, y].Create(x, y, type);
        return gridArray[x, y];
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

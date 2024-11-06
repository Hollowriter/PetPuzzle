using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public enum PieceType 
    {
        EMPTY,
        NORMAL,
        UNMOVABLE,
        ROW_CLEAR,
        COLUMN_CLEAR,
        RAINBOW,
        COUNT
    }
    [System.Serializable]
    public struct PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;
    }
    [System.Serializable]
    public struct PiecePosition 
    {
        public PieceType type;
        public int x;
        public int y;
    }
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float fillTime;
    [SerializeField] private PiecePrefab[] gemPrefabs;
    [SerializeField] private PiecePosition[] gemPositions;
    [SerializeField] private Gem[,] gridArray;
    [SerializeField] private int matchGems;
    [SerializeField] private Level level;
    private Dictionary<PieceType, GameObject> piecePrefabDictionary;
    private bool inverse = false;
    private bool gameOver = false;
    private Gem pressedGem;
    private Gem enteredGem;

    private void Start()
    {
        gridArray = new Gem[width, height];
        piecePrefabDictionary = new Dictionary<PieceType, GameObject>();
        for (int i = 0; i < gemPrefabs.Length; i++) 
        {
            if (!piecePrefabDictionary.ContainsKey(gemPrefabs[i].type)) 
            {
                piecePrefabDictionary.Add(gemPrefabs[i].type, gemPrefabs[i].prefab);
            }
        }
        FillGrid();
        StartCoroutine(FillWithGems());
    }

    public IEnumerator FillWithGems() 
    {
        bool needsRefill = true;
        while (needsRefill)
        {
            while (FillStep())
            {
                inverse = !inverse;
                yield return new WaitForSeconds(fillTime);
            }
            needsRefill = ClearAllValidMatches();
            yield return new WaitForSeconds(fillTime);
        }
    }

    public bool FillStep()
    {
        bool movedPiece = false;

        for (int y = height-2; y >= 0; y--) 
        {
            for (int loopX = 0; loopX < width; loopX++) 
            {
                int x = loopX;
                if (inverse) 
                {
                    x = width - 1 - loopX;
                }
                Gem gem = gridArray[x, y];

                if (gem.IsMovable()) 
                {
                    Gem gemBelow = gridArray[x, y + 1];
                    if (gemBelow.GetPieceType() == PieceType.EMPTY) 
                    {
                        Destroy(gemBelow.gameObject);
                        gem.MoveGem.Move(x, y + 1, fillTime);
                        gridArray[x, y + 1] = gem;
                        SpawnNewGem(x, y, PieceType.EMPTY);
                        movedPiece = true;
                    }
                    else
                    {
                        for (int diag = -1; diag <= 1; diag++) 
                        {
                            if (diag != 0) 
                            {
                                int diagX = x + diag;
                                if (inverse) 
                                {
                                    diagX = x - diag;
                                }
                                if (diagX >= 0 && diagX < width) 
                                {
                                    Gem diagonalGem = gridArray[diagX, y + 1];
                                    if (diagonalGem.GetPieceType() == PieceType.EMPTY) 
                                    {
                                        bool hasPieceAbove = true;
                                        for (int aboveY = y; aboveY >= 0; aboveY--) 
                                        {
                                            Gem aboveGem = gridArray[diagX, aboveY];
                                            if (aboveGem.IsMovable()) 
                                            {
                                                break;
                                            }
                                            else if (!aboveGem.IsMovable() && aboveGem.GetPieceType() == PieceType.EMPTY) 
                                            {
                                                hasPieceAbove = false;
                                                break;
                                            }
                                        }
                                        if (!hasPieceAbove) 
                                        {
                                            Destroy(diagonalGem.gameObject);
                                            gem.MoveGem.Move(diagX, y + 1, fillTime);
                                            gridArray[diagX, y + 1] = gem;
                                            SpawnNewGem(x, y, PieceType.EMPTY);
                                            movedPiece = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        for (int x = 0; x < width; x++) 
        {
            Gem gemBelow = gridArray[x, 0];

            if (gemBelow.GetPieceType() == PieceType.EMPTY) 
            {
                Destroy(gemBelow.gameObject);
                GameObject newGem = (GameObject)Instantiate(piecePrefabDictionary[PieceType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity);
                newGem.transform.parent = transform;

                gridArray[x, 0] = newGem.GetComponent<Gem>();
                gridArray[x, 0].Create(x, -1, this, PieceType.NORMAL);
                gridArray[x, 0].MoveGem.Move(x, 0, fillTime);
                gridArray[x, 0].ColorGem.SetColor((ColorGem.ColorType)Random.Range(0, gridArray[x, 0].ColorGem.NumColors));
                movedPiece = true;
            }
        }

        return movedPiece;
    }

    private void FillGrid()
    {
        for (int i = 0; i < gemPositions.Length; i++) 
        {
            if (gemPositions[i].x >= 0 && gemPositions[i].x < width 
                && gemPositions[i].y >= 0 && gemPositions[i].y < height) 
            {
                SpawnNewGem(gemPositions[i].x, gemPositions[i].y, gemPositions[i].type);
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (gridArray[x, y] == null)
                {
                    SpawnNewGem(x, y, PieceType.EMPTY);
                }
            }
        }
    }

    public Gem SpawnNewGem(int x, int y, PieceType type) 
    {
        GameObject newGem = (GameObject)Instantiate(piecePrefabDictionary[type], GetWorldPosition(x, y), Quaternion.identity, this.transform);
        newGem.transform.parent = transform;
        gridArray[x, y] = newGem.GetComponent<Gem>();
        gridArray[x, y].Create(x, y, this, type);
        return gridArray[x, y];
    }

    public Level GetLevel() 
    {
        return level;
    }

    public bool IsAdjacent(Gem gem1, Gem gem2) 
    {
        return (gem1.GetX() == gem2.GetX() && (int)Mathf.Abs(gem1.GetY() - gem2.GetY()) == 1) 
            || (gem1.GetY() == gem2.GetY() && (int)Mathf.Abs(gem1.GetX() - gem2.GetX()) == 1);
    }

    public void SwapGems(Gem gem1, Gem gem2) 
    {
        if (gameOver) 
        {
            return;
        }

        if (gem1.IsMovable() && gem2.IsMovable()) 
        {
            gridArray[gem1.GetX(), gem1.GetY()] = gem2;
            gridArray[gem2.GetX(), gem2.GetY()] = gem1;
            if (GetMatch(gem1, gem2.GetX(), gem2.GetY()) != null || GetMatch(gem2, gem1.GetX(), gem1.GetY()) != null
                || gem1.GetPieceType() == PieceType.RAINBOW || gem2.GetPieceType() == PieceType.RAINBOW)
            {
                int gem1X = gem1.GetX();
                int gem1Y = gem1.GetY();
                gem1.MoveGem.Move(gem2.GetX(), gem2.GetY(), fillTime);
                gem2.MoveGem.Move(gem1X, gem1Y, fillTime);
                if (gem1.GetPieceType() == PieceType.RAINBOW && gem1.IsClearable() && gem2.IsColored()) 
                {
                    ClearColorGem clearColor = gem1.GetComponent<ClearColorGem>();
                    if (clearColor) 
                    {
                        clearColor.Color = gem2.ColorGem.Color;
                    }
                    ClearGem(gem1.GetX(), gem1.GetY());
                }
                if (gem2.GetPieceType() == PieceType.RAINBOW && gem2.IsClearable() && gem1.IsColored())
                {
                    ClearColorGem clearColor = gem2.GetComponent<ClearColorGem>();
                    if (clearColor)
                    {
                        clearColor.Color = gem1.ColorGem.Color;
                    }
                    ClearGem(gem2.GetX(), gem2.GetY());
                }
                ClearAllValidMatches();
                if (gem1.GetPieceType() == PieceType.ROW_CLEAR || gem1.GetPieceType() == PieceType.COLUMN_CLEAR) 
                {
                    ClearGem(gem1.GetX(), gem1.GetY());
                }
                if (gem2.GetPieceType() == PieceType.ROW_CLEAR || gem2.GetPieceType() == PieceType.COLUMN_CLEAR)
                {
                    ClearGem(gem2.GetX(), gem2.GetY());
                }
                pressedGem = null;
                enteredGem = null;
                StartCoroutine(FillWithGems());
                level.OnMove();
            }
            else 
            {
                gridArray[gem1.GetX(), gem1.GetY()] = gem1;
                gridArray[gem2.GetX(), gem2.GetY()] = gem2;
            }
        }
    }

    public void PressGem(Gem _gem) 
    {
        pressedGem = _gem;
    }

    public void EnterGem(Gem _gem) 
    {
        enteredGem = _gem;
    }

    public void ReleaseGem() 
    {
        if (IsAdjacent(pressedGem, enteredGem)) 
        {
            SwapGems(pressedGem, enteredGem);
        }
    }

    public void ClearObstacles(int x, int y) 
    {
        for (int adjacentX = x - 1; adjacentX <= x + 1; adjacentX++) 
        {
            if (adjacentX != x && adjacentX >= 0 && adjacentX < width) 
            {
                if (gridArray[adjacentX, y].GetPieceType() == PieceType.UNMOVABLE && gridArray[adjacentX, y].IsClearable()) 
                {
                    gridArray[adjacentX, y].ClearableGem.ClearGem();
                    SpawnNewGem(adjacentX, y, PieceType.EMPTY);
                }
            }
        }

        for (int adjacentY = y - 1; adjacentY <= y + 1; adjacentY++)
        {
            if (adjacentY != y && adjacentY >= 0 && adjacentY < height)
            {
                if (gridArray[x, adjacentY].GetPieceType() == PieceType.UNMOVABLE && gridArray[x, adjacentY].IsClearable())
                {
                    gridArray[x, adjacentY].ClearableGem.ClearGem();
                    SpawnNewGem(x, adjacentY, PieceType.EMPTY);
                }
            }
        }
    }

    public List<Gem> GetMatch(Gem gem, int newX, int newY) 
    {
        if (gem.IsColored()) 
        {
            ColorGem.ColorType color = gem.ColorGem.Color;
            List<Gem> horizontalGems = new List<Gem>();
            List<Gem> verticalGems = new List<Gem>();
            List<Gem> matchingGems = new List<Gem>();
            horizontalGems.Add(gem);

            for (int dir = 0; dir <= 1; dir++) 
            {
                for (int xOffset = 1; xOffset < width; xOffset++) 
                {
                    int x;
                    if (dir == 0) 
                    {
                        x = newX - xOffset;
                    }
                    else 
                    {
                        x = newX + xOffset;
                    }

                    if (x < 0 || x >= width) 
                    {
                        break;
                    }

                    if (gridArray[x, newY].IsColored() && gridArray[x, newY].ColorGem.Color == color) 
                    {
                        horizontalGems.Add(gridArray[x, newY]);
                    }
                    else
                    {
                        break;
                    }
                }

                if (horizontalGems.Count >= matchGems) 
                {
                    for (int i = 0; i < horizontalGems.Count; i++) 
                    {
                        matchingGems.Add(horizontalGems[i]);
                    }
                }

                if (horizontalGems.Count > matchGems) 
                {
                    for (int i = 0; i < horizontalGems.Count; i++) 
                    {
                        for (int dire = 0; dire <= 1; dire++) 
                        {
                            for (int yOffset = 1; yOffset < height; yOffset++) 
                            {
                                int y;

                                if (dire == 0) 
                                {
                                    y = newY - yOffset;
                                }
                                else 
                                {
                                    y = newY + yOffset;
                                }

                                if (y < 0 || y >= width)
                                {
                                    break;
                                }
                                if (gridArray[horizontalGems[i].GetX(), y].IsColored() && gridArray[horizontalGems[i].GetX(), y].ColorGem.Color == color) 
                                {
                                    verticalGems.Add(gridArray[horizontalGems[i].GetX(), y]);
                                }
                                else 
                                {
                                    break;
                                }
                            }
                        }
                        if (verticalGems.Count < matchGems - 1) 
                        {
                            verticalGems.Clear();
                        }
                        else 
                        {
                            for (int j = 0; j < verticalGems.Count; j++) 
                            {
                                matchingGems.Add(verticalGems[j]);
                            }
                            break;
                        }
                    }
                }

                if (matchingGems.Count >= matchGems) 
                {
                    return matchingGems;
                }
            }

            horizontalGems.Clear();
            verticalGems.Clear();
            verticalGems.Add(gem);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < height; yOffset++)
                {
                    int y;
                    if (dir == 0)
                    {
                        y = newY - yOffset;
                    }
                    else
                    {
                        y = newY + yOffset;
                    }

                    if (y < 0 || y >= width)
                    {
                        break;
                    }

                    if (gridArray[newX, y].IsColored() && gridArray[newX, y].ColorGem.Color == color)
                    {
                        verticalGems.Add(gridArray[newX, y]);
                    }
                    else
                    {
                        break;
                    }
                }

                if (verticalGems.Count >= matchGems)
                {
                    for (int i = 0; i < verticalGems.Count; i++)
                    {
                        matchingGems.Add(verticalGems[i]);
                    }
                }

                if (verticalGems.Count > matchGems)
                {
                    for (int i = 0; i < verticalGems.Count; i++)
                    {
                        for (int dire = 0; dire <= 1; dire++)
                        {
                            for (int xOffset = 1; xOffset < width; xOffset++)
                            {
                                int x;

                                if (dire == 0)
                                {
                                    x = newX - xOffset;
                                }
                                else
                                {
                                    x = newX + xOffset;
                                }

                                if (x < 0 || x >= width)
                                {
                                    break;
                                }
                                if (gridArray[x, verticalGems[i].GetY()].IsColored() && gridArray[x, verticalGems[i].GetY()].ColorGem.Color == color)
                                {
                                    horizontalGems.Add(gridArray[x, verticalGems[i].GetY()]);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        if (horizontalGems.Count < matchGems - 1)
                        {
                            horizontalGems.Clear();
                        }
                        else
                        {
                            for (int j = 0; j < verticalGems.Count; j++)
                            {
                                matchingGems.Add(verticalGems[j]);
                            }
                            break;
                        }
                    }
                }

                if (matchingGems.Count >= matchGems)
                {
                    return matchingGems;
                }
            }
        }
        return null;
    }

    public bool ClearGem(int x, int y) 
    {
        if (gridArray[x, y].IsClearable() && !gridArray[x, y].ClearableGem.IsBeingCleared) 
        {
            gridArray[x, y].ClearableGem.ClearGem();
            SpawnNewGem(x, y, PieceType.EMPTY);
            ClearObstacles(x, y);
            return true;
        }
        return false;
    }

    public void ClearRow(int row) 
    {
        for (int x = 0; x < width; x++) 
        {
            ClearGem(x, row);
        }
    }

    public void ClearColor(ColorGem.ColorType color) 
    {
        for (int x = 0; x < width; x++) 
        {
            for (int y = 0; y < height; y++) 
            {
                if (gridArray[x, y].IsColored() && gridArray[x, y].ColorGem.Color == color || color == ColorGem.ColorType.ANY) 
                {
                    ClearGem(x, y);
                }
            }
        }
    }

    public void ClearColumn(int column) 
    {
        for (int y = 0; y < height; y++) 
        {
            ClearGem(column, y);
        }
    }

    public void GameOver() 
    {
        gameOver = true;
    }

    public bool ClearAllValidMatches() 
    {
        bool needsRefill = false;
        for (int y = 0; y < height; y++) 
        {
            for (int x = 0; x < width; x++) 
            {
                if (gridArray[x, y].IsClearable()) 
                {
                    List<Gem> match = GetMatch(gridArray[x, y], x, y);
                    if (match != null) 
                    {
                        PieceType specialPieceType = PieceType.COUNT;
                        Gem randomGem = match[Random.Range(0, match.Count)];
                        int specialGemX = randomGem.GetX();
                        int specialGemY = randomGem.GetY();
                        if (match.Count == 4) 
                        {
                            if (pressedGem == null || enteredGem == null) 
                            {
                                specialPieceType = (PieceType)Random.Range((int)PieceType.ROW_CLEAR, (int)PieceType.COLUMN_CLEAR);
                            }
                            else if (pressedGem.GetY() == enteredGem.GetY()) 
                            {
                                specialPieceType = PieceType.ROW_CLEAR;
                            }
                            else
                            {
                                specialPieceType = PieceType.COLUMN_CLEAR;
                            }
                        }
                        else if (match.Count >= 5) 
                        {
                            specialPieceType = PieceType.RAINBOW;
                        }
                        for (int i = 0; i < match.Count; i++) 
                        {
                            if (ClearGem(match[i].GetX(), match[i].GetY())) 
                            {
                                needsRefill = true;
                                if (match[i] == pressedGem || match[i] == enteredGem) 
                                {
                                    specialGemX = match[i].GetX();
                                    specialGemY = match[i].GetY();
                                }
                            }
                        }
                        if (specialPieceType != PieceType.COUNT) 
                        {
                            Destroy(gridArray[specialGemX, specialGemY].gameObject);
                            Gem newGem = SpawnNewGem(specialGemX, specialGemY, specialPieceType);
                            if ((specialPieceType == PieceType.ROW_CLEAR || specialPieceType == PieceType.COLUMN_CLEAR) 
                                 && newGem.IsColored() && match[0].IsColored()) 
                            {
                                newGem.ColorGem.SetColor(match[0].ColorGem.Color);
                            }
                            else if (specialPieceType == PieceType.RAINBOW && newGem.IsColored()) 
                            {
                                newGem.ColorGem.SetColor(ColorGem.ColorType.ANY);
                            }
                        } 
                    }
                }
            }
        }
        return needsRefill;
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

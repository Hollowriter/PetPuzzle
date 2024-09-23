using System.Runtime.InteropServices;
using UnityEngine;

public class Gem : MonoBehaviour
{
    private int x;
    private int y;
    private GridManager.PieceType pieceType;
    private GridManager gridManager;
    private MoveGem moveGem;
    private ColorGem colorGem;

    public int GetX() 
    {
        return x;
    }

    public int GetY() 
    {
        return y;
    }

    public void SetX(int _x)
    {
        if (IsMovable())
            x = _x;
    }

    public void SetY(int _y) 
    {
        if (IsMovable())
            y = _y;
    }

    public MoveGem MoveGem 
    {
        get { return moveGem; }
    }

    public ColorGem ColorGem 
    {
        get { return colorGem; }
    }

    public GridManager.PieceType GetPieceType() 
    {
        return pieceType;
    }

    public GridManager GetGridManager() 
    {
        return gridManager;
    }

    public void Create(int _x, int _y, GridManager.PieceType _pieceType) 
    {
        x = _x;
        y = _y;
        pieceType = _pieceType;
    }

    private void Awake()
    {
        moveGem = GetComponent<MoveGem>();
        colorGem = GetComponent<ColorGem>();
        gridManager = FindObjectOfType<GridManager>();
    }

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
    }

    public bool IsMovable() 
    {
        return moveGem != null;
    }

    public bool IsColored() 
    {
        return colorGem != null;
    }
}

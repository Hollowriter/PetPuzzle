using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public enum LevelType 
    {
        TIMER,
        OBSTACLE,
        MOVES
    }

    protected LevelType type;
    protected int currentScore;
    public GridManager gridManager;
    [SerializeField] private int score1Star;
    [SerializeField] private int score2Star;
    [SerializeField] private int score3Star;
    public LevelType Type { get { return type; } }

    public virtual void GameWin() 
    {
    }

    public virtual void GameLose() 
    {
    }

    public virtual void OnMove() 
    { 
    }

    public virtual void OnGemCleared(Gem gem) 
    {
    }
}
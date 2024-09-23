using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGem : MonoBehaviour
{
    private Gem gem;

    private void Awake()
    {
        gem = GetComponent<Gem>();
    }

    public void Move(int newX, int newY) 
    {
        gem.SetX(newX);
        gem.SetY(newY);
        gem.transform.localPosition = gem.GetGridManager().GetWorldPosition(newX, newY);
    }
}

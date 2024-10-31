using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearLineGem : ClearableGem
{

    public bool isRow;

    public override void ClearGem()
    {
        base.ClearGem();
        if (isRow) 
        {
            gem.GetGridManager().ClearRow(gem.GetY());
        }
        else 
        {
            gem.GetGridManager().ClearColumn(gem.GetX());
        }
    }
}

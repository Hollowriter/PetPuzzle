using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearColorGem : ClearableGem
{
    private ColorGem.ColorType color;
    public ColorGem.ColorType Color 
    {
        get { return color; }
        set { color = value; }
    }

    public override void ClearGem()
    {
        base.ClearGem();
        gem.GetGridManager().ClearColor(color);
    }
}

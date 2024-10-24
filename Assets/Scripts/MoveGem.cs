using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGem : MonoBehaviour
{
    private Gem gem;
    private IEnumerator moveCoroutine;

    private void Awake()
    {
        gem = GetComponent<Gem>();
    }

    public void Move(int newX, int newY, float time) 
    {
        if (moveCoroutine != null) 
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = MoveCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutine);
    }

    private IEnumerator MoveCoroutine(int newX, int newY, float time) 
    {
        gem.SetX(newX);
        gem.SetY(newY);
        Vector3 startPos = transform.localPosition;
        Vector3 endPos = gem.GetGridManager().GetWorldPosition(newX, newY);
        for (float t = 0; t <= 1 * time; t += Time.deltaTime) 
        {
            gem.transform.localPosition = Vector3.Lerp(startPos, endPos, t / time);
            yield return 0;
        }
        gem.transform.localPosition = endPos;
        if (gem.GetPieceType() == GridManager.PieceType.ROW_CLEAR || gem.GetPieceType() == GridManager.PieceType.COLUMN_CLEAR)
        {
            Debug.Log("sussex" + endPos + "startpos" + startPos);
        }
    }
}

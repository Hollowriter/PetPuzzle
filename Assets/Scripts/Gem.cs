using System.Runtime.InteropServices;
using UnityEngine;

public class Gem : MonoBehaviour
{
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private GridManager gridManager;
    private Vector2 gemPosition;

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        gemPosition = transform.position;
    }

    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        DetectSwipe();
    }

    private void DetectSwipe()
    {
        float swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;

        if (swipeAngle > -45 && swipeAngle <= 45 && gemPosition.x < gridManager.GetWidth() - 1) // Swipe Right
        {
            gridManager.SwapGems(gemPosition, new Vector2(gemPosition.x + 1, gemPosition.y));
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && gemPosition.y < gridManager.GetHeight() - 1) // Swipe Up
        {
            gridManager.SwapGems(gemPosition, new Vector2(gemPosition.x, gemPosition.y + 1));
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && gemPosition.x > 0) // Swipe Left
        {
            gridManager.SwapGems(gemPosition, new Vector2(gemPosition.x - 1, gemPosition.y));
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && gemPosition.y > 0) // Swipe Down
        {
            gridManager.SwapGems(gemPosition, new Vector2(gemPosition.x, gemPosition.y - 1));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGem : MonoBehaviour
{
    public enum ColorType 
    {
        BLUE,
        GREEN,
        RED,
        WHITE,
        ORANGE,
        PURPLE,
        YELLOW,
        ANY,
        COUNT
    }

    [System.Serializable]
    public struct ColorSprite 
    {
        public ColorType color;
        public Sprite sprite;
    };

    [SerializeField] private ColorSprite[] colorSprites;
    private Dictionary<ColorType, Sprite> colorSpriteDictionary;
    private ColorType color;
    public ColorType Color 
    {
        get { return color; }
        set { SetColor(value); }
    }
    public int NumColors 
    {
        get { return colorSprites.Length; }
    }

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        colorSpriteDictionary = new Dictionary<ColorType, Sprite>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        for (int i = 0; i < colorSprites.Length; i++)
        {
            if (!colorSpriteDictionary.ContainsKey(colorSprites[i].color))
            {
                colorSpriteDictionary.Add(colorSprites[i].color, colorSprites[i].sprite);
            }
        }
    }

    public void SetColor(ColorType newColor) 
    {
        color = newColor;
        if (colorSpriteDictionary.ContainsKey(newColor)) 
        {
            spriteRenderer.sprite = colorSpriteDictionary[newColor];
        }
    }
}

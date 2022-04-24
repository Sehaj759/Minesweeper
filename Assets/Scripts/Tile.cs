using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    enum SpriteName
    {
        EMPTY = 0,
        BOMB = 9,
        BOMB_EXPLODED,
        FLAG_1,
        QUESTION_1,
        UNKNOWN_2
    }

    List<string> spriteMap = new List<string>()
        {
            "empty",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "bomb",
            "bomb_exploded",
            "flag_1",
            "question_1",
            "unknown_2"
        };

    SpriteRenderer spriteRenderer;
    int spriteIndex = (int)SpriteName.EMPTY;
    int rightClickSpriteIndex = (int)SpriteName.UNKNOWN_2;
    bool isOpen = false;
    bool isProtected = false;
    bool isBomb = false;

    public bool IsProtected { get => isProtected; set => isProtected = value; }

    public bool IsBomb { get => isBomb; }

    public bool IsOpen { get => isOpen; }

    public void SetAsBomb()
    {
        isBomb = true;
        SpriteIndex = (int)SpriteName.BOMB;
    }

    public int SpriteIndex
    {
        get => spriteIndex;
        set
        {
            if (value >= 0 && value < spriteMap.Count)
            {
                spriteIndex = value;
            }
        }
    }

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        SetSprite(false);
    }

    private void SetSprite(bool openTile = true)
    {
        if (openTile)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/" + spriteMap[SpriteIndex]);
        }
        else
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/" + spriteMap[rightClickSpriteIndex]);
        }
    }

    public void CycleRightClick()
    {
        if (!isOpen)
        {
            rightClickSpriteIndex = (int)SpriteName.FLAG_1 + (rightClickSpriteIndex - (int)SpriteName.FLAG_1 + 1) % 3;
            IsProtected = rightClickSpriteIndex != (int)SpriteName.UNKNOWN_2;
            SetSprite(false);
        }
    }

    public void Open()
    {
        if (!isOpen && !isProtected)
        {
            if (isBomb)
            {
                SpriteIndex = (int)SpriteName.BOMB_EXPLODED;
            }
            SetSprite();
            isOpen = true;
        }
    }
}

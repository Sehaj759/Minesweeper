using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    [SerializeField] int nRows = 6;
    [SerializeField] int nCols = 6;
    [SerializeField] int nBombs = 10;
    [SerializeField] Tile tilePrefab;

    float tileSize = 1.28f;
    Vector3 topLeft;
    Vector3 bottomRight;

    List<List<Tile>> tiles;

    bool gameWon = false;
    bool gameOver = false;
    bool boardIsInitialized = false;

    int totalTiles;
    int openedTiles;

    void Start()
    {
        if (tilePrefab && nRows > 0 && nCols > 0)
        {
            totalTiles = nRows * nCols - nBombs;
            openedTiles = 0;

            tiles = new List<List<Tile>>(nRows);

            topLeft = new Vector3(-(nCols / 2.0f) * tileSize, (nRows / 2.0f) * tileSize, 0);
            bottomRight = topLeft + (new Vector3(nCols * tileSize, -nRows * tileSize, 0));

            Vector3 pos = topLeft;
            
            for(int row = 0; row < nRows; ++row)
            {
                List<Tile> curRow = new List<Tile>(nCols);
                for(int col = 0; col < nCols; ++col)
                {
                    Tile tile = Instantiate(tilePrefab, pos, Quaternion.identity);
                    curRow.Add(tile);

                    pos.x += tileSize;
                }

                tiles.Add(curRow);

                pos.x = topLeft.x;
                pos.y -= tileSize;
            }
        }
    }

    void Update()
    {
        if (gameWon)
        {
            Debug.Log("Game Won");
        }
        else if (!gameOver)
        {
            if (Input.GetMouseButtonDown(0))
            {
                int row;
                int col;
                if (MousePosToIndex(Camera.main.ScreenToWorldPoint(Input.mousePosition), out row, out col))
                {
                    if (tiles[row][col].IsBomb)
                    {
                        Debug.Log("Game Over");
                        tiles[row][col].Open();
                        gameOver = true;
                    }
                    else
                    {
                        if (!boardIsInitialized)
                        {
                            InitializeBombs(row, col);
                        }
                        OpenTiles(row, col);
                    }
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                int row;
                int col;
                if (MousePosToIndex(Camera.main.ScreenToWorldPoint(Input.mousePosition), out row, out col))
                {
                    tiles[row][col].CycleRightClick();
                }
            }
        }
    }

    void InitializeBombs(int ignoreRow, int ignoreCol)
    {
        int bombs = 0;
        while (bombs < nBombs)
        {
            int row;
            int col;
            do
            {
                row = Random.Range(0, nRows);
                col = Random.Range(0, nCols);
            } while (tiles[row][col].IsBomb || (row == ignoreRow && col == ignoreCol));

            tiles[row][col].SetAsBomb();

            ++bombs;
        }

        boardIsInitialized = true;
    }

    bool InBounds(int row, int col)
    {
        return row >= 0 && row < nRows && col >= 0 && col < nCols;
    }
    void OpenTiles(int row, int col)
    {
        if (!InBounds(row, col) || tiles[row][col].IsBomb || tiles[row][col].IsOpen)
            return;

        int bombs = 0;
        for(int i = row - 1; i <= row + 1; ++i)
        {
            for(int j = col - 1; j <= col + 1; ++j)
            {
                bombs = InBounds(i, j) && tiles[i][j].IsBomb ? bombs + 1 : bombs;
            }
        }

        tiles[row][col].SpriteIndex = bombs;
        tiles[row][col].Open();
        ++openedTiles;
        if (openedTiles == totalTiles)
        {
            gameWon = true;
        }
        else if (bombs == 0)
        {
            for (int i = row - 1; i <= row + 1; ++i)
            {
                for (int j = col - 1; j <= col + 1; ++j)
                {
                    OpenTiles(i, j);
                }
            }
        }
    }

    bool MousePosToIndex(Vector3 mousePos, out int row, out int col)
    {
        float x = mousePos.x;
        float y = mousePos.y;
        if(x < topLeft.x || x > bottomRight.x || y > topLeft.y || y < bottomRight.y)
        {
            row = -1;
            col = -1;
            return false;
        }

        row = (int)( (topLeft.y - y) / tileSize );
        col = (int)( (x - topLeft.x) / tileSize );

        return true;
    }
}

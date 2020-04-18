using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirusSweeper : MonoBehaviour
{
    public int boardSize = 8;
    public int virusCount = 10;

    public Slider timerBar;
    public Image fillImage;
    public float timerSpeed = 0.03f;
    float timer;

    public GameObject cellPrefab;

    VirusSweeperCell[] cells;
    int[,] board;
    bool[,] revealed;

    bool firstMove;

    private void Awake()
    {
        timer = 1;
        GenerateBoard();
    }

    private void Update()
    {
        timer -= Time.deltaTime * GameManager.inst.globalSpeed * timerSpeed;

        timerBar.value = timer;

        if (timer <= 0)
        {
            Lose();
        }
    }

    void GenerateBoard()
    {
        board = new int[boardSize, boardSize];
        revealed = new bool[boardSize, boardSize];
        
        for (int i = 0; i < virusCount; i++)
        {
            int x = Random.Range(0, boardSize);
            int y = Random.Range(0, boardSize);

            if (board[x, y] == -1) i--;
            else board[x, y] = -1;
        }

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (board[x, y] == -1) continue;
                
                board[x, y] = CalculateNumber(x, y);
            }
        }

        cells = new VirusSweeperCell[boardSize * boardSize];

        for (int i = 0; i < cells.Length; i++)
        {
            GameObject cellGO = Instantiate(cellPrefab, transform);
            VirusSweeperCell cell = cellGO.GetComponent<VirusSweeperCell>();
            cell.index = i;
            cell.SetController(this);
            cells[i] = cell;
        }

        firstMove = true;
    }

    int CalculateNumber(int x, int y)
    {
        if (x < 0 || x >= boardSize || y < 0 || y >= boardSize) return 0;

        int c = 0;
        for (int fx = -1; fx <= 1; fx++)
        {
            for (int fy = -1; fy <= 1; fy++)
            {
                int ox = fx + x;
                int oy = fy + y;
                if (ox >= 0 && ox < boardSize && oy >= 0 && oy < boardSize)
                {
                    if (board[ox, oy] == -1) c++;
                }
            }
        }
        return c;
    }

    public void RevealCell(int i)
    {
        int x = i % boardSize;
        int y = i / boardSize;

        Reveal(x, y);

        CheckForWin();
    }

    void Reveal(int x, int y)
    {
        if (x >= 0 && x < boardSize && y >= 0 && y < boardSize)
        {
            if (revealed[x, y]) return;

            if (firstMove)
            {
                board[x, y] = 0;
                for (int fx = -1; fx <= 1; fx++)
                {
                    for (int fy = -1; fy <= 1; fy++)
                    {
                        RemoveMine(fx + x, fy + y);
                    }
                }
                RecalculateBoard();
                firstMove = false;
            }

            revealed[x, y] = true;

            int i = x + y * boardSize;

            cells[i].SetNumber(board[x, y]);

            if (board[x, y] == -1)
            {
                Lose();
            }
            else if (board[x, y] == 0)
            {
                Reveal(x + 1, y    );
                Reveal(x + 1, y + 1);
                Reveal(x    , y + 1);
                Reveal(x - 1, y + 1);
                Reveal(x - 1, y    );
                Reveal(x - 1, y - 1);
                Reveal(x    , y - 1);
                Reveal(x + 1, y - 1);
            }
        }
    }

    void RemoveMine(int x, int y)
    {
        if (x < 0 || x >= boardSize || y < 0 || y >= boardSize) return;

        board[x, y] = 0;
    }

    void RecalculateBoard()
    {
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (board[x, y] == -1) continue;

                board[x, y] = CalculateNumber(x, y);
            }
        }
    }

    void CheckForWin()
    {
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (board[x, y] != -1 && !revealed[x, y]) return;
            }
        }

        Win();
    }

    void ResetBoard()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        timer = 1;
        GenerateBoard();
    }

    void Lose()
    {
        GameManager.inst.RandomEvent();

        fillImage.color = Color.red;
        StartCoroutine(ResetCoroutine());
    }

    void Win()
    {
        fillImage.color = Color.green;
        StartCoroutine(ResetCoroutine());
    }

    IEnumerator ResetCoroutine()
    {
        yield return new WaitForSeconds(1f);

        fillImage.color = Color.white;
        ResetBoard();
    }
}

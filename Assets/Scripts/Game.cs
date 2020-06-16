using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public static int gridWidth = 10;
    public static int gridHeight = 20;

    public static float fallSpeed = 1.0f;

    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    private int scoreOneLine = 40;
    private int scoreTwoLine = 100;
    private int scoreThreeLine = 300;
    private int scoreFourLine = 1200;
    private int numberOfRowsThisTurn = 0;

    public static int currentLevel = 1;
    private static int numLinesCleared = 0;

    public Text hud_score, hud_line, hud_level;
    public static int currentScore = 0;
    private GameObject previewTetromino;
    private GameObject nextTetromino;
    private static bool gameStarted = false;

    public static void ReTry()
    {
        currentLevel = 1;
        numLinesCleared = 0;
        currentScore = 0;
        gameStarted = false;
    }

    private Vector2 previewTetrominoPosition = new Vector2(-6.7f, 16f);

    public void UpdateLevel()
    {
        currentLevel = 1 + (numLinesCleared >> 1);
        //Debug.Log("Level: " + currentLevel);
    }

    public void UpdateSpeed()
    {
        fallSpeed = (float) (1.0 / (currentLevel*currentLevel));
        //Debug.Log("FallSpeed: " + fallSpeed);
    }

    public void UpdateScore()
    {
        if (numberOfRowsThisTurn > 0)
        {
            if (numberOfRowsThisTurn == 1)
            {
                currentScore += scoreOneLine;
                numLinesCleared++;
            }
            else if (numberOfRowsThisTurn == 2)
            {
                currentScore += scoreTwoLine;
                numLinesCleared += 2;
            }
            else if (numberOfRowsThisTurn == 3)
            {
                currentScore += scoreThreeLine;
                numLinesCleared += 3;
            }
            else if (numberOfRowsThisTurn == 4)
            {
                currentScore += scoreFourLine;
                numLinesCleared += 4;
            }
            numberOfRowsThisTurn = 0;
        }
    }

    public void UpdateUI()
    {
        hud_score.text = currentScore.ToString();
        hud_level.text = currentLevel.ToString();
        hud_line.text = numLinesCleared.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnNextTetromino();
    }

    public bool CheckIsAboveGrid (Tetromino tetromino)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            foreach (Transform mino in tetromino.transform)
            {
                Vector2 pos = Round(mino.position);
                if (pos.y > gridHeight - 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScore();
        UpdateUI();
        UpdateLevel();
        UpdateSpeed();
    }

    public bool IsFullRowAt (int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }
        numberOfRowsThisTurn++;
        return true;
    }

    public void DeleteMinoAt(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    public void MoveRowDown(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public void MoveAllRowDown (int y)
    {
        for (int i = y; i < gridHeight;++i)
        {
            MoveRowDown(i);
        }
    }
    
    public bool DeleteRow()
    {
        bool check = false;
        for (int y = 0; y < gridHeight; ++y)
        {
            if (IsFullRowAt(y))
            {
                DeleteMinoAt(y);
                MoveAllRowDown(y + 1);
                --y;
                check = true;
            }
        }
        return check;
    }

    public void UpdateGrid (Tetromino tetromino)
    {
        for (int y = 0; y < gridHeight; ++y)
        {
            for (int x = 0; x < gridWidth; ++x)
            {
                if (grid[x,y] != null)
                {
                    if (grid[x,y].parent == tetromino.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }
        foreach (Transform mino in tetromino.transform)
        {
            Vector2 pos = Round(mino.position);
            if (pos.y < gridHeight)
            {
                grid[(int)pos.x, (int)pos.y] = mino;
            }
        }
    }

    public Transform GetTransformAtGridPosition (Vector2 pos)
    {
        if (pos.y > gridHeight - 1)
        {
            return null;
        }
        else
        {
            return grid[(int)pos.x, (int)pos.y];
        }
    }

    public void SpawnNextTetromino()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector2(5.0f, 20.0f), Quaternion.identity);
            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            previewTetromino.GetComponent<Tetromino>().enabled = false;
        }
        else
        {
            previewTetromino.transform.localPosition = new Vector2(5.0f, 20.0f);
            nextTetromino = previewTetromino;
            nextTetromino.GetComponent<Tetromino>().enabled = true;
            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            previewTetromino.GetComponent<Tetromino>().enabled = false;
        }
    }

    public bool CheckIsInsideGrid(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);

    }
    public Vector2 Round (Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    string GetRandomTetromino ()
    {
        int randomTetromino = Random.Range(1, 8);
        string randomTetrominoName = "Prefabs/Tetromino_T";
        switch (randomTetromino)
        {
            case 1:
                randomTetrominoName = "Prefabs/Tetromino_T";
                break;
            case 2:
                randomTetrominoName = "Prefabs/Tetromino_Long";
                break;
            case 3:
                randomTetrominoName = "Prefabs/Tetromino_Square";
                break;
            case 4:
                randomTetrominoName = "Prefabs/Tetromino_J";
                break;
            case 5:
                randomTetrominoName = "Prefabs/Tetromino_L";
                break;
            case 6:
                randomTetrominoName = "Prefabs/Tetromino_S";
                break;
            case 7:
                randomTetrominoName = "Prefabs/Tetromino_Z";
                break;
        }
        return randomTetrominoName;
    }

    public void GameOver ()
    {
        //Application.LoadLevel("GameOver");
        PlayerPrefs.SetInt("lastScore", currentScore);
        SceneManager.LoadScene("GameOver",LoadSceneMode.Single);
    }
}

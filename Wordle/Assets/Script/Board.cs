using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class Board : MonoBehaviour {

    private static readonly KeyCode[] SUPPORTED_KEYS = new KeyCode[] {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F,
        KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R,
        KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X,
        KeyCode.Y, KeyCode.Z, KeyCode.Return, KeyCode.Backspace,
    };

    private Row[] rows;

    private string[] solutions;
    private string[] validWords;
    private string word;


    private int rowIndex;
    private int columnIndex;

    [Header("States")]
    public Tile.State normalState;
    public Tile.State correctState;
    public Tile.State wrongSpotState;
    public Tile.State incorrectState;

    public TextMeshProUGUI invalidText;
    public TextMeshProUGUI gameText;
    public Key[] keyButtons;
    public Button newGame;
    public Heart heartManager;

    private void Awake()
    {
        rows = GetComponentsInChildren<Row>();
    }

    private void Start()
    {
        LoadData();

        foreach (Key key in keyButtons)
        {
            key.OnKeyPressed += Key_OnKeyPressed;
        }

        enabled = false;
    }

    private void Key_OnKeyPressed(object sender, KeyCode e)
    {
        if(enabled) EnterChar(e);
    }

    private void Update()
    {
        if (enabled) 
        {
            for (int i = 0; i < SUPPORTED_KEYS.Length; i++)
            {
                if (Input.GetKeyDown(SUPPORTED_KEYS[i]))
                {
                   
                    EnterChar(SUPPORTED_KEYS[i]);
                    break;
                }
            }
        }
    }

    private void EnterChar(KeyCode key)
    {
        Row currentRow = rows[rowIndex];

        if (key == KeyCode.Backspace)
        {
            columnIndex = Mathf.Max(columnIndex - 1, 0);
            currentRow.tiles[columnIndex].SetLetter('\0');
            currentRow.tiles[columnIndex].SetState(normalState);

            invalidText.gameObject.SetActive(false);
        }

        else if (columnIndex >= currentRow.tiles.Length)
        {
            if (key == KeyCode.Return) SubmitRow(currentRow);
        }
        else
        {           
            currentRow.tiles[columnIndex].SetLetter((char)key);
            columnIndex++;              
        }
    }

    private void LoadData()
    {
        TextAsset textFile = Resources.Load("official_wordle_all") as TextAsset;
        validWords = textFile.text.Split('\n');
        textFile = Resources.Load("official_wordle_common") as TextAsset;
        solutions = textFile.text.Split('\n');
    }

    private void SetRandomWord()
    {
        word = solutions[Random.Range(0, solutions.Length)].ToLower().Trim();
    }

    private void SubmitRow(Row row)
    {
        if (!IsValidWord(row.word))
        {
            invalidText.gameObject.SetActive(true);
            return;
        }
        string remaining = word;

        for (int i = 0; i < row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];

            if (tile.letter == word[i])
            {
                tile.SetState(correctState);

                remaining = remaining.Remove(i, 1);
                remaining = remaining.Insert(i, " ");
            }
            else if (!word.Contains(tile.letter))
            {
                tile.SetState(incorrectState);
            }
        }

        for (int i = 0; i < row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];

            if (tile.state != correctState && tile.state != incorrectState)
            {
                if (remaining.Contains(tile.letter))
                {
                    tile.SetState(wrongSpotState);

                    int index = remaining.IndexOf(tile.letter);
                    remaining = remaining.Remove(index, 1);
                    remaining = remaining.Insert(index, " ");
                }
                else tile.SetState(incorrectState);
            }
        }

        SetKeyboardColor(row);

        rowIndex++;
        columnIndex = 0;

        if (CheckWin(row))
        {
            gameText.text = "You won";
            enabled = false;
        }

        if (rowIndex >= rows.Length)
        {
            gameText.text = "Answer: " + word;
            enabled = false;
        }
    }

    private bool IsValidWord(string word)
    {
        
        for (int i = 0; i < validWords.Length; i++)
        {
            if (validWords[i] == word) return true;

        }
        return false;
    }

    private void SetKeyboardColor(Row row)
    {
        for (int i = 0; i < row.tiles.Length; i++)
        {
            if (row.tiles[i].state == correctState || row.tiles[i].state == wrongSpotState) 
            {
                Key keyButton = FindKey(row.tiles[i].letter);
                keyButton.SetState(correctState);
            }
            else
            {
                Key keyButton = FindKey(row.tiles[i].letter);
                keyButton.SetState(incorrectState);
            }
        }
        
    }
    private Key FindKey(char keyCode)
    {
        foreach(Key key in keyButtons)
        {
            if ((char)key.keyCode == keyCode) return key;
        }

        return null;
    }

    public void NewBoard()
    {

        if(heartManager.currentHeart <= 0)
        {
            gameText.gameObject.SetActive(true);
            gameText.text = "You have no heart";
            return;
        }
        gameText.gameObject.SetActive(false);
        heartManager.LoseHeart();

        rowIndex = 0;
        columnIndex = 0;

        for (int i = 0; i < rows.Length; i++)
        {
            for (int j = 0; j < rows[i].tiles.Length; j++)
            {
                rows[i].tiles[j].SetLetter('\0');
                rows[i].tiles[j].SetState(normalState);
            }
        }

        for (int i = 0; i <keyButtons.Length; i++)
        {
            keyButtons[i].stateSettable = true;
            keyButtons[i].SetState(normalState);
            keyButtons[i].stateSettable = true;
        }

        SetRandomWord();

        enabled = true;

    }

    private bool CheckWin(Row row)
    {
        foreach (Tile tile in row.tiles)
        {
            if (!(tile.state == correctState))
            {
                return false;
            }
        }
        return true;
    }

    private void OnDisable()
    {
        gameText.gameObject.SetActive(true);
        newGame.gameObject.SetActive(true);
    }
    private void OnEnable()
    {
        gameText.gameObject.SetActive(false);
        newGame.gameObject.SetActive(false);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{

    Setup boardSetup;
    
    public enum State
    {
        player1,
        player2
    }

    public State gameState;
    private float columnSize;
    private float startPosX;
    private int columns, rows;

    public Color player1Color, player2Color, highlightedColor, normalColor, player1Win, player2Win;
    private int previouslyHighlighted = -1;

    public GameObject player1Text, player2Text, gameOverText;
    public bool gameIsOver = false;

    void Start()
    {
        boardSetup = FindObjectOfType<Setup>();

        if(!boardSetup || !player1Text || !player2Text || !gameOverText)
        {
            Debug.LogWarning("Elements are missing in the inspector.");
            return;
        }

        gameOverText.SetActive(false);

        gameState = UnityEngine.Random.Range(0, 2) == 0 ? State.player1 : State.player2;
        UpdatePlayerText();

        columns = boardSetup.columns;
        rows = boardSetup.rows;

        columnSize = (boardSetup.width - boardSetup.gap) / columns;
        startPosX = boardSetup.bottomLeft.position.x;
    }

    private void UpdatePlayerText()
    {
        player1Text.SetActive(gameState == State.player1 && !gameIsOver);
        player2Text.SetActive(gameState == State.player2 && !gameIsOver);
    }

    private void OnMouseDown()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int selectedColumn = GetColumn(mousePosition);
        
        if(selectedColumn == -1 || gameIsOver)
        {
            return;
        }

        int startIndex = (selectedColumn - 1) * rows;
        for (int i = startIndex; i < startIndex + rows; i++)
        {
            GameObject coinObject = boardSetup.circles[i];
            Coin coin = coinObject.GetComponent<Coin>();
            if (coin.isEmpty)
            {
                coinObject.GetComponent<SpriteRenderer>().color = gameState == State.player1 ? player1Color : player2Color;

                coin.isEmpty = false;
                coin.isPlayer1 = gameState == State.player1 ? true : false;

                CheckForWin(i, gameState == State.player1 ? true : false);

                gameState = gameState == State.player1 ? State.player2 : State.player1;
                UpdatePlayerText();
                
                break;
            }
        }

    }

    private void OnMouseOver()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int selectedColumn = GetColumn(mousePosition);

        if(selectedColumn < 1 || selectedColumn > columns || gameIsOver)
        {
            return;
        }

        if (previouslyHighlighted != -1)
        {
            HighlightColumn(previouslyHighlighted, normalColor);
        }

        HighlightColumn(selectedColumn, highlightedColor);
        previouslyHighlighted = selectedColumn;

    }

    private void OnMouseExit()
    {
        HighlightColumn(previouslyHighlighted, normalColor);
        previouslyHighlighted = -1;
    }

    private void HighlightColumn(int columnIndex, Color color)
    {
        int startIndex = (columnIndex - 1) * rows;

        for (int i = startIndex; i < startIndex + rows; i++)
        {
            if (i < 0 || i >= rows * columns)
            {
                return;
            }

            GameObject coinObject = boardSetup.circles[i];
            if (coinObject.GetComponent<Coin>().isEmpty)
            {
                coinObject.GetComponent<SpriteRenderer>().color = color;
            }
        }
    }

    private void HighlightWinningCoins(List<int> validCoins)
    {
        foreach (int coinIndex in validCoins)
        {
            GameObject coinObject = boardSetup.circles[coinIndex];
            coinObject.GetComponent<SpriteRenderer>().color = gameState == State.player1 ? player1Win : player2Win;
        }
    }

    private void CheckForWin(int index, bool isPlayer1)
    {
        CheckHorizontally(index, isPlayer1);
        CheckVertically(index, isPlayer1);
        CheckDiagonalLeftToRight(index, isPlayer1);
        CheckDiagonalRightToLeft(index, isPlayer1);
    }

    private void CheckHorizontally(int index, bool isPlayer1)
    {
        int leftCoins = 0;
        int rightCoins = 0;
        int leftIndex = index - 6;
        int rightIndex = index + 6;
        List<int> validCoins = new List<int>
        {
            index
        };

        while (leftIndex >= 0 && leftIndex < rows * columns)
        {
            Coin coin = GetCoin(leftIndex);
            if (!coin.isEmpty && coin.isPlayer1 == isPlayer1)
            {
                leftCoins++;
                validCoins.Add(leftIndex);
            }
            else
            {
                break;
            }

            leftIndex -= 6;
        }

        while (rightIndex >= 0 && rightIndex < rows * columns)
        {
            Coin coin = GetCoin(rightIndex);
            if (!coin.isEmpty && coin.isPlayer1 == isPlayer1)
            {
                rightCoins++;
                validCoins.Add(rightIndex);
            }
            else
            {
                break;
            }

            rightIndex += 6;
        }

        int total = leftCoins + rightCoins + 1;

        if (total >= 4)
        {
            EndOfGame();
            HighlightWinningCoins(validCoins);
            print("Horizontal Win with " + total.ToString() + " coins! " + isPlayer1.ToString());
        }
    }

    private void CheckVertically(int index, bool isPlayer1)
    {
        int bottomCoins = 0;
        int bottomIndex = index - 1;
        List<int> validCoins = new List<int>
        {
            index
        };

        while (bottomIndex >= Mathf.FloorToInt(index / rows) * rows)
        {
            Coin coin = GetCoin(bottomIndex);
            if (!coin.isEmpty && coin.isPlayer1 == isPlayer1)
            {
                bottomCoins++;
                validCoins.Add(bottomIndex);
            }
            else
            {
                break;
            }

            bottomIndex -= 1;
        }

        int total = bottomCoins + 1;

        if (total >= 4)
        {
            EndOfGame();
            HighlightWinningCoins(validCoins);
            print("Vertical Win with " + total.ToString() + " coins! " + isPlayer1.ToString());
        }
    }

    private void CheckDiagonalLeftToRight(int index, bool isPlayer1)
    {
        int leftCoins = 0;
        int rightCoins = 0;
        int leftIndex = index - 7;
        int rightIndex = index + 7;
        int startRowIndex = GetRowOfIndex(index);
        List<int> validCoins = new List<int>
        {
            index
        };

        while (leftIndex >= 0 && GetRowOfIndex(leftIndex) < startRowIndex)
        {
            Coin coin = GetCoin(leftIndex);
            if (!coin.isEmpty && coin.isPlayer1 == isPlayer1)
            {
                leftCoins++;
                validCoins.Add(leftIndex);
            }
            else
            {
                break;
            }

            leftIndex -= 7;
        }

        while (rightIndex < rows * columns && GetRowOfIndex(rightIndex) > startRowIndex)
        {
            Coin coin = GetCoin(rightIndex);
            if (!coin.isEmpty && coin.isPlayer1 == isPlayer1)
            {
                rightCoins++;
                validCoins.Add(rightIndex);
            }
            else
            {
                break;
            }

            rightIndex += 7;
        }

        int total = leftCoins + rightCoins + 1;

        if (total >= 4)
        {
            EndOfGame();
            HighlightWinningCoins(validCoins);
            print("Diagonal (1) Win with " + total.ToString() + " coins! " + isPlayer1.ToString());
        }
    }

    private void CheckDiagonalRightToLeft(int index, bool isPlayer1)
    {
        int leftCoins = 0;
        int rightCoins = 0;
        int leftIndex = index - 5;
        int rightIndex = index + 5;
        int startRowIndex = GetRowOfIndex(index);
        List<int> validCoins = new List<int>
        {
            index
        };

        while (leftIndex >= 0 && GetRowOfIndex(leftIndex) > startRowIndex)
        {
            Coin coin = GetCoin(leftIndex);
            if (!coin.isEmpty && coin.isPlayer1 == isPlayer1)
            {
                leftCoins++;
                validCoins.Add(leftIndex);
            }
            else
            {
                break;
            }

            leftIndex -= 5;
        }

        while (rightIndex < rows * columns && GetRowOfIndex(rightIndex) < startRowIndex)
        {
            Coin coin = GetCoin(rightIndex);
            if (!coin.isEmpty && coin.isPlayer1 == isPlayer1)
            {
                rightCoins++;
                validCoins.Add(rightIndex);
            }
            else
            {
                break;
            }

            rightIndex += 5;
        }

        int total = leftCoins + rightCoins + 1;

        if (total >= 4)
        {
            EndOfGame();
            HighlightWinningCoins(validCoins);
            print("Diagonal (2) Win with " + total.ToString() + " coins! " + isPlayer1.ToString());
        }
    }

    private Coin GetCoin(int index)
    {
        GameObject coinObject = boardSetup.circles[index];
        Coin coin = coinObject.GetComponent<Coin>();
        return coin;
    }

    private int GetRowOfIndex(int index)
    {
        return index % 6 + 1;
    }

    private int GetColumn(Vector2 mousePosition)
    {
        for (int i = 1; i <= columns; i++)
        {
            if (mousePosition.x < -0.5f * columnSize + (-3 + i) * columnSize)
            {
                return i;
            }
        }

        return -1;
    }

    private void EndOfGame()
    {
        gameIsOver = true;
        HighlightColumn(previouslyHighlighted, normalColor);

        gameOverText.SetActive(true);
        string winner = gameState == State.player1 ? "Player 1" : "Player 2";
        gameOverText.GetComponent<TextMeshProUGUI>().text = "Congratulations to " + winner + "!";
    }


}

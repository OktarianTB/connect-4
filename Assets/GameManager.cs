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
        player2,
        gameOver
    }

    public State gameState;
    private float columnSize;
    private float startPosX;
    private int columns, rows;

    public Color player1Color, player2Color, highlightedColor, normalColor;
    private int previouslyHighlighted = -1;

    public GameObject player1Text, player2Text;

    void Start()
    {
        boardSetup = FindObjectOfType<Setup>();

        gameState = UnityEngine.Random.Range(0, 2) == 0 ? State.player1 : State.player2;
        UpdatePlayerText();

        columns = boardSetup.columns;
        rows = boardSetup.rows;

        columnSize = (boardSetup.width - boardSetup.gap) / columns;
        startPosX = boardSetup.bottomLeft.position.x;
    }

    private void UpdatePlayerText()
    {
        player1Text.SetActive(gameState == State.player1);
        player2Text.SetActive(gameState == State.player2);
    }

    private void OnMouseDown()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int selectedColumn = GetColumn(mousePosition);

        if(selectedColumn == -1)
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

        if(selectedColumn <= 0 || selectedColumn > columns)
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
            if (!boardSetup.circles[i])
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

    private int GetColumn(Vector2 mousePosition)
    {
        for (int i = 1; i <= columns; i++)
        {
            if(mousePosition.x < -0.5f * columnSize + (-3+i) * columnSize)
            {
                return i;
            }
        }

        return -1;
    }
}

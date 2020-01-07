using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        boardSetup = FindObjectOfType<Setup>();

        gameState = UnityEngine.Random.Range(0, 2) == 0 ? State.player1 : State.player2;
        print(gameState.ToString() + " starts!");

        columns = boardSetup.columns;
        rows = boardSetup.rows;

        columnSize = (boardSetup.width - boardSetup.gap) / columns;
        startPosX = boardSetup.bottomLeft.position.x;
    }

    private void OnMouseDown()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int selectedColumn = GetColumn(mousePosition);

        if(selectedColumn == 0)
        {
            return;
        }

        int startIndex = (selectedColumn - 1) * rows;
        for (int i = startIndex; i < startIndex + rows; i++)
        {
            GameObject coinObject = boardSetup.circles[i];
            if (coinObject.GetComponent<Coin>().isEmpty)
            {
                coinObject.GetComponent<SpriteRenderer>().color = gameState == State.player1 ? player1Color : player2Color;

                Coin coin = coinObject.GetComponent<Coin>();
                coin.isEmpty = false;
                coin.isPlayer1 = gameState == State.player1 ? true : false;

                gameState = gameState == State.player1 ? State.player2 : State.player1;
                break;
            }
        }

    }

    private void OnMouseOver()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int selectedColumn = GetColumn(mousePosition);

        if (previouslyHighlighted != -1)
        {
            ClearHighlighted(previouslyHighlighted);
        }

        HighlightColumn(selectedColumn);
        previouslyHighlighted = selectedColumn;

    }

    private void OnMouseExit()
    {
        ClearHighlighted(previouslyHighlighted);
        previouslyHighlighted = -1;
    }

    private void HighlightColumn(int columnIndex)
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
                coinObject.GetComponent<SpriteRenderer>().color = highlightedColor;
            }
        }
    }

    private void ClearHighlighted(int columnIndex)
    {
        int startIndex = (columnIndex - 1) * rows;
        for (int i = startIndex; i < startIndex + rows; i++)
        {
            GameObject coinObject = boardSetup.circles[i];
            if (coinObject.GetComponent<Coin>().isEmpty)
            {
                coinObject.GetComponent<SpriteRenderer>().color = normalColor;
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

        return 0;
    }
}

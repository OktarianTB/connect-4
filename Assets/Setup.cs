using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{

    public Transform bottomLeft;
    public Transform topRight;

    public GameObject circlePrefab;
    public List<GameObject> circles;

    public float width, height;
    public float gap = 0.6f;
    public int rows = 6;
    public int columns = 7;
    float spacing;

    void Start()
    {
        if (!circlePrefab || !bottomLeft || !topRight)
        {
            Debug.LogWarning("Objects missing in inspector.");
            return;
        }

        circles = new List<GameObject>();

        width = Mathf.Abs(bottomLeft.position.x - topRight.position.x);
        height = Mathf.Abs(bottomLeft.position.y - topRight.position.y);

        GenerateBoard();
    }

    private void GenerateBoard()
    {
        for (int i = 1; i < columns + 1; i++)
        {
            spacing = (width + gap) / (columns + 1);

            for (int j = 1; j < rows + 1; j++)
            {
                float spawnX = bottomLeft.position.x + spacing * i - gap / 2;
                float spawnY = bottomLeft.position.y + spacing * j - gap / 2;
                Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);
                GameObject circle = Instantiate(circlePrefab, spawnPosition, Quaternion.identity);
                circle.gameObject.transform.SetParent(transform);
                circle.gameObject.name = "Circle at (" + i.ToString() + ", " + j.ToString() + ")";
                circles.Add(circle);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CircleSpawner : MonoBehaviour
{
    public GameObject circlePrefab;
    public int numberOfPositions = 10;
    public float timeout = 3.0f; // Timeout in seconds
    public int minCircleCount = 5;
    public int maxCircleCount = 10;

    private List<Vector3> fixedSpawnPositions;
    private float startTime;

    private void OnEnable()
    {
        fixedSpawnPositions = new List<Vector3>();
        startTime = Time.time;

        GenerateFixedSpawnPositions();
        SpawnCircles();

        Task_2_Manager.Instance.currentState = Task_2_Manager.state.In_Progress;
    }

    private void GenerateFixedSpawnPositions()
    {
        float safeMargin = circlePrefab.GetComponent<CircleCollider2D>().radius * 2;

        for (int i = 0; i < numberOfPositions; i++)
        {
            Vector3 newPosition;

            do
            {
                float x = UnityEngine.Random.Range(safeMargin, Screen.width - safeMargin);
                float y = UnityEngine.Random.Range(safeMargin, Screen.height - safeMargin);

                newPosition = new Vector3(x, y, 0);
            } while (CheckOverlap(newPosition));

            fixedSpawnPositions.Add(Camera.main.ScreenToWorldPoint(newPosition));
        }
    }

    private bool CheckOverlap(Vector3 position)
    {
        foreach (Vector3 fixedPosition in fixedSpawnPositions)
        {
            if (Vector3.Distance(position, fixedPosition) < circlePrefab.GetComponent<CircleCollider2D>().radius * 2)
            {
                return true;
            }
        }
        return false;
    }

    private void SpawnCircles()
    {
        SimpleShuffle(fixedSpawnPositions);

        var randomCountOfCircles = UnityEngine.Random.Range(minCircleCount, maxCircleCount + 1);

        Task_2_Manager.Instance.circlesInitialized?.Invoke(randomCountOfCircles);

        for (Int16 i=0;i< randomCountOfCircles;i++)
        {
            Instantiate(circlePrefab, (Vector2) fixedSpawnPositions[i], Quaternion.identity);
        }
    }

    private void Update()
    {
        // Timeout to prevent endless attempts
        if (Time.time - startTime > timeout)
        {
            if(fixedSpawnPositions!= null && fixedSpawnPositions.Count != 10)
            {
                Debug.LogWarning("Timed out while finding non-overlapping positions.");
                enabled = false; // Disable the script
            }
        }
    }

    private void SimpleShuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

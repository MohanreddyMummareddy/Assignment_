using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public static Manager Instance { get; private set; }
    public LayerMask circleLayer;
    public Button restart;

    public enum state { Idle, In_Progress, Done }
    public state currentState = state.Idle;

    public Action<Vector3[]> isLineDrawingCompleted;
    public Action<int> circlesInitialized;

    [SerializeField] private int countOfCirclesInitialized;
    [SerializeField] private int countOfCirclesDeleted;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        restart.onClick.AddListener( delegate { Restart(); } );
    }

    private void OnEnable()
    {
        isLineDrawingCompleted += GameOver;
        circlesInitialized += CirclesInitialized;
    }

    private void OnDisable()
    {
        isLineDrawingCompleted -= GameOver;
        circlesInitialized -= CirclesInitialized;
    }

    private void CirclesInitialized(int count)
    {
        countOfCirclesInitialized = count;
    }

    private void GameOver(Vector3[] linePositions)
    {
        foreach (Vector3 position in linePositions)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f, circleLayer);

            foreach (Collider2D collider in colliders)
            {
                DestroyImmediate(collider.gameObject);
            }
        }

       var currentCountOfCircles =  GameObject.FindGameObjectsWithTag("Circle").Length;

        countOfCirclesDeleted = countOfCirclesInitialized - currentCountOfCircles;

        Debug.Log(countOfCirclesDeleted);

        if (countOfCirclesInitialized == currentCountOfCircles)
        {
            return; // No circles got deleted, that means, user didn't draw on atleast one circle
        }
        else
        {
            currentState = state.Done;

            // Enable Restart Button
            restart.gameObject.SetActive(true);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

}

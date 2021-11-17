using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    [HideInInspector] public bool isGameOver;

    [SerializeField] private bool canAutoMove;
    [SerializeField] private bool hint;
    [SerializeField] private Grid_Viz grid;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;
        canAutoMove = false;
        hint = false;
    }

    private void Update()
    {
        if (canAutoMove)
        {
            grid.hint = true;
            grid.canAutoMove = true;
            canAutoMove = false;
        }

        if (hint)
        {
            grid.hint = true;
            grid.canAutoMove = false;
            hint = false;
        }

        if (isGameOver)
        {
            EndGame();
        }
    }

    public void AutoMove()
    {
        canAutoMove = true;
    }

    public void Hint()
    {
        hint = true;
    }

    void EndGame()
    {
        SceneManager.LoadScene("GamePlay");
    }

    public void BackButton()
    {
        SceneManager.LoadScene("SelectLevel");
    }

}

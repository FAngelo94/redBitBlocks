using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{ 
    public GameObject restart;

    public GameObject buttonsContainer;
    private GameObject[,] buttons;
    private int[,] buttonsValue;

    public Text points;
    private PointsManager pointsManager;

    public Text time;
    private TimeManager timeManager;

    private GamePlay gamePlay;

    public bool InGame { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        int index = 0;
        buttons = new GameObject[4,4];
        buttonsValue = new int[4,4];
        for (int r = 0; r < 4; r++)
        {
            for(int c = 0; c < 4; c++)
            {

                buttons[r,c]= buttonsContainer.transform.GetChild(index).gameObject;
                DataButton dataButton = buttons[r, c].GetComponent<DataButton>();
                dataButton.R = r;
                dataButton.C = c;
                index++;
            }
        }
        gamePlay = gameObject.AddComponent(typeof(GamePlay)) as GamePlay;
        timeManager = gameObject.AddComponent(typeof(TimeManager)) as TimeManager;
        pointsManager = gameObject.AddComponent(typeof(PointsManager)) as PointsManager;
        SetVariables();
    }

    /// <summary>
    /// Init here everything you need set again after the game restart
    /// (for example the Timer or the color of buttons)
    /// </summary>
    private void SetVariables()
    {
        InGame = true;
        restart.SetActive(false);
        timeManager.init(this, time);
        pointsManager.init(this, points);
        gamePlay.init(this, timeManager, pointsManager, buttons, buttonsValue);

    }

    /// <summary>
    /// Function to restart the game after GameOver
    /// </summary>
    public void Restart()
    {
        SetVariables();
    }

    /// <summary>
    /// Stop the game
    /// </summary>
    public void GameOver()
    {
        InGame = false;
        restart.SetActive(true);
    }

    public void ClickButton()
    {
        gamePlay.ClickButton();
    }
}
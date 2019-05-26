using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    //red, blue, green, yellow, violet,orange
    private readonly Color[] buttonColors = { Color.red, Color.blue, Color.green, Color.yellow, new Color(255f / 255f, 113f / 255f, 25f / 255f), new Color(191f / 255f, 33f / 255f, 188f / 255f) };

    public GameObject restart;

    public GameObject buttonsContainer;
    private GameObject[,] buttons;
    private int[,] buttonsValue;
    public Text points;
    private float pointsValue;
    public Text time;
    private float timeSeconds;
    private bool inGame;
    private bool isBlocksDestroyed;

    private static Mutex mut = new Mutex();

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
        SetVariables();
    }

    /// <summary>
    /// Function called when user click on a button
    /// </summary>
    public void ClickButton()
    {
        if (!inGame || isBlocksDestroyed)
            return;
        int blocks = 0;
        DataButton dataButton = EventSystem.current.currentSelectedGameObject.GetComponent<DataButton>();
        int r = dataButton.R;
        int c = dataButton.C;
        int currentValue = buttonsValue[r, c];
        blocks = CountBlocks(r, c, buttonsValue[r,c]);

        if (blocks > 1)
        {
            isBlocksDestroyed = true;
            UpdatePoints(blocks);
            UpdateTimer(blocks);
            StartCoroutine(UpdateBlocksDelay());
        }
        else
        {
            buttonsValue[r, c] = currentValue;
        }
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
    private void GameOver()
    {
        inGame = false;
        restart.SetActive(true);
    }

    /// <summary>
    /// Init here everything you need set again after the game restart
    /// (for example the Timer or the color of buttons)
    /// </summary>
    private void SetVariables()
    {
        pointsValue = 0;
        points.text = "0";
        timeSeconds = 120;
        time.text = "2:00";
        inGame = true;
        isBlocksDestroyed = false;
        SetInitColorButtons();
        StartCoroutine(Timer());
        restart.SetActive(false);
    }

    /// <summary>
    /// Recoursive function that count how many blocks user remove when click a button
    /// if there are at leat 2 equal color near
    /// </summary>
    /// <param name="r">Row of the block you want to check</param>
    /// <param name="c">Column of the block you want to check</param>
    /// <param name="color">Color of the blocks you want destroy, it does't change during
    /// the recursion</param>
    /// <returns></returns>
    private int CountBlocks(int r, int c, int color)
    {
        if (c < 0 || c > 3 || r < 0 || r > 3)
            return 0;
        if (buttonsValue[r, c] == color)
        {
            buttonsValue[r, c] = -1;
            return 1 + CountBlocks(r + 1, c, color) + CountBlocks(r - 1, c, color) + CountBlocks(r, c + 1, color) + CountBlocks(r, c - 1, color);
        }
        return 0;
    }

    /// <summary>
    /// Create the new blocks after 0.5 seconds, so player undestand wich blocks
    /// he destroyed
    /// </summary>
    private IEnumerator UpdateBlocksDelay()
    {
        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                if (buttonsValue[r, c] == -1)
                {
                    buttons[r, c].SetActive(false);
                    SetColorButton(r, c);
                }

            }
        }
        yield return new WaitForSeconds(0.5f);
        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                buttons[r, c].SetActive(true);
            }
        }
        if (!IsAvailableMoves())
        {
            GameOver();
        }
        isBlocksDestroyed = false;
    }

    /// <summary>
    /// Function to check if player can do other move 
    /// </summary>
    /// <returns>True if the player can do an other move, False if the game is end
    /// because there isn't any other move</returns>
    private bool IsAvailableMoves()
    {
        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                if (c - 1 > 0 && buttonsValue[r, c] == buttonsValue[r, c - 1])
                    return true;
                if (c + 1 < 4 && buttonsValue[r, c] == buttonsValue[r, c + 1])
                    return true;
                if (r - 1 > 0 && buttonsValue[r, c] == buttonsValue[r -1, c])
                    return true;
                if (r + 1 < 4 && buttonsValue[r, c] == buttonsValue[r + 1, c])
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Set the initial color for the button
    /// </summary>
    private void SetInitColorButtons()
    {
        Debug.Log("SetInitColor");
        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                SetColorButton(r, c);
            }
        }
    }

    /// <summary>
    /// Set randomly the color of the button in the row r and column c
    /// </summary>
    /// <param name="r">Row of the blocks you want to change color</param>
    /// <param name="c">Column of the blocks you want to change color</param>
    private void SetColorButton(int r, int c)
    {
        buttonsValue[r, c] = (int)Random.Range(0, buttonColors.Length);
        Button b = buttons[r, c].GetComponent<Button>();
        ColorBlock colorBlock = b.colors;
        Color randomColor = buttonColors[buttonsValue[r, c]];
        Debug.Log(randomColor);
        Debug.Log(buttonsValue[r, c]);
        b.GetComponent<Image>().color = randomColor;
        colorBlock.normalColor = randomColor;
        colorBlock.highlightedColor = randomColor;
        colorBlock.pressedColor = new Color(randomColor.r, randomColor.g, randomColor.b, 0.5f);
    }

    /// <summary>
    /// Function to update the points everytime user destroy new blocks
    /// </summary>
    /// <param name="x">Number of blocks destroyed</param>
    private void UpdatePoints(int x)
    {
        pointsValue += (x - 1) * 80 + Mathf.Pow((x - 2) / 5, 2);
        points.text = pointsValue.ToString();
    }

    /// <summary>
    /// Courutine that make the coundown
    /// </summary>
    private IEnumerator Timer()
    {
        while (inGame)
        {
            UpdateTimer();
            yield return new WaitForSeconds(1.0f);
        }
        yield return 0;
    }

    /// <summary>
    /// Function that update the second, here there is a Mutex to avoid that Courutine 
    /// and main Program (when user destroy new blocks) change the value of the seconds in the same moment
    /// causing an inconsistent value
    /// </summary>
    /// <param name="x">Number of blocks destroyed, if x=0 (default value) function just decrement the timer of 1 second.
    ///  If x > 0 add seconds bonus to the timer</param>
    private void UpdateTimer(int x = 0)
    {
        mut.WaitOne();
        if (x == 0)
            timeSeconds--;
        else
            timeSeconds += 10 + Mathf.Pow((x - 2) / 3, 2) * 20;
        time.text = ((int)timeSeconds / 60) + ":";
        if (timeSeconds % 60 == 0)
            time.text += "00";
        if (timeSeconds % 60 > 0 && timeSeconds % 60 < 10)
            time.text += "0" + (timeSeconds % 60);
        if (timeSeconds % 60 > 9)
            time.text += (timeSeconds % 60);
        if (timeSeconds <= 0)
            GameOver();
        mut.ReleaseMutex();
    }
}
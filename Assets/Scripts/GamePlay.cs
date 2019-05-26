using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePlay : MonoBehaviour
{
    //red, blue, green, yellow, violet,orange
    private readonly Color[] buttonColors = { Color.red, Color.blue, Color.green, Color.yellow, new Color(255f / 255f, 113f / 255f, 25f / 255f), new Color(191f / 255f, 33f / 255f, 188f / 255f) };

    private GameManager GM;
    private TimeManager timeManager;
    private PointsManager pointsManager;
    private GameObject[,] buttons;
    private int[,] buttonsValue;
    private bool isBlocksDestroyed;

    public void init(GameManager GM, TimeManager timeManager, PointsManager pointsManager, GameObject[,] buttons, int[,] buttonsValue)
    {
        this.GM = GM;
        this.timeManager = timeManager;
        this.pointsManager = pointsManager;
        this.buttons = buttons;
        this.buttonsValue = buttonsValue;
        isBlocksDestroyed = false;
        SetInitColorButtons();
    }

    /// <summary>
    /// Function called when user click on a button
    /// </summary>
    public void ClickButton()
    {
        if (!GM.InGame || isBlocksDestroyed)
            return;
        int blocks = 0;
        DataButton dataButton = EventSystem.current.currentSelectedGameObject.GetComponent<DataButton>();
        int r = dataButton.R;
        int c = dataButton.C;
        int currentValue = buttonsValue[r, c];
        blocks = CountBlocks(r, c, buttonsValue[r, c]);

        if (blocks > 1)
        {
            isBlocksDestroyed = true;
            pointsManager.UpdatePoints(blocks);
            timeManager.UpdateTimer(blocks);
            StartCoroutine(UpdateBlocksDelay());
        }
        else
        {
            buttonsValue[r, c] = currentValue;
        }
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
            GM.GameOver();
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
                if (r - 1 > 0 && buttonsValue[r, c] == buttonsValue[r - 1, c])
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
        b.GetComponent<Image>().color = randomColor;
        colorBlock.normalColor = randomColor;
        colorBlock.highlightedColor = randomColor;
        colorBlock.pressedColor = new Color(randomColor.r, randomColor.g, randomColor.b, 0.5f);
    }
}

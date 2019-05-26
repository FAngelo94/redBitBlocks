using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsManager : MonoBehaviour
{
    private GameManager GM;
    private float pointsValue;
    private Text points;

    public void init(GameManager GM, Text points)
    {
        this.GM = GM;
        this.points = points;
        pointsValue = 0;
        points.text = "0";
    }

    /// <summary>
    /// Function to update the points everytime user destroy new blocks
    /// </summary>
    /// <param name="x">Number of blocks destroyed</param>
    public void UpdatePoints(int x)
    {
        pointsValue += (x - 1) * 80 + Mathf.Pow((x - 2) / 5, 2);
        points.text = pointsValue.ToString();
    }
}

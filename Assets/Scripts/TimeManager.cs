using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    private GameManager GM;
    private float timeSeconds;
    private static Mutex mut = new Mutex();
    private Text time;

    public void init(GameManager GM, Text time)
    {
        Debug.Log("construtor");
        this.GM = GM;
        this.time = time;
        timeSeconds = 120;
        time.text = "2:00";
        StartCoroutine(Timer());
    }
    /// <summary>
    /// Courutine that make the coundown
    /// </summary>
    private IEnumerator Timer()
    {
        while (GM.InGame)
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
    public void UpdateTimer(int x = 0)
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
            GM.GameOver();
        mut.ReleaseMutex();
    }
}

using System.Globalization;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public bool isTimerRunning = false;
    public float timeElapsed;
    public float totalTime;

    private void Update()
    {
        UpdateTimer();
    }
    private void UpdateTimer()
    {
        if (isTimerRunning)
        {
            timeElapsed += Time.deltaTime;
        }
        else if (totalTime != timeElapsed)
        {
            totalTime = timeElapsed;
        }
    }
}

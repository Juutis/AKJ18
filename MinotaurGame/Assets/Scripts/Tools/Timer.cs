using System;
using System.Diagnostics;

public class Timer
{

    Stopwatch stopwatch;
    public Timer()
    {
        stopwatch = Stopwatch.StartNew();
    }
    public void Pause()
    {
        if (stopwatch.IsRunning)
        {
            stopwatch.Stop();
        }
    }

    public void Unpause()
    {
        stopwatch.Start();
    }

    public static string GetFormattedString(double timeInMs)
    {
        TimeSpan ts = TimeSpan.FromMilliseconds(timeInMs);
        return ts.ToString(@"mm\:ss\.ff");
    }

    public string GetString()
    {
        return stopwatch.Elapsed.ToString(@"mm\:ss\.ff");
    }

    public double GetTime()
    {
        return stopwatch.Elapsed.TotalMilliseconds;
    }

}
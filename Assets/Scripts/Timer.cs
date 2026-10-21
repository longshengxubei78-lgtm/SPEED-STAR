using UnityEngine;

public class Timer
{
    private float _startTime;
    private float _duration;
    private bool _hasStarted;

    public bool IsFinished =>
        _hasStarted &&
        Time.time - _startTime >= _duration;

    public bool IsRunning =>
        _hasStarted && !IsFinished;

    public void Start(float duration)
    {
        _duration = Mathf.Max(0f, duration);
        _startTime = Time.time;
        _hasStarted = true;
    }

    public void Cancel()
    {
        _hasStarted = false;
    }
}
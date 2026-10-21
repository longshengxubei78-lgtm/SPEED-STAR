using System;
using XqLua;

public class InputData : IDisposable
{
    public bool _isDrifting { get; private set; }
    public float _inputHorizontal { get; private set; }
    public float _inputVertical { get; private set; }
    
    public Publisher<bool> DriftChanged { get; } = new Publisher<bool>();

    public void SetMove(float horizontal, float vertical)
    {
        _inputHorizontal = horizontal;
        _inputVertical = vertical;
    }

    public void SetDrift(bool drifting)
    {
        if (_isDrifting == drifting) return;
        _isDrifting = drifting;
        DriftChanged.Invoke(drifting);
    }

    public void Dispose()
    {
        DriftChanged.Dispose();
    }
}
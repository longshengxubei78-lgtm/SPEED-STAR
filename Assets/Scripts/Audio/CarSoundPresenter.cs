using System;
using UnityEngine;

public class CarSoundPresenter :IDisposable
{
    private SoundBank bank;
    private InputData _input;
    public CarSoundPresenter(InputData input, SoundBank soundBank)
    {
        bank = soundBank;
        _input = input;
        input.DriftChanged.Subscribe(OnDriftChanged);
    }

    private void OnDriftChanged(bool drifting)
    {
        if (drifting) bank.Play("Brake");
        else bank.Stop("Brake"); // Brakeをloopにしている場合のみ必要

        Debug.Log($"Drift state changed: {drifting}");
    }
    public void Dispose()
    {
        _input.DriftChanged.Dispose();
    }
    public void PlaySound(string soundName)
    {
        bank.Play(soundName);
    }
    public void StopSound(string soundName)
    {
        bank.Stop(soundName);
    }
}
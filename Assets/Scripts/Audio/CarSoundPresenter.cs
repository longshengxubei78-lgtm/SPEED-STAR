using System;
using UnityEngine;
using XqLua;
using XqLua.Extension;

public class CarSoundPresenter :IDisposable
{
    private SoundBank bank;
    private IPlayerStatus _playerStatus;
    private Disposables _disposables;
    public CarSoundPresenter(IPlayerStatus playerStatus ,SoundBank soundBank)
    {
        bank = soundBank;
        _playerStatus = playerStatus;
        _disposables = new Disposables();
        _playerStatus._onStateChanged.Subscribe(OnDriftChanged).AddTo(_disposables);
    }

    private void OnDriftChanged(CarState carState, CarState oldCarState)
    {
        if(carState == CarState.Drift)
        {
            PlaySound("Drift");
        }
        else if (oldCarState == CarState.Drift)
        {
            StopSound("Drift");
        }
    }
    public void Dispose()
    {
        _disposables.Dispose();
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
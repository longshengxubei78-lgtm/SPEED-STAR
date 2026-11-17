using System;
using XqLua;
using XqLua.Extension;

public class CarSoundPresenter :IDisposable
{
    private SoundBank _bank;
    private IPlayerData _playerStatus;
    private Disposables _disposables;
    public CarSoundPresenter(IPlayerData playerStatus ,SoundBank soundBank)
    {
        _bank = soundBank;
        _playerStatus = playerStatus;
        _disposables = new Disposables();
        _playerStatus._onStateChanged.Subscribe(OnDriftChanged).AddTo(_disposables);
    }

    private void OnDriftChanged(CarState oldCarState, CarState newCarState)
    {
        if(newCarState == CarState.Drift)
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
        _bank.Play(soundName);
    }
    public void StopSound(string soundName)
    {
        _bank.Stop(soundName);
    }
}
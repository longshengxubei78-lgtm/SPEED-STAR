using TMPro;
using UnityEngine;
using XqLua;
public class CarPresenter
{
    private CarDriver _carDriver;


    private Publisher<float> _currentSpeed;
    private IDisposableSubscription _subscription;
    public CarPresenter(SpeedMeter speedMater ,CarDriver carDriver) 
    {
        _currentSpeed = new Publisher<float>();
        _subscription = _currentSpeed.Subscribe(speedMater.SetMeterValue);
        _carDriver = carDriver;
    }

    public void CurrentCarSpeed()
    {
        float currentSpeed = _carDriver.GetCurrentSpeed();

        _currentSpeed.Invoke(currentSpeed);
    }
    public void Disposables()
    {
        _subscription.Dispose();
    }
}

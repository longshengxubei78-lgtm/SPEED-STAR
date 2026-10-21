using UnityEngine;
using System.Collections.Generic;
public class CarStateMachine
{
    private ICarState _currentState = default;

    private Dictionary<System.Type, ICarState> states = default;

    public CarStateMachine(CarDriver driver, InputData inputData, GroundDetector groundDetector,CarPresenter carPresenter, CarSoundPresenter carSoundPresenter,JumpRotation jumpRotation)
    {
        states = new Dictionary<System.Type, ICarState>
        {
            { typeof(CarIdleState), new CarIdleState(this,inputData,carPresenter,carSoundPresenter) },
            { typeof(CarMoveState), new CarMoveState(this, driver, groundDetector,carPresenter,carSoundPresenter) },
            { typeof(CarAirState), new CarAirState(this, groundDetector,carPresenter,carSoundPresenter,jumpRotation) },
        };
        SwitchState(typeof(CarIdleState));
    }
    public void FixedTick()
    {
        _currentState.FixedTick();
    }
    public void Input()
    {
        _currentState.Input();
    }
    public void SwitchState(System.Type newStateType)
    {
        if(_currentState != null)
        {
            _currentState.Exit();
        }

        if (states.TryGetValue(newStateType, out ICarState newState))
        {
            _currentState = newState;

            _currentState.Enter();
        }
        else 
        {
            Debug.LogError("ステートが見つかりません");
        }
    }
}

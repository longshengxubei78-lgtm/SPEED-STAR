using UnityEngine;
using System.Collections.Generic;
public class CarStateMachine
{
    private ICarState _currentState = default;

    private Dictionary<System.Type, ICarState> states = default;

    public CarStateMachine(CarDriver driver, InputReader inputReader, GroundDetector groundDetector,CarPresenter carPresenter)
    {
        states = new Dictionary<System.Type, ICarState>
        {
            { typeof(CarIdleState), new CarIdleState(this,inputReader,carPresenter) },
            { typeof(CarMoveState), new CarMoveState(this, driver, inputReader, groundDetector,carPresenter) },
            { typeof(CarAirState), new CarAirState(this, inputReader, groundDetector,carPresenter) }
        };
        SwitchState(typeof(CarIdleState));
    }
    public void Tick()
    {
        _currentState.Tick();
    }
    public void FixedTick()
    {
        _currentState.FixedTick();
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

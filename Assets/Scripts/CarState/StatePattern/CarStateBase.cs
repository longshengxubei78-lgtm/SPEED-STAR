public abstract class CarStateBase : ICarState
{
    protected CarStateMachine _stateMachine;

    public CarStateBase(CarStateMachine stateMachine)
    {
        _stateMachine = stateMachine;

    }


    public abstract void Enter();
    public abstract void Exit();
    public abstract void FixedTick();
    public abstract void Input();
}

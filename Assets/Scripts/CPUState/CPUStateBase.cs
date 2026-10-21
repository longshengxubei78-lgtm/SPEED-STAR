public abstract class CPUStateBase : ICarState
{
    protected CPUStateMachine _stateMachine;

    public CPUStateBase(CPUStateMachine stateMachine)
    {
        this._stateMachine = stateMachine;
    }

    public abstract void Enter();
    public abstract void Tick();
    public abstract void Exit();
    public abstract void FixedTick();
    public abstract void Input();
}

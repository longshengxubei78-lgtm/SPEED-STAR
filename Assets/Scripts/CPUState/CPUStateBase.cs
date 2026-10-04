public abstract class CPUStateBase : ICPUState
{
    protected CPUStateMachine stateMachine;

    public CPUStateBase(CPUStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public abstract void Enter();
    public abstract void Tick();
    public abstract void Exit();
    public abstract void FixedTick();
}

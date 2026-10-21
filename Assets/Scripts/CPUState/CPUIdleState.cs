using UnityEngine;

public class CPUIdleState : CPUStateBase
{
    private InputData _inputData;
    public CPUIdleState(CPUStateMachine stateMachine, InputData inputData) : base(stateMachine)
    {
        _inputData = inputData;
    }
    public override void Enter()
    {
    }
    public override void Exit()
    {

    }
    public override void Tick()
    {
    }
    public override void FixedTick()
    {
        float vertical =_inputData._inputVertical;
        if (vertical != 0)
        {
            _stateMachine.SwitchState(typeof(CPUMoveState));
        }
    }
    public override void Input()
    {
        // Handle input for idle state
    }
}
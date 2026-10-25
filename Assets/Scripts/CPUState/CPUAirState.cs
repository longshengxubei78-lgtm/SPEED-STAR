using UnityEngine;

public class CPUAirState : CPUStateBase
{
    public CPUAirState(CPUStateMachine stateMachine) : base(stateMachine)
    {

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
        //GroundDetector _groundDetector = new GroundDetector(_stateMachine.transform);
        
        ////bool isGrounded = _groundDetector.GroundSearch();

        //if (isGrounded)
        //{
        //    _stateMachine.SwitchState(typeof(CPUMoveState));
        //}
    }
    public override void Input()
    {
        // Handle input for air state
    }
}

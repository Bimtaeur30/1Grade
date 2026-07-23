public sealed class PlayerDigState : PlayerState
{
    public PlayerDigState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        StateMachine.AgentMover.SetMovementEnabled(false);
        StateMachine.PlayAnimation(PlayerStateMachine.DigAnimationHash);
    }

    public override void Exit()
    {
        StateMachine.AgentMover.SetMovementEnabled(true);
    }
}

public sealed class PlayerDigState : PlayerState
{
    public PlayerDigState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        StateMachine.AgentMover.SetMovementEnabled(false);
        StateMachine.PlayAnimation(PlayerStateMachine.DigAnimationHash);
        StateMachine.PlayDigParticle();
    }

    public override void Exit()
    {
        StateMachine.StopDigParticle();
        StateMachine.AgentMover.SetMovementEnabled(true);
    }
}

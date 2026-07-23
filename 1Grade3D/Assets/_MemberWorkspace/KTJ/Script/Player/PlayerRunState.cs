public sealed class PlayerRunState : PlayerState
{
    public PlayerRunState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        StateMachine.PlayAnimation(PlayerStateMachine.RunAnimationHash);
    }

    public override void Tick()
    {
        if (!StateMachine.AgentMover.HasMoveInput)
        {
            StateMachine.ChangeState(StateMachine.IdleState);
        }
    }
}

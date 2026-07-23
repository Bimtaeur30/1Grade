public sealed class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        StateMachine.PlayAnimation(PlayerStateMachine.IdleAnimationHash);
    }

    public override void Tick()
    {
        if (StateMachine.AgentMover.HasMoveInput)
        {
            StateMachine.ChangeState(StateMachine.RunState);
        }
    }
}

public abstract class PlayerState
{
    protected readonly PlayerStateMachine StateMachine;

    protected PlayerState(PlayerStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }

    public abstract void Enter();
    public virtual void Exit() { }
    public virtual void Tick() { }
}

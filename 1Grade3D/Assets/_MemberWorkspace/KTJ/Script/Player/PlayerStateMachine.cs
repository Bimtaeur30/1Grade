using UnityEngine;

public sealed class PlayerStateMachine : MonoBehaviour
{
    [SerializeField] private AgentMover agentMover;
    [SerializeField] private Animator animator;
    [SerializeField, Min(0f)] private float animationTransitionDuration = 0.1f;

    public static readonly int IdleAnimationHash = Animator.StringToHash("IDLE");
    public static readonly int RunAnimationHash = Animator.StringToHash("RUN");
    public static readonly int DigAnimationHash = Animator.StringToHash("DIG");

    public AgentMover AgentMover => agentMover;
    public PlayerIdleState IdleState { get; private set; }
    public PlayerRunState RunState { get; private set; }
    public PlayerDigState DigState { get; private set; }

    private PlayerState currentState;

    private void Awake()
    {
        IdleState = new PlayerIdleState(this);
        RunState = new PlayerRunState(this);
        DigState = new PlayerDigState(this);
    }

    private void Start()
    {
        ChangeState(IdleState);
    }

    private void Update()
    {
        currentState?.Tick();
    }

    public void ChangeState(PlayerState nextState)
    {
        if (nextState == null || currentState == nextState)
        {
            return;
        }

        currentState?.Exit();
        currentState = nextState;
        currentState.Enter();
    }

    // 파기 시작 이벤트에 이 메서드를 구독하세요.
    public void OnDigStarted()
    {
        ChangeState(DigState);
    }

    // 파기 종료 이벤트에 이 메서드를 구독하세요.
    public void OnDigEnded()
    {
        if (currentState != DigState)
        {
            return;
        }

        ChangeState(agentMover.HasMoveInput ? RunState : IdleState);
    }

    public void PlayAnimation(int animationHash)
    {
        animator.CrossFade(animationHash, animationTransitionDuration);
    }
}

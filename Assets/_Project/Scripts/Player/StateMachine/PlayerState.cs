public abstract class PlayerState {

    protected readonly PlayerController Controller;
    protected readonly PlayerStateMachine StateMachine;
    protected readonly PlayerStateFactory Factory;
    protected readonly PlayerStats Stats;
    protected readonly PlayerAnimator Animator;

    protected PlayerState(PlayerController controller, PlayerStateMachine stateMachine, PlayerStateFactory factory) {
        Controller = controller;
        StateMachine = stateMachine;
        Factory = factory;
        Stats = controller.Stats;
        Animator = controller.Animator;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}

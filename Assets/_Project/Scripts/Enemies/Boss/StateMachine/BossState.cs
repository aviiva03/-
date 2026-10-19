public abstract class BossState {
    protected readonly BossController Controller;
    protected readonly BossStateMachine StateMachine;
    protected readonly BossStateFactory Factory;
    protected readonly BossStats Stats;
    protected readonly BossAnimator Animator;
    protected readonly BossMovement Movement;

    protected BossState(BossController c, BossStateMachine m, BossStateFactory f) {
        Controller = c;
        StateMachine = m;
        Factory = f;
        Stats = c.Stats;
        Animator = c.Animator;
        Movement = c.Movement;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
public class BossStateFactory {

    public BossIdleState Idle { get; }
    public BossChaseState Chase { get; }
    public BossStrafeState Strafe { get; }
    public BossAttackState Attack { get; }

    public BossStateFactory(BossController c, BossStateMachine m) {
        Idle = new BossIdleState(c, m, this);
        Chase = new BossChaseState(c, m, this);
        Strafe = new BossStrafeState(c, m, this);
        Attack = new BossAttackState(c, m, this);
    }
}
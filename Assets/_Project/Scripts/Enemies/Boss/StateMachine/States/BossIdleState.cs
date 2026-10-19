using UnityEngine;

public class BossIdleState : BossState {

    private float _idleTimer;

    public BossIdleState(BossController c, BossStateMachine m, BossStateFactory f)
        : base(c, m, f) { }

    public override void Enter() {
        _idleTimer = 1f;
        Movement.Stop();
    }

    public override void Update() {
        if (Controller.Player != null)
            Movement.FaceTarget(Controller.Player.position, Stats.TurnDegreesPerSecond);

        _idleTimer -= Time.deltaTime;
        if (_idleTimer > 0f) return;

        float dist = Controller.DistanceToPlayer;

        if (dist > Stats.StrafeMaxDistance) {
            StateMachine.ChangeState(Factory.Chase);
        } else {
            StateMachine.ChangeState(Factory.Strafe);
        }
    }
}
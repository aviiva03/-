using UnityEngine;

public class BossChaseState : BossState {

    public BossChaseState(BossController c, BossStateMachine m, BossStateFactory f)
        : base(c, m, f) { }

    public override void Enter() { }

    public override void Update() {
        if (Controller.Player == null) return;

        float dist = Controller.DistanceToPlayer;

        // Поворот к игроку
        Movement.FaceTarget(Controller.Player.position, Stats.TurnDegreesPerSecond);

        // Скорость плавно падает при приближении
        float slowdownRange = 3f;
        float speedFactor = Mathf.Clamp01(
            Mathf.InverseLerp(Stats.StrafeMaxDistance, Stats.StrafeMaxDistance + slowdownRange, dist));

        float targetSpeed = Stats.ChaseSpeed * speedFactor;
        var phase = Controller.CurrentPhase;
        if (phase != null) targetSpeed *= phase.SpeedMultiplier;

        Movement.ChasePlayer(targetSpeed);
        Animator.SetSpeed(targetSpeed / Stats.ChaseSpeed);

        // Дошли до зоны Strafe — переход
        if (dist <= Stats.StrafeMaxDistance) {
            StateMachine.ChangeState(Factory.Strafe);
        }
    }
}
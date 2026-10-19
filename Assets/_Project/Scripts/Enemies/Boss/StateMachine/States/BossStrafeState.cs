using UnityEngine;

public class BossStrafeState : BossState {

    private float _directionChangeTimer;
    private float _strafeUpdateTimer;
    private float _attackCheckTimer;

    public BossStrafeState(BossController c, BossStateMachine m, BossStateFactory f)
        : base(c, m, f) { }

    public override void Enter() {
        _directionChangeTimer = Stats.StrafeDirectionChangeTime;
        _strafeUpdateTimer = 0f;
        _attackCheckTimer = Random.Range(1f, 2f);   // дольше перед первой атакой

        Movement.SetStrafeSign(Random.value > 0.5f ? 1f : -1f);
    }

    public override void Update() {
        if (Controller.Player == null) return;

        float dist = Controller.DistanceToPlayer;

        Movement.FaceTarget(Controller.Player.position, Stats.TurnDegreesPerSecond);
        Animator.SetStrafe(Movement.StrafeSign);

        // —лишком далеко Ч Chase
        if (dist > Stats.StrafeMaxDistance + 1f) {
            StateMachine.ChangeState(Factory.Chase);
            return;
        }

        // —лишком близко Ч Idle (ждЄм, пока игрок отойдЄт)
        if (dist < Stats.StrafeMinDistance - 0.5f) {
            StateMachine.ChangeState(Factory.Idle);
            return;
        }

        //  ружение
        _strafeUpdateTimer -= Time.deltaTime;
        if (_strafeUpdateTimer <= 0f) {
            _strafeUpdateTimer = 0.3f;
            Movement.StrafeAroundPlayer(Stats.StrafeSpeed);
        }

        // —мена направлени€
        _directionChangeTimer -= Time.deltaTime;
        if (_directionChangeTimer <= 0f) {
            Movement.FlipStrafe();
            _directionChangeTimer = Stats.StrafeDirectionChangeTime;
        }

        // ѕроверка атаки
        _attackCheckTimer -= Time.deltaTime;
        if (_attackCheckTimer <= 0f) {
            if (TryAttackIfPossible()) return;
            _attackCheckTimer = Random.Range(1f, 2f);
        }
    }

    private bool TryAttackIfPossible() {
        if (Controller.Combat == null) return false;
        if (Controller.AttackLibrary == null) return false;
        if (Controller.CurrentPhase == null) return false;
        if (!Controller.Combat.CanStartCombo) return false;

        StateMachine.ChangeState(Factory.Attack);
        return true;
    }
}
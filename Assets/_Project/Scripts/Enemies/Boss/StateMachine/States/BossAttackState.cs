using System.Collections;
using UnityEngine;

public class BossAttackState : BossState {

    private BossAttackExecutor _executor;

    public BossAttackState(BossController c, BossStateMachine m, BossStateFactory f)
        : base(c, m, f) { }

    public override void Enter() {
        Debug.Log("[ATTACK STATE] Enter");

        if (!Controller.Combat.TryStartCombo()) {
            Debug.Log("[ATTACK STATE] No combo available! Going Idle.");
            StateMachine.ChangeState(Factory.Idle);
            return;
        }

        Debug.Log($"[ATTACK STATE] Combo started: {Controller.Combat.ActiveCombo.ComboName}");
        Controller.SetAttacking(true);
        _executor = new BossAttackExecutor(Controller);
        Controller.StartCoroutine(RunCombo());
    }

    public override void Update() { }

    public override void Exit() {
        if (_executor != null) {
            _executor.Cancel();
            _executor = null;
        }
        Controller.SetAttacking(false);
        Controller.Combat.EndCombo();
    }

    private IEnumerator RunCombo() {
        while (!Controller.Combat.IsComboFinished) {
            var attack = Controller.Combat.GetCurrentAttack();
            if (attack == null) break;

            yield return new WaitForSeconds(0.1f);
            yield return _executor.Execute(attack);
            Controller.Combat.AdvanceAttack();
        }

        if (!Controller.IsAlive) yield break;

        // Всегда в Strafe — он сам решит, Chase если надо
        StateMachine.ChangeState(Factory.Strafe);
    }
}
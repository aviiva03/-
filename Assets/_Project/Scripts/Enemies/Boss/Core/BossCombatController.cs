using UnityEngine;

public class BossCombatController : MonoBehaviour {

    private BossController _boss;
    private float _cooldownTimer;

    public ComboData ActiveCombo { get; private set; }
    public int AttackIndex { get; private set; }

    public bool CanStartCombo => _cooldownTimer <= 0f;

    private void Awake() {
        _boss = GetComponent<BossController>();
        ResetCooldown();
    }

    private void Update() {
        if (_cooldownTimer > 0f) _cooldownTimer -= Time.deltaTime;
    }

    public void ResetCooldown() {
        float min = _boss.Stats.MinComboDelay;
        float max = _boss.Stats.MaxComboDelay;

        var phase = _boss.CurrentPhase;
        if (phase != null && phase.AggressionMultiplier > 0f) {
            min /= phase.AggressionMultiplier;
            max /= phase.AggressionMultiplier;
        }

        _cooldownTimer = Random.Range(min, max);
    }

    /// <summary>
    /// Попробовать начать новое комбо.
    /// </summary>
    public bool TryStartCombo() {
        if (!CanStartCombo) return false;

        var phase = _boss.CurrentPhase;
        if (phase == null) return false;

        var combo = phase.PickCombo(_boss.HealthPercent, _boss.DistanceToPlayer);
        if (combo == null) return false;

        ActiveCombo = combo;
        AttackIndex = 0;
        ResetCooldown();
        return true;
    }

    public AttackData GetCurrentAttack() {
        if (ActiveCombo == null) return null;
        if (AttackIndex < 0 || AttackIndex >= ActiveCombo.Attacks.Count) return null;
        return ActiveCombo.Attacks[AttackIndex];
    }

    public void AdvanceAttack() {
        AttackIndex++;
    }

    public bool IsComboFinished => ActiveCombo == null
        || AttackIndex >= ActiveCombo.Attacks.Count;

    public void EndCombo() {
        ActiveCombo = null;
        AttackIndex = 0;
    }
}
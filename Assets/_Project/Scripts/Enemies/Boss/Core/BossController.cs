using UnityEngine;

public class BossController : MonoBehaviour {

    [ContextMenu("Debug: Take 600 Damage")]
    private void DebugTakeDamage() {
        TakeDamage(600f);
    }

    [Header("References")]
    [SerializeField] private BossStats _stats;
    [SerializeField] private BossAttackLibrary _attackLibrary;
    [SerializeField] private Transform _player;

    public BossStats Stats => _stats;
    public BossAttackLibrary AttackLibrary => _attackLibrary;
    public Transform Player => _player;
    public BossAnimator Animator { get; private set; }
    public BossMovement Movement { get; private set; }
    public BossCombatController Combat { get; private set; }

    public float CurrentHealth { get; private set; }
    public float MaxHealth => _stats.MaxHealth;
    public float HealthPercent => CurrentHealth / MaxHealth;
    public bool IsAlive => CurrentHealth > 0f;

    public int CurrentPhaseIndex { get; private set; }  // 0 = фаза 1, 1 = фаза 2
    public PhaseData CurrentPhase => _attackLibrary != null
        ? _attackLibrary.GetPhase(CurrentPhaseIndex)
        : null;

    public bool IsAttacking { get; private set; }

    public float DistanceToPlayer {
        get {
            if (_player == null) return 999f;
            Vector3 d = _player.position - transform.position;
            d.y = 0f;
            return d.magnitude;
        }
    }

    private BossStateMachine _stateMachine;
    private BossStateFactory _factory;

    private void Awake() {
        Animator = GetComponent<BossAnimator>();
        Movement = GetComponent<BossMovement>();
        Combat = GetComponent<BossCombatController>();

        CurrentHealth = _stats.MaxHealth;
        CurrentPhaseIndex = 0;

        if (_player == null) {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) _player = p.transform;
        }

        _stateMachine = new BossStateMachine();
        _factory = new BossStateFactory(this, _stateMachine);
        _stateMachine.Initialize(_factory.Idle);
    }

    private void Update() {
        if (!IsAlive) return;
        _stateMachine.Update();
        CheckPhaseTransition();

        if (_stats.DebugMode && Time.frameCount % 30 == 0) {
            var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            UnityEngine.Debug.Log($"[STATE] {_stateMachine.CurrentState.GetType().Name}, " +
                $"dist={DistanceToPlayer:F1}, " +
                $"agentStopped={agent.isStopped}, " +
                $"agentVel={agent.velocity.magnitude:F2}, " +
                $"agentRemaining={agent.remainingDistance:F1}");
        }
    }

    public void TakeDamage(float damage) {
        if (!IsAlive) return;
        CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);
        if (_stats.DebugMode)
            Debug.Log($"[BOSS] Took {damage} damage. HP: {CurrentHealth}/{MaxHealth}");
        if (CurrentHealth <= 0f) OnDeath();
    }

    private void CheckPhaseTransition() {
        if (CurrentPhaseIndex == 0 && HealthPercent <= _stats.Phase2Threshold) {
            CurrentPhaseIndex = 1;
            if (_stats.DebugMode) Debug.Log("[BOSS] Phase 2 started!");
        }
    }

    private void OnDeath() {
        if (_stats.DebugMode) Debug.Log("[BOSS] DEAD!");
        enabled = false;
    }

    public void SetAttacking(bool v) => IsAttacking = v;

    private void OnDrawGizmosSelected() {
        if (_stats == null || _player == null) return;
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, _stats.CloseRange);
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, _stats.MidRange);
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, _stats.StrafeMinDistance);
        Gizmos.DrawWireSphere(transform.position, _stats.StrafeMaxDistance);
    }
}
using UnityEngine;

public class EnemyController : EnemyBase {

    [Header("Movement Speeds")]
    [SerializeField] private float _patrolSpeed = 2f;
    [SerializeField] private float _chaseSpeed = 4f;

    [Header("Patrol")]
    [SerializeField] private Transform[] _patrolPoints;

    [Header("Combat")]
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private float _preferredDistence = 4f;
    [SerializeField] private float _distenceTolerance = 1f;
    [SerializeField] private float _globalAttackCooldown = 2.0f;
    [SerializeField] private SimpleAttackSelector _attackSelector;

    private float _lastAttackTime = -999f;

    private EnemyIdleState _idleState;
    private EnemyPatrolState _patrolState;
    private EnemyChaseState _chaseState;
    private EnemyAttackState _attackState;

    public float PatrolSpeed => _patrolSpeed;
    public float ChaseSpeed => _chaseSpeed;
    public Transform[] PatrolPoints => _patrolPoints;
    public float AttackRange => _attackRange;
    public float PreferredDistence => _preferredDistence;
    public float DistenceTolerance => _distenceTolerance;
    public SimpleAttackSelector SimpleAttackSelector => _attackSelector;
    public ComboData PendingCombo { get; private set; }

    public void ToIdle() => ChangeState(_idleState);
    public void ToPatrol() => ChangeState(_patrolState);
    public void ToChase() => ChangeState(_chaseState);
    public void ToAttack() => ChangeState(_attackState);

    protected override void Start() {
        base.Start();

        _idleState = new EnemyIdleState(this);
        _patrolState = new EnemyPatrolState(this);
        _chaseState = new EnemyChaseState(this);
        _attackState = new EnemyAttackState(this);

        ChangeState(_idleState);
    }

    public bool CanAttack() {
        return Time.time - _lastAttackTime >= _globalAttackCooldown;
    }

    public void SetPendingCombo(ComboData combo) {
        PendingCombo = combo;
    }

    public void ClearPendingCombo() {
        PendingCombo = null;
    }

    public void NotifyAttackFinished() {
        _lastAttackTime = Time.time;
    }
}

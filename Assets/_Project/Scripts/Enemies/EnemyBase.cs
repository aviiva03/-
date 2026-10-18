using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour {

    [Header("Detection")]
    [SerializeField] protected float _detectionRange = 8f;
    [SerializeField] protected float _loseSightRange = 12f;
    [SerializeField] protected LayerMask _obstacleMask;

    [Header("References")]
    [SerializeField] protected Transform _player;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected NavMeshAgent _agent;
    [SerializeField] protected Health _health;

    public State CurrentState { get; private set; }

    public float DetectionRange => _detectionRange;
    public float LoseSightRange => _loseSightRange;
    public Animator Animator => _animator;
    public NavMeshAgent Agent => _agent;
    public Transform Player => _player;
    public Health Health => _health;

    protected virtual void Start() { }

    protected virtual void Update() {
        CurrentState?.Update();
        OnUpdate();
    }

    protected virtual void OnUpdate() { }

    public void ChangeState(State newState) {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public float DistanceToPlayer() {
        Vector3 diff = _player.position - transform.position;
        diff.y = 0f;
        return diff.magnitude;
    }

    public bool CanSeePlayer() {
        if (DistanceToPlayer() > _detectionRange) return false;

        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 target = _player.position + Vector3.up * 1f;
        Vector3 dir = target - origin;

        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dir.magnitude, _obstacleMask))
            return false;

        return true;
    }

    public void RotateTowardsPlayer(float speed = 10f) {
        Vector3 dir = _player.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion target = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * speed);
    }

    public void MoveTo(Vector3 destination) {
        _agent.isStopped = false;
        _agent.SetDestination(destination);
    }

    public void StopMoving() {
        _agent.isStopped = true;
    }
}

using UnityEngine;
using UnityEngine.AI;

public class BossMovement : MonoBehaviour {

    private BossController _boss;
    private NavMeshAgent _agent;
    private float _strafeSign = 1f;

    public Vector3 LastMoveDirection => _agent.velocity;
    public float StrafeSign => _strafeSign;

    private void Awake() {
        _boss = GetComponent<BossController>();
        _agent = GetComponent<NavMeshAgent>();

        _agent.updateRotation = false;
        _agent.isStopped = true;
    }

    // ============================================================
    // Движение
    // ============================================================

    public void ChasePlayer(float speed) {
        if (_boss.Player == null) return;

        float dist = _boss.DistanceToPlayer;
        if (dist < _agent.stoppingDistance || speed < 0.1f) {
            _agent.isStopped = true;
            return;
        }

        _agent.isStopped = false;
        _agent.speed = speed;
        _agent.SetDestination(_boss.Player.position);
    }

    public void StrafeAroundPlayer(float speed) {
        if (_boss.Player == null) return;

        Vector3 toPlayer = _boss.Player.position - transform.position;
        toPlayer.y = 0f;
        if (toPlayer.magnitude < 0.1f) return;

        Vector3 sideOffset = Vector3.Cross(Vector3.up, toPlayer.normalized) * _strafeSign;
        Vector3 targetPos = transform.position + sideOffset * 2.5f;

        _agent.isStopped = false;
        _agent.speed = speed;
        _agent.SetDestination(targetPos);
    }

    public void Stop() {
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;
        _agent.ResetPath();
    }

    public void TeleportTo(Vector3 position) => _agent.Warp(position);

    // ============================================================
    // Поворот — RotateTowards с максимальной угловой скоростью
    // ============================================================

    /// <summary>
    /// Плавный поворот с ограничением угловой скорости.
    /// rotationSpeed = градусы в секунду (например, 120 = 1/3 оборота в секунду).
    /// </summary>
    public void FaceTarget(Vector3 target, float degreesPerSecond) {
        Vector3 lookDir = target - transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            degreesPerSecond * Time.deltaTime);
    }

    /// <summary>
    /// Мгновенный поворот — ТОЛЬКО для тестов или критических случаев.
    /// В бою НЕ используем — используем FaceTarget с высокой скоростью.
    /// </summary>
    public void FaceTargetImmediate(Vector3 target) {
        Vector3 lookDir = target - transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude < 0.001f) return;

        transform.rotation = Quaternion.LookRotation(lookDir);
    }

    // ============================================================
    // Утилиты
    // ============================================================

    public void SetAgentSpeed(float speed) => _agent.speed = speed;
    public void SetStoppingDistance(float d) => _agent.stoppingDistance = d;

    public void SetStrafeSign(float sign) => _strafeSign = sign > 0 ? 1f : -1f;
    public void FlipStrafe() => _strafeSign = -_strafeSign;

    public bool IsAtDestination() {
        if (_agent.pathPending) return false;
        if (_agent.remainingDistance > _agent.stoppingDistance) return false;
        return true;
    }
}
using System.Collections;
using UnityEngine;

public class BossAttackExecutor {

    private BossController _boss;
    private BossMovement _movement;
    private BossAnimator _animator;

    public bool IsFinished { get; private set; }
    public bool IsCancelled { get; private set; }

    public BossAttackExecutor(BossController boss) {
        _boss = boss;
        _movement = boss.Movement;
        _animator = boss.Animator;
    }

    public IEnumerator Execute(AttackData attack) {
        IsFinished = false;
        IsCancelled = false;
        _boss.SetAttacking(true);

        Debug.Log($"[EXEC] START ATTACK: {attack.AttackName} (Type: {attack.Type})");

        switch (attack.Type) {
            case AttackType.Topot: yield return ExecuteTopot(attack); break;
            case AttackType.Sweep: yield return ExecuteSweep(attack); break;
            case AttackType.Jump: yield return ExecuteJump(attack); break;
            case AttackType.Earthquake: yield return ExecuteEarthquake(attack); break;
            case AttackType.RushClap: yield return ExecuteRushClap(attack); break;
            default:
                Debug.LogWarning($"[EXEC] Unknown type: {attack.Type}");
                break;
        }

        _boss.SetAttacking(false);
        Debug.Log($"[EXEC] FINISH ATTACK: {attack.AttackName}");
        IsFinished = true;
    }

    public void Cancel() {
        IsCancelled = true;
        Debug.Log("[EXEC] CANCELLED");
    }

    private void DebugHit(string attackName) {
        if (_boss.Player == null) {
            Debug.LogWarning("[EXEC] Player is null");
            return;
        }
        Debug.Log($"[EXEC] {attackName} HIT CHECK");
        //CheckHitInFront(_boss.CurrentAttackDamage); // временно, если нужно
    }

    private IEnumerator ExecuteTopot(AttackData a) {
        _movement.Stop();
        TurnToPlayer(0.3f);
        yield return new WaitForSeconds(a.Windup);
        if (IsCancelled) yield break;

        CheckHitInFront(a.Damage);
        yield return new WaitForSeconds(a.ImpactTime);
        if (IsCancelled) yield break;

        yield return new WaitForSeconds(a.RecoveryTime);
    }

    private IEnumerator ExecuteSweep(AttackData a) {
        _movement.Stop();
        TurnToPlayer(0.3f);
        yield return new WaitForSeconds(a.Windup);
        if (IsCancelled) yield break;

        CheckHitInFront(a.Damage);
        yield return new WaitForSeconds(a.ImpactTime);
        if (IsCancelled) yield break;

        int repeats = Random.Range(a.MinRepeats, a.MaxRepeats + 1);
        for (int i = 0; i < repeats; i++) {
            yield return new WaitForSeconds(a.RepeatGap);
            if (IsCancelled) yield break;

            TurnToPlayer(0.15f);
            yield return new WaitForSeconds(a.RepeatWindup);
            if (IsCancelled) yield break;

            CheckHitInFront(a.Damage);
            yield return new WaitForSeconds(a.ImpactTime);
        }
        yield return new WaitForSeconds(a.RecoveryTime);
    }

    private IEnumerator ExecuteJump(AttackData a) {
        _movement.Stop();
        TurnToPlayer(0.3f);
        yield return new WaitForSeconds(a.JumpWindup);
        if (IsCancelled) yield break;

        // ”прощЄнный прыжок (без визуала, телепорт к игроку)
        Vector3 startPos = _boss.transform.position;
        Vector3 targetPos = _boss.Player.position;
        _boss.transform.position = targetPos;

        CheckHitAround(_boss.transform.position, a.HitRadius, a.Damage);
        yield return new WaitForSeconds(a.JumpAirTime + a.JumpRecovery);
    }

    private IEnumerator ExecuteEarthquake(AttackData a) {
        _movement.Stop();
        TurnToPlayer(0.3f);
        yield return new WaitForSeconds(a.EarthquakeChargeTime);
        if (IsCancelled) yield break;

        for (int i = 0; i < a.EarthquakeExplosionCount; i++) {
            if (IsCancelled) yield break;
            float radius = a.EarthquakeMaxRadius * ((i + 1f) / a.EarthquakeExplosionCount);
            CheckHitAround(_boss.transform.position, radius, a.Damage);
            Debug.Log($"[EXEC] Earthquake boom {i + 1}, radius {radius:F1}");
            yield return new WaitForSeconds(a.EarthquakeExplosionDelay);
        }
        yield return new WaitForSeconds(a.RecoveryTime);
    }

    private IEnumerator ExecuteRushClap(AttackData a) {
        _movement.Stop();
        TurnToPlayer(0.3f);
        yield return new WaitForSeconds(a.RushWindup);
        if (IsCancelled) yield break;

        // –ывок
        float elapsed = 0f;
        Vector3 dir = _boss.transform.forward;
        while (elapsed < a.RushDuration) {
            if (IsCancelled) yield break;
            elapsed += Time.deltaTime;
            _boss.transform.position += dir * a.RushSpeed * Time.deltaTime;
            yield return null;
        }

        CheckHitAround(_boss.transform.position, a.ClapRadius, a.Damage);
        yield return new WaitForSeconds(a.RecoveryTime);
    }

    private IEnumerator TurnToPlayer(float duration) {
        if (_boss.Player == null) yield break;
        float elapsed = 0f;
        while (elapsed < duration) {
            if (IsCancelled) yield break;
            Vector3 dir = _boss.Player.position - _boss.transform.position;
            dir.y = 0;
            if (dir.sqrMagnitude > 0.001f) {
                _boss.transform.rotation = Quaternion.RotateTowards(
                    _boss.transform.rotation,
                    Quaternion.LookRotation(dir),
                    240f * Time.deltaTime);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void CheckHitInFront(float damage) {
        if (_boss.Player == null) return;
        Vector3 center = _boss.transform.position + _boss.transform.forward * 1.5f + Vector3.up * 0.5f;
        CheckHitAt(center, 2.5f, damage);
    }

    private void CheckHitAround(Vector3 center, float radius, float damage) {
        CheckHitAt(center, radius, damage);
    }

    private void CheckHitAt(Vector3 center, float radius, float damage) {
        Collider[] hits = Physics.OverlapSphere(center, radius, ~0, QueryTriggerInteraction.Ignore);
        foreach (var hit in hits) {
            if (!hit.CompareTag("Player")) continue;
            var health = hit.GetComponent<PlayerHealth>();
            if (health != null) {
                //health.TakeDamage(damage);
                Debug.Log($"[EXEC] HIT PLAYER for {damage}");
            }
        }
    }
}
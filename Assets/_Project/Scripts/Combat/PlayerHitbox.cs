using System.Collections.Generic;
using UnityEngine;

/// Хитбокс игрока. Активируется из состояния атаки,
/// проверяет пересечение с IDamageable, наносит урон.
/// 
/// Живёт на дочернем GameObject, который двигается вместе с игроком.
public class PlayerHitbox : MonoBehaviour {

    [Header("Config")]
    [SerializeField] private float _radius = 0.6f;
    [SerializeField] private LayerMask _hitLayers;

    [Header("Debug")]
    [SerializeField] private bool _debugMode = true;

    // Текущее состояние
    private bool _isActive;
    private float _currentDamage;
    private Vector3 _hitDirection;

    // Защита от повторного урона в одной атаке
    private readonly HashSet<IDamageable> _alreadyHit = new();

    public bool IsActive => _isActive;
    public float Radius => _radius;

    /// Включить хитбокс. Вызывается из состояния атаки.
    public void Activate(float damage, Vector3 hitDirection, float radius = -1f) {
        if (radius > 0f) _radius = radius;
        _isActive = true;
        _currentDamage = damage;
        _hitDirection = hitDirection.normalized;
        _alreadyHit.Clear();

        // Один раз проверяем — не каждый кадр
        PerformHitCheck();

        if (_debugMode) {
            Debug.Log($"[HITBOX] Activated, damage={damage}, dir={_hitDirection}");
        }
    }

    public void Deactivate() {
        _isActive = false;
        _alreadyHit.Clear();
    }

    /// Основная проверка попадания — один раз при активации.
    private void PerformHitCheck() {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            _radius,
            _hitLayers,
            QueryTriggerInteraction.Collide);

        foreach (var col in hits) {
            // Найти IDamageable — сначала на самом объекте, потом в родителях
            IDamageable damageable = FindDamageable(col);
            if (damageable == null) continue;
            if (!damageable.IsAlive) continue;
            if (_alreadyHit.Contains(damageable)) continue;

            // Урон!
            _alreadyHit.Add(damageable);
            Vector3 hitPoint = col.ClosestPoint(transform.position);
            damageable.TakeDamage(_currentDamage, hitPoint, _hitDirection);

            if (_debugMode) {
                Debug.Log($"[HITBOX] Hit {damageable.Transform.name} for {_currentDamage}");
            }
        }
    }

    /// Ищет IDamageable в коллайдере и его родителях (до 5 уровней вверх).
    private IDamageable FindDamageable(Collider col) {
        // Сначала на самом коллайдере
        if (col.TryGetComponent<IDamageable>(out var d)) return d;

        // Потом в родителях — враг может быть "родителем" с дочерними коллайдерами
        Transform t = col.transform.parent;
        int depth = 0;
        while (t != null && depth < 5) {
            if (t.TryGetComponent<IDamageable>(out d)) return d;
            t = t.parent;
            depth++;
        }
        return null;
    }

    // ============================================================
    // Визуализация
    // ============================================================

    private void OnDrawGizmos() {
        if (!_debugMode) return;

        Gizmos.color = _isActive
            ? new Color(1f, 0f, 0f, 0.7f)   // красная — активен
            : new Color(1f, 1f, 0f, 0.3f);  // жёлтая — неактивен
        Gizmos.DrawWireSphere(transform.position, _radius);

        if (_isActive) {
            Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
            Gizmos.DrawSphere(transform.position, _radius);
        }
    }
}
using UnityEngine;
public interface IDamageablee {
    void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection);
    bool IsAlive { get; }
    Transform Transform { get; }
}
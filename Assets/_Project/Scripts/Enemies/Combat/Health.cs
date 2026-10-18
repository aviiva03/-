using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable {

    [SerializeField] private int _maxHealth = 100;
    private int _currentHealth;

    public event Action<int, int> OnHealthChanged;
    public event Action OnDied;

    public bool IsAlive => _currentHealth > 0;
    public int Current => _currentHealth;
    public int Max => _maxHealth;

    private void Awake() { _currentHealth = _maxHealth; }

    private void Start() { OnHealthChanged?.Invoke(_currentHealth, _maxHealth); }

    public void TakeDamage(int amount, Vector3 hitPoint) {
        if (!IsAlive) return;

        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

        if(!IsAlive)
            OnDied?.Invoke();
    }

    public void Heal(int amount) {
        if (!IsAlive) return;

        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }
}

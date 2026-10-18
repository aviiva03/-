using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour {

    [SerializeField] private Health _health;
    [SerializeField] private Image _fillImage;

    private void Start() {
        _health.OnHealthChanged += UpdateBar;
        _health.OnDied += HandleDeath;
    }

    private void OnDestroy() {
        _health.OnHealthChanged -= UpdateBar;
        _health.OnDied -= HandleDeath;
    }

    private void UpdateBar(int current, int max) {
        _fillImage.fillAmount = (float)current / max;
    }

    private void HandleDeath() {
        Debug.Log("Игрок умер!");
    }
}

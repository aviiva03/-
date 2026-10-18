using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Player/Stats")]
public class PlayerStats : ScriptableObject {

    [Header("Movement")]
    public float WalkSpeed = 4f;
    public float RunSpeed = 8f;
    public float RotationSpeed = 15f;
    public float Acceleration = 12f;

    [Header("Jump & Gravity")]
    public float JumpHeight = 1.5f;
    public float Gravity = -25f;

    [Header("Stamina")]
    public float MaxStamina = 100f;
    public float StaminaDrainPerSec = 15f;
    public float StaminaRegenPerSec = 20f;
    public float RegenDelay = 1f;

    [Header("Dodge")]
    public float DodgeSpeed = 12f;
    public float DodgeDuration = 0.3f;
    public float DodgeCooldown = 0.8f;
    public float DodgeStaminaCost = 25f;

    [Header("Game Feel")]
    [Tooltip("Сколько секунд после схода с платформы ещё можно прыгнуть")]
    public float CoyoteTime = 0.15f;
    [Tooltip("Сколько секунд запоминается нажатие прыжка до приземления")]
    public float JumpBufferTime = 0.15f;
    [Tooltip("Множитель гравитации при падении (для резкого прыжка)")]
    public float FallGravityMultiplier = 2f;
    [Tooltip("Множитель гравитации при отпускании кнопки прыжка (для короткого прыжка)")]
    public float LowJumpMultiplier = 3f;

    [System.NonSerialized] public float CurrentStamina;
    [System.NonSerialized] public float LastStaminaUseTime;

    public bool HasStamina(float amount) => CurrentStamina >= amount;
    
    public void ConsumeStamina(float amount) {
        CurrentStamina = Mathf.Max(0f, CurrentStamina - amount);
        LastStaminaUseTime = Time.time;
    }

    public void RegenStamina(float deltaTime) {
        if (Time.time - LastStaminaUseTime < RegenDelay) return;
        CurrentStamina = Mathf.Min(MaxStamina, CurrentStamina + StaminaRegenPerSec * deltaTime);
    }
}

using UnityEngine;

public enum AttackType {
    Topot,          // топтушки
    Sweep,          // взмахи руками
    Jump,           // прыжок
    Earthquake,     // землетрясение
    RushClap        // рывок + хлопок
}

[CreateAssetMenu(fileName = "Attack", menuName = "Boss/Attack Data")]
public class AttackData : ScriptableObject {

    [Header("Identity")]
    public AttackType Type;
    public string AttackName = "Attack";

    [Header("Damage")]
    public float Damage = 15f;
    public float HitRadius = 2.5f;
    public float HitForwardOffset = 1.5f;

    [Header("Timings")]
    [Tooltip("Замах первой итерации")]
    public float Windup = 0.6f;
    [Tooltip("Замах повторных итераций (если RepeatCount > 0)")]
    public float RepeatWindup = 0.3f;
    [Tooltip("Момент удара")]
    public float ImpactTime = 0.15f;
    [Tooltip("Восстановление")]
    public float RecoveryTime = 0.4f;
    [Tooltip("Пауза между итерациями")]
    public float RepeatGap = 0.3f;

    [Header("Repeats")]
    public int MinRepeats = 0;
    public int MaxRepeats = 2;

    [Header("Animator")]
    [Tooltip("Имя триггера в Animator (например, 'TopotTrigger')")]
    public string AnimatorTriggerName = "";

    // ============================================================
    // Earthquake-специфичное
    // ============================================================
    [Header("Earthquake")]
    public float EarthquakeMaxRadius = 8f;
    public int EarthquakeMaxCharge = 4;
    public float EarthquakeChargeTime = 2f;
    public float EarthquakeExplosionDelay = 0.3f;
    public int EarthquakeExplosionCount = 6;

    // ============================================================
    // Jump-специфичное
    // ============================================================
    [Header("Jump")]
    public float JumpWindup = 0.5f;
    public float JumpAirTime = 0.7f;
    public float JumpRecovery = 0.6f;
    public float JumpMaxRange = 10f;

    // ============================================================
    // Rush-специфичное
    // ============================================================
    [Header("Rush Clap")]
    public float RushWindup = 0.6f;
    public float RushSpeed = 10f;
    public float RushDuration = 0.3f;
    public float ClapRadius = 3f;
}
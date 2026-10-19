using UnityEngine;

[CreateAssetMenu(fileName = "BossStats", menuName = "Boss/Stats")]
public class BossStats : ScriptableObject {

    [Header("Health")]
    public float MaxHealth = 1000f;

    [Header("Movement")]
    public float WalkSpeed = 2.5f;
    public float ChaseSpeed = 4f;
    public float StrafeSpeed = 2f;
    [Tooltip("Скорость поворота в градусах/сек")]
    public float TurnDegreesPerSecond = 90f;

    [Header("Phases")]
    [Range(0.1f, 0.9f)] public float Phase2Threshold = 0.5f;

    [Header("Combat")]
    public float MinComboDelay = 2f;
    public float MaxComboDelay = 4f;
    public float CloseRange = 5f;
    public float MidRange = 10f;

    [Header("Strafe")]
    public float StrafeMinDistance = 4f;
    public float StrafeMaxDistance = 7f;
    public float StrafeDirectionChangeTime = 2.5f;

    [Header("Debug")]
    public bool DebugMode = true;
}
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Combo", menuName = "Boss/Combo Data")]
public class ComboData : ScriptableObject {

    [Header("Identity")]
    public string ComboName = "Combo";

    [Header("Conditions")]
    [Tooltip("Мин. HP% босса (0-1)")]
    [Range(0f, 1f)] public float MinHealthPercent = 0f;
    [Tooltip("Макс. HP% босса (0-1)")]
    [Range(0f, 1f)] public float MaxHealthPercent = 1f;

    [Tooltip("Мин. дистанция до игрока")]
    public float MinDistance = 0f;
    [Tooltip("Макс. дистанция до игрока")]
    public float MaxDistance = 5f;

    [Header("Attacks — по порядку")]
    public List<AttackData> Attacks = new();

    /// <summary>
    /// Подходит ли комбо в текущих условиях?
    /// </summary>
    public bool CanExecute(float bossHealthPercent, float distanceToPlayer) {
        if (bossHealthPercent < MinHealthPercent) return false;
        if (bossHealthPercent > MaxHealthPercent) return false;
        if (distanceToPlayer < MinDistance) return false;
        if (distanceToPlayer > MaxDistance) return false;
        if (Attacks == null || Attacks.Count == 0) return false;
        return true;
    }
}
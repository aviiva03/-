using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossAttackLibrary", menuName = "Boss/Attack Library")]
public class BossAttackLibrary : ScriptableObject {

    [Header("Phases — по порядку (индекс 0 = фаза 1)")]
    public List<PhaseData> Phases = new();

    /// <summary>
    /// Получить фазу по индексу (0-based). Если индекс больше — берём последнюю.
    /// </summary>
    public PhaseData GetPhase(int phaseIndex) {
        if (Phases == null || Phases.Count == 0) return null;
        int idx = Mathf.Clamp(phaseIndex, 0, Phases.Count - 1);
        return Phases[idx];
    }
}
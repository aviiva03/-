using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Phase", menuName = "Boss/Phase Data")]
public class PhaseData : ScriptableObject {

    [Header("Identity")]
    public string PhaseName = "Phase 1";

    [Header("Stats modifiers")]
    [Tooltip("Множитель скорости движения в этой фазе")]
    public float SpeedMultiplier = 1f;
    [Tooltip("Множитель агрессии (короче паузы между комбо)")]
    public float AggressionMultiplier = 1f;

    [Header("Combos")]
    public List<ComboData> Combos = new();

    /// <summary>
    /// Выбрать подходящее комбо (случайное из тех, что подходят).
    /// </summary>
    public ComboData PickCombo(float healthPercent, float distance) {
        List<ComboData> valid = new();
        foreach (var c in Combos)
            if (c.CanExecute(healthPercent, distance)) valid.Add(c);

        if (valid.Count == 0) return null;
        return valid[Random.Range(0, valid.Count)];
    }
}
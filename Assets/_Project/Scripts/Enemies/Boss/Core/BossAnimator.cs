using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class BossAnimator : MonoBehaviour {

    private static readonly int StateHash = Animator.StringToHash("State");
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int StrafeHash = Animator.StringToHash("Strafe");

    public enum AnimState {
        Idle = 0,
        Locomotion = 1,
        Strafe = 2,
        Topot = 10,
        Sweep = 20,
        Jump = 30,
        Earthquake = 40,
        Rush = 50
    }

    private Animator _anim;
    public AnimState CurrentState { get; private set; } = AnimState.Idle;

    private void Awake() {
        _anim = GetComponent<Animator>();
    }

    public void SetState(AnimState state) {
        if (_anim == null) return;
        if (CurrentState == state) return;

        CurrentState = state;
        _anim.SetInteger(StateHash, (int)state);
    }

    public void SetSpeed(float normalized) {
        if (_anim == null) return;
        _anim.SetFloat(SpeedHash, Mathf.Clamp01(normalized), 0.15f, Time.deltaTime);
    }

    public void SetStrafe(float value) {
        if (_anim == null) return;
        _anim.SetFloat(StrafeHash, Mathf.Clamp(value, -1f, 1f), 0.15f, Time.deltaTime);
    }
}
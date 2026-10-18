using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int GroundedHash = Animator.StringToHash("Grounded");
    private static readonly int VertVelHash = Animator.StringToHash("VerticalVelocity");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int DodgeHash = Animator.StringToHash("Dodge");

    private Animator _anim;

    private void Awake() => _anim = GetComponent<Animator>();

    public void SetSpeed(float normalized) => _anim.SetFloat(SpeedHash, normalized, 0.1f, Time.deltaTime);
    public void SetGrounded(bool g) => _anim.SetBool(GroundedHash, g);
    public void SetVerticalVelocity(float v) => _anim.SetFloat(VertVelHash, v);
    public void PlayJump() => _anim.SetTrigger(JumpHash);
    public void PlayDodge() => _anim.SetTrigger(DodgeHash);
}

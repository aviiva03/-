using UnityEngine;

public class PlayerIdleState : PlayerState {

    public PlayerIdleState(PlayerController c, PlayerStateMachine m, PlayerStateFactory f) : base(c, m, f) { }

    public override void Enter() {
        Controller.MoveDirection = Vector3.zero;
        Controller.ApplyMovement();
        Animator.SetSpeed(0f);
    }

    public override void Update() {
        if (Controller.WantsToDodge)
            StateMachine.ChangeState(Factory.Dodge);
        else if (Controller.WantsToJump)
            StateMachine.ChangeState(Factory.Jump);
        else if (!Controller.IsGrounded)
            StateMachine.ChangeState(Factory.Fall);
        else if (Controller.HasMoveInput)
            StateMachine.ChangeState(Factory.Run);
    }
}

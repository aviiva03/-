public class PlayerRunState : PlayerState {

    public PlayerRunState(PlayerController c, PlayerStateMachine m, PlayerStateFactory f) : base(c, m, f) { }

    public override void Update() {
        if (Controller.WantsToDodge) {
            StateMachine.ChangeState(Factory.Dodge);
            return;
        }
        if (Controller.WantsToJump) {
            StateMachine.ChangeState(Factory.Jump);
            return;
        }
        if (!Controller.IsGrounded) {
            StateMachine.ChangeState(Factory.Fall);
            return;
        }
        if (!Controller.HasMoveInput) {
            StateMachine.ChangeState(Factory.Idle);
            return;
        }

        Controller.ApplyMovement();
    }
}

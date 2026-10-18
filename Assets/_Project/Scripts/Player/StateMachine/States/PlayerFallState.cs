public class PlayerFallState : PlayerState {

    public PlayerFallState(PlayerController c, PlayerStateMachine m, PlayerStateFactory f) : base(c, m, f) { }

    public override void Enter() {
        // Можно проиграть анимацию падения
    }

    public override void Update() {
        // Двигаемся в воздухе
        Controller.ApplyMovement();

        if (Controller.IsGrounded) {
            StateMachine.ChangeState(Factory.Land);
            return;
        }

        // Рывок в воздухе
        if (Controller.WantsToDodge)
            StateMachine.ChangeState(Factory.Dodge);
    }
}

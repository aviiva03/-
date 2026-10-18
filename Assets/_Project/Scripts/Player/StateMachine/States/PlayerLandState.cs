using UnityEngine;

public class PlayerLandState : PlayerState {

    private float _landTimeLeft;

    public PlayerLandState(PlayerController c, PlayerStateMachine m, PlayerStateFactory f) : base(c, m, f) { }

    public override void Enter() {
        _landTimeLeft = 0.15f;
        Controller.MoveDirection = Vector3.zero;
        Controller.ApplyMovement();
        // Animator.PlayLand();
    }

    public override void Update() {
        _landTimeLeft -= Time.deltaTime;
        if (_landTimeLeft <= 0f) {
            if (Controller.HasMoveInput)
                StateMachine.ChangeState(Factory.Run);
            else
                StateMachine.ChangeState(Factory.Idle);
        }
    }
}

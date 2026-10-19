public class BossStateMachine {
    public BossState CurrentState { get; private set; }

    public void Initialize(BossState startingState) {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(BossState newState) {
        if (newState == CurrentState) return;

        UnityEngine.Debug.Log($"[FSM] {CurrentState?.GetType().Name} -> {newState?.GetType().Name}");

        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update() {
        CurrentState.Update();
    }
}
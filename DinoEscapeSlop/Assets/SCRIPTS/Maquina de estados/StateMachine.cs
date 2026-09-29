public class StateMachine
{
    public IState CurrentState { get; private set; }

    public void Initialize(IState initialState)
    {
        CurrentState = initialState;
        CurrentState.Enter();
    }

    public void ChangeState(IState newState)
    {
        if (newState == null || newState == CurrentState) return;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }
}
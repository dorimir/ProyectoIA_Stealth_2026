public class StateMachine
{
    private IState currentState;

    public void Initialze(IState startingState)
    {
        currentState = startingState;
        currentState.Enter();
    }

    public void ChangeState(IState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        currentState?.Execute();
    }
}
public abstract class PlayerBaseState
{
    #region Variables
    // Variables.
    protected PlayerStateMachine ctx;
    protected PlayerStateFactory factory;
    #endregion Veriables

    #region Public Methods
    // Public Methods.
    public PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory stateFactory)
    {
        ctx = currentContext;
        factory = stateFactory;
    }

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void ExitState();

    public abstract void CheckSwitchStates();

    public abstract void InitializeSubState();
    #endregion

    #region Private Methods
    // Private Methods.
    private void UpdateStates() 
    {

    }

    protected void SwitchState(PlayerBaseState newState) 
    {
        // Exit current state.
        ExitState();

        // Enter new state.
        newState.EnterState();

        ctx.CurrentState = newState;
    }

    protected void SetSuperState() 
    { 
        
    }

    protected void SetSubState() 
    {
        
    }
    #endregion
}

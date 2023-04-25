using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    #region Variables
    // Variables.
    
    #endregion

    #region Unity Methods

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    #endregion

    #region Private Methods
    // Private Methods.
    private void HandleJump()
    {
        /*if (groundedTimer > 0)
        {
            // When the player jumps on the ground they are no longer grounded, but can still double jump.
            groundedTimer = 0;
            jumpDelayTimer = jumpDelay;
            // Physics dynamics formula for calculating jump velocity based on height and gravity.
            verticalVelocity = Mathf.Sqrt(jumpHeight * -3 * gravity);
        }
        else if (multiJumpAmount > 0)
        {
            multiJumpAmount--;
            jumpDelayTimer = jumpDelay;
            // Physics dynamics formula for calculating jump velocity based on height and gravity.
            verticalVelocity = Mathf.Sqrt(multiJumpHeight * -3 * gravity);
        }*/
    }
    #endregion

    #region Public Methods
    // Public Methods.
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory stateFactory) : base(currentContext, stateFactory) { }

    public override void EnterState()
    {
        HandleJump();
    }
    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }

    public override void ExitState()
    {
        throw new System.NotImplementedException();
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    public override void CheckSwitchStates()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}

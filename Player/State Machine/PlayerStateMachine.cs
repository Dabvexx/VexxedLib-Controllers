using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
public class PlayerStateMachine : MonoBehaviour
{
    #region Variables
    // Variables.
    // public static PlayerStateMachine Instance;

    public CharacterController controller;

    private PlayerBaseState currentState;

    private PlayerStateFactory states;

    // Input action vars.
    private PlayerInput playerInput;
    private InputAction jumpAction;
    private InputAction moveAction;
    private InputAction dashAction;
    private InputAction sprintAction;

    // Movement Vars
    private float verticalVelocity;
    private Vector3 prevDir;
    private Vector3 move;

    [Space(10), Header("Ground Variables")]
    [SerializeField, Tooltip("Speed of player on the ground.")] private float groundSpeed = 15f;
    [SerializeField, Tooltip("Speed of player on the ground.")] private float sprintSpeed = 25f;
    [SerializeField, Tooltip("Speed of player turning to face the movement angle.")] private float turnSpeed = 2f;
    // The hit data of the ground the controller is standing on.
    [HideInInspector] public ControllerColliderHit groundData { get; private set; } = null;
    [HideInInspector] public bool isPlayerGrounded { get; private set; } = true;
    private bool isSprinting = false;

    [Space(10), Header("Sliding Variables")]
    [SerializeField, Tooltip("Speed of player sliding down a slope."), Min(0f)] private float slidingSpeed = 10f;
    [SerializeField, Tooltip("The slope of the ground below the player before they player will slide down it.")] private float slideSlopeLimit = 60f;
    [SerializeField, Tooltip("The slope of the ground where the player loses the ability to jump out of the slope.")] private float maxSlopeLimit = 80f;
    private bool isSliding = false;


    [Space(10), Header("Air Variables")]
    [SerializeField, Tooltip("Speed of player in the air.")] private float airSpeed = 13f;
    [SerializeField, Tooltip("Speed of player in the air.")] private float airSprintSpeed = 20f;
    [SerializeField, Tooltip("Amount of time before the player can jump again."), Range(0f, 2f)] private float jumpDelay = .2f;
    [SerializeField, Tooltip("Height of jump off the ground."), Min(0f)] private float jumpHeight = 2.0f;
    [SerializeField, Tooltip("Amount of jumps in the air off the ground."), Min(0)] private int maxMultiJumps = 1;
    [SerializeField, Tooltip("Height of jumps in midair."), Min(0)] private float multiJumpHeight = 1.0f;
    [SerializeField, Tooltip("How fast the player is pulled down.")] public float gravity = -9.81f;
    // The current jumps you have.
    private int multiJumpAmount;
    // The current timer counting down before the player can jump.
    private float jumpDelayTimer = 0f;
    // ground timer so the player can go down slopes and stairs without being off the ground.
    private float groundedTimer = 0f;


    [Space(10), Header("Wall Jump Variables")]
    [SerializeField, Tooltip("The strength after jumping off a wall")] private float jumpForce = 5;
    [SerializeField, Tooltip("The amount of degrees off 90 degrees will still count as being able to wall slide.")] private float angleTolerance = 5;
    //Check to see if player is touching a wall, maybe begin a wall slide.
    private bool isTouchingWall = false;

    //[Space(10), Header("Below Is Currently Unused")]

    //[Space(10), Header("Dash Variables")]
    //[SerializeField] private float dashSpeed;
    //[SerializeField] private Vector3 dashDir;
    //[SerializeField] private Vector3 dashVelocity;
    //[SerializeField] private bool hasDash = true;
    //[SerializeField] private bool isDashing = false;
    #endregion

    #region Getter Setters
    public PlayerBaseState CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }

    public CharacterController Controller 
    { 
        get { return controller; }
        private set { controller = value; }
    }

    #endregion Getter Setters

    #region Unity Methods

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        jumpAction = playerInput.actions["Jump"];
        moveAction = playerInput.actions["Move"];
        dashAction = playerInput.actions["Dash"];
        sprintAction = playerInput.actions["Sprint"];


        multiJumpAmount = maxMultiJumps;
        groundData = new ControllerColliderHit();
        slideSlopeLimit = controller.slopeLimit;

        states = new PlayerStateFactory(this);
        currentState = states.Grounded();
        currentState.EnterState();
    }

    void Update()
    {
        
    }

    #endregion

    #region Private Methods
    // Private Methods.
    
    #endregion

    #region Public Methods
    // Public Methods.
    
    #endregion
}

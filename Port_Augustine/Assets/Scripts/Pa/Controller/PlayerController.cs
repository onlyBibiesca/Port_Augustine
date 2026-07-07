using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{

    public static PlayerMovement Instance;

    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip[] footstepClips;

    [SerializeField] private float walkInterval = 0.45f;

    private float footstepTimer;

    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject sureButton;
    [SerializeField] public Animator _Animator;

    public AudioSource source;
    public AudioClip clip;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    public static bool isInventoryClick, isDebugClick, isQuestClick, isPausePressed = false;

    private bool isDialogueActive = false;


    [HideInInspector] public float moveSpeedModifier = 0f; // Trait system will modify this

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isDialogueActive)
        {
            rb.velocity = Vector2.zero;
            moveInput = Vector2.zero;
        }
        else
        {
            rb.velocity = moveInput * (moveSpeed + moveSpeedModifier);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (isDialogueActive)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = context.ReadValue<Vector2>();
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            _Animator.SetBool("IsMovingRight", true);
            _Animator.SetBool("IsMovingLeft", false);
            _Animator.SetBool("IsMovingUp", false);
            _Animator.SetBool("IsMovingDown", false);
            _Animator.SetBool("StandingStill", false);
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            _Animator.SetBool("IsMovingLeft", true);
            _Animator.SetBool("IsMovingRight", false);
            _Animator.SetBool("IsMovingUp", false);
            _Animator.SetBool("IsMovingDown", false);
            _Animator.SetBool("StandingStill", false);
        }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            _Animator.SetBool("IsMovingRight", false);
            _Animator.SetBool("IsMovingLeft", false);
            _Animator.SetBool("IsMovingUp", true);
            _Animator.SetBool("IsMovingDown", false);
            _Animator.SetBool("StandingStill", false);
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            _Animator.SetBool("IsMovingDown", true);
            _Animator.SetBool("IsMovingRight", false);
            _Animator.SetBool("IsMovingLeft", false);
            _Animator.SetBool("IsMovingUp", false);
            _Animator.SetBool("StandingStill", false);
        }
        else
        {
            _Animator.SetBool("IsMovingRight", false);
            _Animator.SetBool("IsMovingLeft", false);
            _Animator.SetBool("IsMovingUp", false);
            _Animator.SetBool("IsMovingDown", false);
            _Animator.SetBool("StandingStill", true);
        }
    }

    public void DisableMovement()
    {
        isDialogueActive = true;
        moveInput = Vector2.zero;
        rb.velocity = Vector2.zero;

        _Animator.SetBool("IsMovingRight", false);
        _Animator.SetBool("IsMovingLeft", false);
        _Animator.SetBool("IsMovingUp", false);
        _Animator.SetBool("IsMovingDown", false);
        _Animator.SetBool("StandingStill", true);

        Debug.Log("Player movement disabled");
    }

    public void EnableMovement()
    {
        isDialogueActive = false;
        Debug.Log("Player movement enabled");
    }

    public void Click(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            source.PlayOneShot(clip);
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
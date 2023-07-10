using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Animator _anim;
    private string currentState;

    private float xAxis;
    private float yAxis;
    private Rigidbody2D rb2d;
    private bool isJumpPressed;
    [SerializeField] private float jumpForce = 25f;
    private int groundMask;
    private bool isGrounded;
    private string currentAnimation;
    private bool isAttackPressed;
    private bool isAttacking;

    [SerializeField] private float attackDelay = 0.3f;
    [SerializeField] private float walkSpeed = 5.0f;

    //animation states
    const string PLAYER_IDLE = "Player_idle";
    const string PLAYER_RUN = "Player_run";
    const string PLAYER_JUMP = "Player_jump";
    const string PLAYER_ATTACK = "Player_attack";
    const string PLAYER_AIR_ATTACK = "Player_air_atack";


    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        groundMask = 1 << LayerMask.NameToLayer("Ground");
    }

    // Update is called once per frame
    void Update()
    {
        //check for inputs
        xAxis = Input.GetAxisRaw("Horizontal");

        //space jump key pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJumpPressed = true;
        }

        //space attack key pressed
        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            isAttackPressed = true;
        }
    }

    private void FixedUpdate()
    {
        //check if player is on the ground
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundMask);

        if(hit.collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        //check movement update
        Vector2 vel = new Vector2(0, rb2d.velocity.y);

        if (xAxis < 0)
        {
            vel.x = -walkSpeed;
            transform.localScale = new Vector2(-1, 1);
        }
        else if(xAxis > 0)
        {
            vel.x = walkSpeed;
            transform.localScale = new Vector2(1, 1);            
        }
        else
        {
            vel.x = 0;            
        }

        if (isGrounded && !isAttacking)
        {
            if(xAxis != 0)
            {
                ChangeAnimationState(PLAYER_RUN);
            }
            else
            {
                ChangeAnimationState(PLAYER_IDLE);
            }
        }

        //check for jumping
        if(isJumpPressed && isGrounded)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce * Time.deltaTime);
            isJumpPressed = false;
            ChangeAnimationState(PLAYER_JUMP);
        }

        //assign the new velocity to the rigidbody
        rb2d.velocity = vel;

        //attack
        if (isAttackPressed)
        {
            isAttackPressed = false;

            if (!isAttacking)
            {
                isAttacking = true;

                if (isGrounded)
                {
                    ChangeAnimationState(PLAYER_ATTACK);
                }
                else
                {
                    ChangeAnimationState(PLAYER_AIR_ATTACK);
                }

                attackDelay = _anim.GetCurrentAnimatorStateInfo(0).length;
                Invoke("AttackComplete", attackDelay);
            }
        }
    }

    private void AttackComplete()
    {
        isAttacking = false;
    }

    private void ChangeAnimationState(string newState)
    {
        //stop the same anim from interupting
        if (currentState == newState) return;

        //paly the animation
        _anim.Play(newState);

        //reassign the current state
        currentState = newState;
    }


}

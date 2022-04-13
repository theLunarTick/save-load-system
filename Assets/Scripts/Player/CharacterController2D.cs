﻿using System.Collections;
using UnityEngine;

// This script is a basic 2D character controller that allows
// the player to run and jump.

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]


public class CharacterController2D : MonoBehaviour, IDataPersistence
{

    [Header("Movement Params")]
    [SerializeField] private float runSpeed = 6.0f;
    [SerializeField] private float jumpSpeed = 8.0f;
    [SerializeField] private float gravityScale = 20.0f;

    [Header("Respawn Point")]
    [SerializeField] private Transform respawnPoint;


    //
    // components attached to player

    private BoxCollider2D coll;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sr;
    private ParticleSystem deathParticles;


    //
    // input parameters for movement

    Vector2 moveDirection = Vector2.zero;
    bool jumpPressed = false;


    //
    // other

    private bool facingRight = true;
    private bool isGrounded = false;
    private bool disableMovement = false;


    //
    // LOAD PLAYER COMPONENTS

    private void Awake()
    {
        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        deathParticles = GetComponentInChildren<ParticleSystem>();

        deathParticles.Stop();

        rb.gravityScale = gravityScale;
    }


    //
    // LOAD SAVED PLAYER POSITION

    public void LoadData(GameData data) 
    {
        this.transform.position = data.playerPosition;
    }


    //
    // SAVE UPDATED PLAYER POSITION

    public void SaveData(GameData data) 
    {
        data.playerPosition = this.transform.position;
    }


    //
    // PLAYER MOVEMENT

    private void FixedUpdate()
    {
        if (disableMovement) 
        {
            return;
        }

        HandleInput();

        UpdateIsGrounded();

        HandleHorizontalMovement();

        HandleJumping();

        UpdateFacingDirection();

        UpdateAnimator();
    }


    //
    // NEW INPUTSYSTEM

    private void HandleInput() 
    {
        moveDirection = InputManager.instance.GetMoveDirection();
        jumpPressed = InputManager.instance.GetJumpPressed();
    }


    //
    // CHECK IS-GROUNDED

    private void UpdateIsGrounded()
    {
        Bounds colliderBounds = coll.bounds;
        // Debug.Log("collidersBounds = " + colliderBounds);

        float colliderRadius = coll.size.x * 0.4f * Mathf.Abs(transform.localScale.x);
        // Debug.Log("colliderRadius = " + colliderRadius);

        Vector3 groundCheckPos = colliderBounds.min + new Vector3(colliderBounds.size.x * 0.5f, colliderRadius * 0.9f, 0);
        // Debug.Log("groundCheckPos = " + groundCheckPos);



        // Check if player is grounded

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPos, colliderRadius);



        // Check if any of the overlapping colliders are not player collider, if so, set isGrounded to true

        this.isGrounded = false;
        if (colliders.Length > 0)
        {
            // Debug.Log("colliders.Length = " + colliders.Length);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != coll)
                {
                    this.isGrounded = true;
                    break;
                }
                // Debug.Log("colliders[i] != coll = " + colliders.Length);
            }
        }
    }


    //
    // CONTROLS LEFT + RIGHT MOVEMENT

    private void HandleHorizontalMovement()
    {
        rb.velocity = new Vector2(moveDirection.x * runSpeed, rb.velocity.y);
    }


    //
    // JUMP CONTROLS

    private void HandleJumping()
    {
        if (isGrounded && jumpPressed)
        {
            isGrounded = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
    }


    //
    // FACING DIRECTION

    private void UpdateFacingDirection()
    {
        // set facing direction
        if (moveDirection.x > 0.1f)
        {
            facingRight = true;
        }
        else if (moveDirection.x < -0.1f)
        {
            facingRight = false;
        }

        // rotate according to direction
        // we do this instead of using the 'flipX' spriteRenderer option because our player is made up of multiple sprites
        if (facingRight)
        {
            this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, 0, this.transform.eulerAngles.z);
        }
        else
        {
            this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, 180, this.transform.eulerAngles.z);
        }
    }


    //
    // ANIMATION

    private void UpdateAnimator()
    {
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("movementX", rb.velocity.x);
        animator.SetFloat("movementY", rb.velocity.y);
    }


    //
    // RESET PLAYER AFTER DEATH

    private IEnumerator HandleDeath() 
    {
        //
        // freeze player movemet
        rb.gravityScale = 0;
        disableMovement = true;
        rb.velocity = Vector3.zero;

        //
        // prevent other collisions
        coll.enabled = false;

        //
        // hide the player visual
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
        deathParticles.Play();

        // send off event that we died for other components in our system to pick up
        GameEventsManager.instance.PlayerDeath();

        yield return new WaitForSeconds(0.4f);
        
        Respawn();
    }

    private void Respawn() 
    {
        //
        // enable movement
        rb.gravityScale = gravityScale;
        coll.enabled = true;
        disableMovement = false;

        //
        // show player visual
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
        deathParticles.Stop();

        //
        // move the player to the respawn point
        this.transform.position = respawnPoint.position;
    }


    //
    // CAUSE DAMAGE

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        // if we collided with anything in the harmful layer, death occurs
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Harmful")))
        {
            StartCoroutine(HandleDeath());
        }
    }

}
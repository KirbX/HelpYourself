using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject
{

    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;
    
    private SpriteRenderer spriteRenderer;

    private Animator animator;
    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = custom_gravity * jumpTakeOffSpeed;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (velocity.y > 0)
            {
                velocity.y = custom_gravity * velocity.y * 0.5f;
            }
        }

        bool flipSprite = (spriteRenderer.flipX ? (move.x > 0.001f) : move.x < 0.001f);
        if (flipSprite)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        animator.SetBool("grounded", grounded);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
        targetVelocity = move * maxSpeed;
    }

    internal void setVelocity(Vector2 direction, float force)
    {
        Vector2 move = Vector2.zero;
        velocity.y = direction.y * force;
        targetVelocity = custom_gravity * direction * force;
    }

    internal void changegravity()
    {
        GetComponent<Rigidbody2D>().gravityScale *= -1;
        transform.RotateAround(GetComponent<Renderer>().bounds.center, new Vector3(0, 0, 1), 180);
        is_inverted = !is_inverted;
        custom_gravity = is_inverted ? -1 : 1;

    }

}

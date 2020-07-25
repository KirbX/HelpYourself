using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{

    public bool is_inverted = false;
    public int custom_gravity = 1;
    public static float gravity_modifier = 1;
    public float minGroundNormalY = .65f;
    public bool externalForce = false;
    protected Vector2 targetVelocity;
    protected bool grounded;
    protected Rigidbody2D rb2d;
    protected Vector2 velocity;
    protected Vector2 groundNormal;
    protected const float minMoveDistance = 0.001f;

    protected ContactFilter2D contactFilter;

    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];

    protected const float shellRadius = 0.01f;
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);


    void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!externalForce)
        {
            targetVelocity = Vector2.zero;
            ComputeVelocity();
        }
    }


    protected virtual void ComputeVelocity()
    {

    }

    void FixedUpdate()
    {
        velocity += custom_gravity * gravity_modifier * Physics2D.gravity * Time.deltaTime;

        velocity.x = targetVelocity.x;
        grounded = false;

        Vector2 deltaPosition = velocity * Time.deltaTime;

        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);
        Vector2 move;

        move = moveAlongGround * deltaPosition.x;
        Movement(move, false);

        move = Vector2.up * deltaPosition.y;
        Movement(move, true);
    }

    void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if (distance > minMoveDistance)
        {
            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();

            if (count > 0 && externalForce)
                externalForce = false;
            for (int i = 0; i < count; i++)
            {
                hitBufferList.Add(hitBuffer[i]);
            }

            for (int i = 0; i < hitBufferList.Count; i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal;
                if (custom_gravity * currentNormal.y > minGroundNormalY)
                {
                    externalForce = false;
                    grounded = true;
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal);
                if (projection < 0)
                {
                    velocity = velocity - projection * currentNormal;
                }

                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }


        }
        rb2d.position = rb2d.position + move.normalized * distance;
    }

    public bool inverted()
    {
        return is_inverted;
    }
}

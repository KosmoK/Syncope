using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GravityBallComponent : MonoBehaviour
{
    private Transform gravitySource;
    private float t = 0;
    private float duration;
    private Rigidbody2D rb;
    private float g;
    // private float mass;
    public ContactFilter2D movementFilter;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // mass = rb.mass;

        GetComponent<NavMeshAgent>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (t > duration)
        {
            GetComponent<NavMeshAgent>().enabled = true;
            Destroy(this);
        }

        Vector3 newPos = Vector3.Lerp(transform.position, gravitySource.position, g * Time.deltaTime); // can also add 1/mass for it to depend on mass
        if (canMove(newPos-transform.position))
        {
            rb.MovePosition(newPos);
        }
    }

    void FixedUpdate()
    {
        
    }

    /*void FixedUpdate()
    {
        float newX = Mathf.Sqrt(Mathf.Pow(velocity.x, 2) + 2*mass*g*)
        Vector2 movement;
        float mag = movement.magnitude;
        // Normalize the movement to stop the bug where moving diagonally would make you faster
        movement.Normalize();

        // Linear Interpolation for slippy
        velocity = Vector2.Lerp(velocity, movement * mag * Time.fixedDeltaTime, Time.fixedDeltaTime);

        // Check Collision
        bool success = canMove(velocity);

        if (success) {rb.MovePosition(rb.position + velocity);} // Using rigidBody.movePosition, move the player through reading the vector value
        else
        {
            // Try the other directions
            success = canMove(new Vector2(velocity.x, 0f));
            if (success){rb.MovePosition(rb.position + new Vector2(velocity.x, 0f));}
            else
            {
                success = canMove(new Vector2(0f, velocity.y));
                if(success){rb.MovePosition(rb.position + new Vector2(0f, velocity.y));}
            }
        }
    }*/

    public void setGravitySource(Transform t)
    {
        gravitySource = t;
    }

    public void setDuration(float d)
    {
        duration = d;
    }

    public void setGravity(float gravity)
    {
        g = gravity;
    }

    private bool canMove(Vector2 direction)
    { // Thank you goat : https://www.youtube.com/watch?v=05eWA0TP3AA
    // Checks if you are allowed to move. Shrimple as that.
        int count = rb.Cast(
            direction,
            movementFilter,
            castCollisions,
            direction.magnitude //topSpeed * Time.fixedDeltaTime + collisionOffset
            );
        
        if(count == 0) {return true;} // If you aren't colliding with anything, you're allowed to move.
        else{return false;}
    }
}

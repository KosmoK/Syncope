using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // CharacterController
    private Rigidbody2D rigidBody; // Requires rigidbody2d in the same object as script

    // Movement Mechanics
    public Vector2 moveDirection; // The direction the entity is INTENDING to move
    public float coefficentOfFriction = 7.5f; // Able to be edited anytime
    public float topSpeed = 5f; // 
    public Vector2 velocity = Vector2.zero; // The direction the entity is ACTUALY moving

    // Collision Mechanics
    public ContactFilter2D movementFilter;
    public float collisionOffset = 0.08f;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();


    // BEGONE!!! 
    // public float accelerationPercent = 0.25f; // 0f to 1f, dictates how quickly the object accelerates
    // public float mass = 1f; // Controls the mass of the character, which affects friction
    // public float gravity = 1f; // Controls the gravity of the character, which affects friction

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Read the move input, and convert it to proper values.
        Vector2 movement = new(moveDirection.x, moveDirection.y);

        // Normalize the movement to stop the bug where moving diagonally would make you faster
        movement.Normalize();

        // higher friction, slower movement
        movement*= 1/coefficentOfFriction*7.5f;

        // Linear Interpolation for slippy
        velocity = Vector2.Lerp(velocity, movement * topSpeed * Time.fixedDeltaTime, coefficentOfFriction * Time.fixedDeltaTime);

        // Check Collision
        bool success = canMove(velocity);

        if (success) {rigidBody.MovePosition(rigidBody.position + velocity);} // Using rigidBody.movePosition, move the player through reading the vector value
        else
        {
            // Try the other directions
            success = canMove(new Vector2(velocity.x, 0f));
            if (success){rigidBody.MovePosition(rigidBody.position + new Vector2(velocity.x, 0f));}
            else
            {
                success = canMove(new Vector2(0f, velocity.y));
                if(success){rigidBody.MovePosition(rigidBody.position + new Vector2(0f, velocity.y));}
            }
        }
    }

    public bool canMove(Vector2 direction)
    { // Thank you goat : https://www.youtube.com/watch?v=05eWA0TP3AA
    // Checks if you are allowed to move. Shrimple as that.
        int count = rigidBody.Cast(
            direction,
            movementFilter,
            castCollisions,
            velocity.magnitude + collisionOffset //topSpeed * Time.fixedDeltaTime + collisionOffset
            );
        
        if(count == 0) {return true;} // If you aren't colliding with anything, you're allowed to move.
        else{return false;}
    }

    /* Detect a collion
    *  If theres a collision
    *     Dont actualy apply the move
    */
}

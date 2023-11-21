using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KangarooController : MonoBehaviour
{
    public float kSpeed = 1f;

    private bool kCanMove = true;

    Vector2 kMovement;

    GameObject kPlayer;
    Rigidbody2D kPlayerRigidBody = new Rigidbody2D();
    Vector2 kPlayerLocation;

    Rigidbody2D kRigidBody;
    Collider2D kCollider;
    Collider2D[] kColliders;

    List<RaycastHit2D> kCastCollisions = new List<RaycastHit2D>();

    Animator kAnimator;

    SpriteRenderer kSpriteRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        kRigidBody = GetComponent<Rigidbody2D>();
        kAnimator = GetComponent<Animator>();
        kSpriteRenderer = GetComponent<SpriteRenderer>();


        Vector2 pos = new Vector2(0,0);

        kRigidBody.MovePosition(pos);
        
        kPlayer = GameObject.FindWithTag("Player");
        kPlayerRigidBody = kPlayer.GetComponent<Rigidbody2D>();
        kPlayerLocation = kPlayerRigidBody.position;
        
    }

    /// <summary>
    /// Update function that is independent of the frame-rate
    /// </summary>
    private void FixedUpdate() {
        kPlayerLocation = kPlayerRigidBody.position;


        kMovement = kPlayerLocation - kRigidBody.position;
        var distance = kMovement.magnitude;
        var direction = kMovement / distance; // This is now the normalized direction.

        // When player attempts to move, checks if player CAN move and attempts to move player
        bool canMove = TryMove(direction);

        if (!canMove) {
            print(direction);
            canMove = TryMove(direction.x, 0);
            if (!canMove) {
                canMove = TryMove(0, direction.y);
            }
        }
        kAnimator.SetBool("isMoving", canMove);
        // Flips player render, based on the direction player is facing
        if (direction.x < 0) {
            transform.localScale = new Vector3(1f,1f,1f);
        } else if (direction.x > 0) {
            transform.localScale = new Vector3(-1f,1f,1f);
        }
    }


    /// <summary>
    /// Overloaded version of TryMove function, in order to accept different format parameters
    /// </summary>
    /// <param name="x"> X-Value of player's new location </param>
    /// <param name="y"> Y-Value of player's new location </param>
    /// <returns> True if player can move to the new location </returns>
    private bool TryMove(float x, float y) {
        Vector2 pos = new Vector2(x, y);
        return TryMove(pos);
    }
    private bool TryMove(Vector2 direction) {
        if (kCanMove) {
            int count = kRigidBody.Cast(
                direction,
                kCastCollisions,
                kSpeed * Time.fixedDeltaTime);
            
            // Nothing was hit by the player's raycast
            if (count == 0) {
                kRigidBody.MovePosition(kRigidBody.position + 
                    direction * kSpeed * Time.fixedDeltaTime);
                return true;
            }
        }
        return false;
    }

    // CHECK IF STUCK ON WALL/OBSTACLE (LAST LOCATION = CURRENT LOCATION, THEN GAIN MS OR SOMETHING, IDK FIGURE OUT HOW TO WALK AROUND IT);
}

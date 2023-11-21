using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Player Controller class
/// 
/// Used to control the player object
/// </summary>
public class PlayerController : MonoBehaviour
{

    // Represents the player's speed
    public float pSpeed = 1f;
    // Represents whether the player can move at this moment
    private bool pCanMove = true;
    // User input of where to move player
    Vector2 pMovementIn;
    // Player's physical body in the game
    Rigidbody2D pRigidBody;
    // List of objects hit by the player's raycast
    List<RaycastHit2D> pCastCollisions = new List<RaycastHit2D>();
    // Player's animator component, used for animating the character in the game
    Animator pAnimator;
    // Renderer, used for rendering the player's sprite in the game
    SpriteRenderer pSpriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
     pRigidBody = GetComponent<Rigidbody2D>();
     pAnimator = GetComponent<Animator>();
     pSpriteRenderer = GetComponent<SpriteRenderer>();
     

    Vector2 pos = new Vector2(7,4);

     pRigidBody.MovePosition(pos);
    }

    /// <summary>
    /// Update function that is independent of the frame-rate
    /// </summary>
    private void FixedUpdate() {

        // When player attempts to move, checks if player CAN move and attempts to move player
        if (pMovementIn != Vector2.zero) {
            bool canMove = TryMove(pMovementIn);

            if (!canMove) {
                canMove = TryMove(pMovementIn.x, 0);
                if (!canMove) {
                    canMove = TryMove(0, pMovementIn.y);
                }
            }
            pAnimator.SetBool("isMoving", canMove);
        } else {
            pAnimator.SetBool("isMoving", false);
        }

        // Flips player render, based on the direction player is facing
        if (pMovementIn.x > 0) {
            pSpriteRenderer.flipX = true;
        } else if (pMovementIn.x < 0) {
            pSpriteRenderer.flipX = false;
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

    /// <summary>
    /// Checks if player can move to new location, moves the player if possible
    /// </summary>
    /// <param name="direction"> Direction that player is attempting to move </param>
    /// <returns> True if player can move to the new location </returns>
    private bool TryMove(Vector2 direction) {
        if (pCanMove) {
            int count = pRigidBody.Cast(
                direction,
                pCastCollisions,
                pSpeed * Time.fixedDeltaTime);
            
            // Nothing was hit by the player's raycast
            if (count == 0) {
                pRigidBody.MovePosition(pRigidBody.position + 
                    direction * pSpeed * Time.fixedDeltaTime);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// When player moves in-game, this function is called
    /// </summary>
    /// <param name="movement"> Movement information of the player </param>
    void OnMove(UnityEngine.InputSystem.InputValue movement) {
        pMovementIn = movement.Get<Vector2>();
    }

}

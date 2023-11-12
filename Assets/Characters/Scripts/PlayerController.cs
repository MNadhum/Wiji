using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float PlayerSpeed = 1f;
    public float CollisionOffset = 0.05f;
    private bool PlayerCanMove = true;
    public ContactFilter2D PlayerMovementFilter;

    Vector2 PlayerMovementIn;
    Rigidbody2D PlayerRigidBody;
    List<RaycastHit2D> CastCollisions = new List<RaycastHit2D>();

    Animator PlayerAnimator;
    SpriteRenderer PlayerSpriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
     PlayerRigidBody = GetComponent<Rigidbody2D>();
     PlayerAnimator = GetComponent<Animator>();
     PlayerSpriteRenderer = GetComponent<SpriteRenderer>();
     

    Vector2 pos = new Vector2(7,4);

     PlayerRigidBody.MovePosition(pos);
    }

    private void FixedUpdate() {
        if (PlayerMovementIn != Vector2.zero) {
            bool canMove = TryMove(PlayerMovementIn);

            if (!canMove) {
                canMove = TryMove(PlayerMovementIn.x, 0);
                if (!canMove) {
                    canMove = TryMove(0, PlayerMovementIn.y);
                }
            }
            PlayerAnimator.SetBool("isMoving", canMove);
        } else {
            PlayerAnimator.SetBool("isMoving", false);
        }

        if (PlayerMovementIn.x > 0) {
            PlayerSpriteRenderer.flipX = true;
        } else if (PlayerMovementIn.x < 0) {
            PlayerSpriteRenderer.flipX = false;
        }
    }
    private bool TryMove(float x, float y) {
        Vector2 pos = new Vector2(x, y);
        return TryMove(pos);
    }
    private bool TryMove(Vector2 direction) {
        if (PlayerCanMove) {
            int count = PlayerRigidBody.Cast(
                direction,
                PlayerMovementFilter,
                CastCollisions,
                PlayerSpeed * Time.fixedDeltaTime + CollisionOffset);
            
            if (count == 0) {
                PlayerRigidBody.MovePosition(PlayerRigidBody.position + 
                    direction * PlayerSpeed * Time.fixedDeltaTime);
                return true;
            }
        }
        return false;
    }

    void OnMove(UnityEngine.InputSystem.InputValue movement) {
        PlayerMovementIn = movement.Get<Vector2>();
    }

}

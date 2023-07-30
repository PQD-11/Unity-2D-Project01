using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpSeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(10f, 10f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    Vector2 moveInput;
    Rigidbody2D myRigibody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    float gravityScaleDefault;

    bool isAlive = true;

    void Start()
    {
        myRigibody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleDefault = myRigibody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) {return;}
        Run();
        FlipSripte();
        ClimbLadder();
        Die();
    }

    void OnFire(InputValue value){
        if (!isAlive) {return;}
        Instantiate(bullet, gun.position, transform.rotation);
    }
    
    void OnMove(InputValue value){
        if (!isAlive) {return;}
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value){
        if (!isAlive) {return;}

        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) return;

        if (value.isPressed){
            myRigibody.velocity = new Vector2(0f, jumpSeed);
        }
    }

    void Run(){
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigibody.velocity.y);
        myRigibody.velocity = playerVelocity;
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigibody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    void FlipSripte(){
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigibody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed){
            transform.localScale = new Vector2(Mathf.Sign(myRigibody.velocity.x), 1f);
        }
    }
    void ClimbLadder(){
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) 
        {
            myRigibody.gravityScale = gravityScaleDefault;
            myAnimator.SetBool("isClimbing", false);
            return;
        }

        Vector2 climbVelocity = new Vector2(myRigibody.velocity.x, moveInput.y * climbSpeed);
        myRigibody.velocity = climbVelocity;
        myRigibody.gravityScale = 0f;
        bool playerHasVerticalSpeed = Mathf.Abs(myRigibody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
    }
    void Die(){
        if (myRigibody.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards", "Water"))){
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigibody.velocity = deathKick;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}

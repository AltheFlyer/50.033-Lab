using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10;
    public float maxSpeed = 20;
    public float upSpeed = 20;
    private Rigidbody2D marioBody;
    private bool onGroundState = true;

    private SpriteRenderer marioSprite;
    private bool faceRightState = true;


    // Custom stuff for resets
    private Vector3 initPlayerLocation;


    // for animation
    public Animator marioAnimator;

    // for audio
    public AudioSource marioAudio;


    public AudioClip marioDeath;

    // state
    [System.NonSerialized]
    public bool alive = true;

    float deathImpulse = 10.0f;


    public Transform gameCamera;
    private Vector3 cameraStart;

    private bool moving = false;
    private bool jumpedState = false;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();

        marioSprite = GetComponent<SpriteRenderer>();

        initPlayerLocation = transform.position;

        // update animator state
        marioAnimator.SetBool("onGround", onGroundState);

        cameraStart = gameCamera.position;
    }

    // Update is called once per frame
    void Update()
    {
        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.velocity.x));
    }

    void FlipMarioSprite(int value)
    {
        if (value == -1 && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
            if (marioBody.velocity.x > 0.05f)
                marioAnimator.SetTrigger("onSkid");

        }

        else if (value == 1 && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
            if (marioBody.velocity.x < -0.05f)
                marioAnimator.SetTrigger("onSkid");
        }
    }

    void FixedUpdate()
    {
        if (alive && moving)
        {
            Move(faceRightState == true ? 1 : -1);
        }
    }

    void Move(int value)
    {

        Vector2 movement = new Vector2(value, 0);
        // check if it doesn't go beyond maxSpeed
        if (marioBody.velocity.magnitude < maxSpeed)
            marioBody.AddForce(movement * speed);
    }

    public void MoveCheck(int value)
    {
        if (value == 0)
        {
            moving = false;
        }
        else
        {
            FlipMarioSprite(value);
            moving = true;
            Move(value);
        }
    }

    public void Jump()
    {
        if (alive && onGroundState)
        {
            // jump
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
            jumpedState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);

        }
    }


    public void JumpHold()
    {
        if (alive && jumpedState)
        {
            // jump higher
            marioBody.AddForce(Vector2.up * upSpeed * 30, ForceMode2D.Force);
            jumpedState = false;

        }
    }


    void OnCollisionEnter2D(Collision2D col)
    {
        int collisionLayerMask = (1 << 3) | (1 << 6) | (1 << 7);

        if (((collisionLayerMask & (1 << col.transform.gameObject.layer)) > 0) && !onGroundState)
        {
            onGroundState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && alive)
        {
            // play death animation
            marioAnimator.Play("mario-die");
            marioAudio.PlayOneShot(marioDeath);
            alive = false;
        }
    }

    public void GameRestart()
    {
        // reset position
        marioBody.transform.position = initPlayerLocation;
        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;

        // reset animation
        marioAnimator.SetTrigger("gameRestart");
        alive = true;

        // reset camera position
        gameCamera.position = cameraStart;
    }

    void PlayJumpSound()
    {
        // play jump sound
        marioAudio.PlayOneShot(marioAudio.clip);
    }



    void PlayDeathImpulse()
    {
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }

    void PromptGameOver()
    {
        GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().GameOver();
    }
}

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

    public TextMeshProUGUI scoreText;
    public GameObject enemies;

    public JumpOverGoomba jumpOverGoomba;


    public GameObject gameOverScreen;
    public Button restartButton;


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
        if (Input.GetKeyDown("a") && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
            if (marioBody.velocity.x > 0.1f)
                marioAnimator.SetTrigger("onSkid");
        }

        if (Input.GetKeyDown("d") && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
            if (marioBody.velocity.x < -0.1f)
                marioAnimator.SetTrigger("onSkid");
        }

        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.velocity.x));
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

    void FixedUpdate()
    {
        if (alive)
        {
            float moveHorizontal = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(moveHorizontal) > 0)
            {
                Vector2 movement = new Vector2(moveHorizontal, 0);
                if (marioBody.velocity.magnitude < maxSpeed)
                {
                    marioBody.AddForce(movement * speed);
                }
            }
            if (marioBody.velocity.magnitude > maxSpeed)
            {
                marioBody.velocity = marioBody.velocity.normalized * maxSpeed;
            }

            if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
            {
                marioBody.velocity = Vector2.zero;
            }

            if (Input.GetKeyDown("space") && onGroundState)
            {
                marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
                onGroundState = false;
                // update animator state
                marioAnimator.SetBool("onGround", onGroundState);
            }
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

    public void GameOverScene()
    {
        Time.timeScale = 0.0f;

        // Show game over screen;
        gameOverScreen.SetActive(true);
        // Move buttons and text
        scoreText.GetComponent<RectTransform>().anchoredPosition = new Vector3(52, 70, 0);
        restartButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
    }

    public void RestartButtonCallback(int input)
    {
        ResetGame();
        Time.timeScale = 1.0f;
    }

    private void ResetGame()
    {
        marioBody.transform.position = initPlayerLocation;
        // Keep mario from sliding initially
        marioBody.velocity = Vector2.zero;
        faceRightState = true;
        marioSprite.flipX = false;
        scoreText.text = "Score: 0";
        foreach (Transform child in enemies.transform)
        {
            child.transform.localPosition = child.GetComponent<EnemyMovement>().startPosition;
        }
        jumpOverGoomba.score = 0;

        // Custom stuff for lab submission

        // Hide game over screen
        gameOverScreen.SetActive(false);
        // Move important stuff to its original position
        scoreText.GetComponent<RectTransform>().anchoredPosition = new Vector3(-700, 450, 0);
        restartButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(850, 450, 0);


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
}

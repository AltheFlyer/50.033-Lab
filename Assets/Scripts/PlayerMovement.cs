using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10;
    public float maxSpeed = 20;
    public float upSpeed = 10;
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

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();

        marioSprite = GetComponent<SpriteRenderer>();

        initPlayerLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("a") && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
        }
        if (Input.GetKeyDown("d") && faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground")) onGroundState = true;
    }

    void FixedUpdate()
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

        if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
        {
            marioBody.velocity = Vector2.zero;
        }

        if (Input.GetKeyDown("space") && onGroundState)
        {
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collided with goomba!");
            Time.timeScale = 0.0f;

            // Show game over screen;
            gameOverScreen.SetActive(true);
            // Move buttons and text
            scoreText.GetComponent<RectTransform>().anchoredPosition = new Vector3(52, 70, 0);
            restartButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        }
    }

    public void RestartButtonCallback(int input)
    {
        Debug.Log("Restart!");
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
    }
}

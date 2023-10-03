using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class EnemyMovement : MonoBehaviour
{

    private float originalX;
    private float maxOffset = 5.0f;
    private float enemyPatroltime = 2.0f;
    private int moveRight = -1;
    private Vector2 velocity;

    private Rigidbody2D enemyBody;

    public Vector3 startPosition;

    private bool isAlive;

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.localPosition;
        enemyBody = GetComponent<Rigidbody2D>();
        originalX = transform.position.x;
        ComputeVelocity();
        isAlive = true;
    }

    void ComputeVelocity()
    {
        velocity = new Vector2((moveRight) * maxOffset / enemyPatroltime, 0);
    }

    void Movegoomba()
    {
        enemyBody.MovePosition(enemyBody.position + velocity * Time.fixedDeltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive)
        {
            return;
        }

        if (Mathf.Abs(enemyBody.position.x - originalX) < maxOffset)
        {
            Movegoomba();
        }
        else
        {
            moveRight *= -1;
            ComputeVelocity();
            Movegoomba();
        }
    }

    // (Used by EnemyManager)
    public void GameRestart()
    {
        transform.localPosition = startPosition;
        originalX = transform.position.x;
        moveRight = -1;
        ComputeVelocity();
        isAlive = true;
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
        animator.SetTrigger("respawnTrigger");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAlive)
        {
            return;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (!player)
            {
                return;
            }
            if (!player.alive)
            {
                return;
            }

            if (other.attachedRigidbody.velocity.y < 0 && other.transform.position.y - transform.position.y > 0.4f)
            {
                animator.SetTrigger("deathTrigger");
                player.Bounce();
                isAlive = false;
                GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().IncreaseScore(1);
            }
            else
            {
                player.Die();
            }
        }
    }

    void Despawn()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }
}

using System;
using UnityEngine;

public class BrickBlock : MonoBehaviour
{
    public bool containsCoin;

    public Transform blockChild;
    public Transform coinChild;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if (!animator)
        {
            throw new Exception("Coin Question Block does not contain an animator!");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // Determine collision direction by finding 
        // where the thing that hit the block (mario) is
        Vector3 other = col.collider.transform.position;

        if (transform.position.y - other.y > 0.25f && col.rigidbody.velocity.y >= 0)
        {
            if (containsCoin)
            {
                animator.SetTrigger("coinHitTrigger");
                containsCoin = false;
            }
            else
            {
                animator.SetTrigger("hitTrigger");
            }
        }
    }
}

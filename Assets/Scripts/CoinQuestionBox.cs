using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinQuestionBox : MonoBehaviour
{
    public enum BlockState
    {
        blinking,
        bouncing,
        disabled
    }

    // State variables
    public BlockState blockState = BlockState.blinking;

    [NonSerialized]
    public Transform blockChild;

    [NonSerialized]
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

    // Whether or not the block contains a coin (and will animate when hit from below)
    private bool containsCoin()
    {
        return blockState == BlockState.blinking;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!containsCoin())
        {
            return;
        }

        // Determine collision direction by finding 
        // where the thing that hit the block (mario) is
        Vector3 other = col.collider.transform.position;

        if (transform.position.y - other.y > 0.25f)
        {
            blockState = BlockState.bouncing;
        }
    }

    // Sets block state to disabled form.
    // Shoudl be invoked by an Animation Event.
    void DisableSelf()
    {
        blockState = BlockState.disabled;
    }
}

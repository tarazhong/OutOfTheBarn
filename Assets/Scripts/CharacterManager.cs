﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [HideInInspector]
    public GameObject player;
    public bool carryButtonDown;
    public bool wistleInterraction;
    public bool blockaction;

    [HideInInspector]
    public GameObject objectTocarry;
    public Transform objectAnchor;

    private Rigidbody2D rigidBody;
    
    public float horizontalMove = 0f;
    [HideInInspector]
    public bool rightDirection;
    [SerializeField]
    private SpriteRenderer mysprite1;
    private Animator myAnimator1;
    [SerializeField]
    private SpriteRenderer mysprite2;

    [SerializeField]
    private float speed;
    public string collisionName;
    public bool wistleCollisionPlayerInCheck;

    [HideInInspector]
    public static CharacterManager instance = null;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        myAnimator1 = GetComponent<Animator>();
        rightDirection = true;
        player = this.gameObject;
        blockaction = false;
    }

    // Update is called once per frame => not good because besed on computer fps
    void Update()
    {
        if (!blockaction)
        {
            // Check Carry/Drop
            if (!wistleInterraction && !carryButtonDown && Input.GetKeyDown(KeyCode.V))
            {
                carryButtonDown = true;
            }
            else
            {
                carryButtonDown = false;
            }

            if (objectTocarry != null && carryButtonDown)
            {
                carryButtonDown = false;
                DropObject();
            }

            // Check Wistle
            if (objectTocarry == null &&!wistleInterraction && !carryButtonDown && Input.GetKeyDown(KeyCode.C))
            {
                wistleInterraction = true;
                myAnimator1.SetTrigger("isWistling");

                if (!TriggerEvent.instance.goodAlert)
                {
                    TriggerEvent.instance.DisplayFalseAlertText();
                }
            }
            else
            {
                wistleInterraction = false;
            }
        }
        Flip();
    }

    // Dedicated for physics, Called a fixed amount (fixed amount of time per second) 
    public void FixedUpdate()
    {
        if (CameraManager.instance.cameraTransition && !CameraManager.instance.walkAnimation)
        {
            return;
        }
        if (!CameraManager.instance.cameraTransition && !CameraManager.instance.walkAnimation && !blockaction) // cant move player when a cinematic is playing
        {
            horizontalMove = Input.GetAxis("Horizontal");
            if (horizontalMove > -0.1f && horizontalMove < 0.1f)
            {
                myAnimator1.SetBool("Walk", false);
            }
            else
            {
                myAnimator1.SetBool("Walk", true);
            }
            transform.Translate(Vector3.right * horizontalMove * speed * Time.deltaTime);
            //rigidBody.velocity = new Vector2(horizontalMove * speed, rigidBody.velocity.y); // x--
        }

    }

    public void Flip()
    {
        if (horizontalMove > 0 && !rightDirection || horizontalMove < 0 && rightDirection)
        {
            rightDirection = !rightDirection;
            mysprite1.flipX = !rightDirection; // flip if facing left
            mysprite2.flipX = !rightDirection; // flip if facing left

            float distanceToMove = 1.64f;
            if (!rightDirection)
                distanceToMove = -distanceToMove;
            objectAnchor.position += new Vector3(distanceToMove, 0f, 0f);

            if (objectTocarry != null)
            {
                SpriteRenderer spriteToFlip = objectTocarry.GetComponent<SpriteRenderer>();
                spriteToFlip.flipX = !rightDirection; // flip if facing left

                objectTocarry.transform.position = objectAnchor.position;
            }
        }
    }

    public void CarryObject(GameObject obj)
    {
        myAnimator1.SetTrigger("CarringTrigger");
        obj.GetComponent<ObjectToCarry>().ObjUp();
        blockaction = true;
        Invoke("Blockaction", 0.25f);

        objectTocarry = obj;
        SpriteRenderer spriteToFlip = objectTocarry.GetComponent<SpriteRenderer>();
        spriteToFlip.flipX = !rightDirection; // flip if facing left
        objectTocarry.transform.parent = this.player.transform;
    }

    private void DropObject()
    {
        carryButtonDown = false;
        myAnimator1.SetTrigger("CarringTrigger");
        objectTocarry.GetComponent<ObjectToCarry>().ObjFall();
        blockaction = true;
        Invoke("Blockaction", 1.5f);

        objectTocarry.transform.parent = null;
        objectTocarry = null;
    }

    public void StopWalkingAnim()
    {
        myAnimator1.SetBool("Walk", false);
    }

    private void Blockaction()
    {
        blockaction = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        collisionName = collision.name;
    }


    // check if in collider for wistle
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.name);
        //Debug.Log(wistleCollisionPlayerInCheck);
        if (collision.name== "Lvl1Collider1")
            wistleCollisionPlayerInCheck = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("Exit " + wistleCollisionPlayerInCheck);
        if (collision.name == "Lvl1Collider1")
            wistleCollisionPlayerInCheck = false;
    }
}

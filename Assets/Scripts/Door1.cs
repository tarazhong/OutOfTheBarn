﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door1 : MonoBehaviour {

    private Animator myAnimator;
    private BoxCollider2D doorCollider;
    public int id;
    public bool doorInterraction;
    public bool locked;
    private bool blockaction;

    // Use this for initialization
    void Start ()
    {
        myAnimator = GetComponent<Animator>();
        doorCollider = GetComponent<BoxCollider2D>();
        locked = true;
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.B))
        {
            doorInterraction = true;
        }
        else
        {
            doorInterraction = false;
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (!blockaction)
        {
            if (doorInterraction && locked)
            {

                blockaction = true;
                locked = false;
                myAnimator.SetBool("DoorOpen", true);
                doorCollider.isTrigger = true;
                Invoke("Blockaction", 0.25f); ;
            }
            if (locked)
            {
                Debug.Log("Locked");
            }
        }
    }



    private void OnTriggerStay2D(Collider2D other)
    {
        if (!blockaction)
        {
            if (doorInterraction && !locked)
            {
                doorCollider.isTrigger = false;
                Debug.Log("ReLocked");
                blockaction = true;
                locked = true;
                myAnimator.SetBool("DoorOpen", false);
                Invoke("Blockaction", 0.25f); ;
            }
        }
    }

    private void Blockaction()
    {
        blockaction = false;
    }
}

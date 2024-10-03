using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerRespawn : MonoBehaviour
{
    private GameObject lastCheckPoint;

    private PlayerStateList pState;
    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D playerCollider;
    private SpriteRenderer sr;
    private GameObject mainCam;
    private UnityEvent bonfireUsedEvent;

    void Start() 
    {
        pState = GetComponent<PlayerStateList>();

        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        mainCam = GetComponent<PlayerController>().mainCam;
        bonfireUsedEvent = GetComponent<BonfireBehaviour>().bonfireUsedEvent;

        if (lastCheckPoint == null)
            lastCheckPoint = GetComponent<BonfireBehaviour>().defaultBonfire;
    }
    
    // public void Respawn() 
    // {
    //     lastCheckPoint = GetComponent<BonfireBehaviour>().lastBonfireRestedAt;
    //     // move to checkpoint
    //     transform.position = lastCheckPoint.transform.position;

    //     // restore health
    //     GetComponent<PlayerController>().Health = GetComponent<PlayerController>().maxHealth;

    //     // reset anims
    //     anim.ResetTrigger("Death");

    //     // !!CHANGELATER: rest at bonfire animation, sound effect 
    //     anim.Play("Player_idle");

    //     // !!CHANGELATER: add that pstate.resting becomes true
    //     pState.dead = false;

    //     // !!CHANGELATER: add change of scenes with camera here or smth idk
    //     mainCam.GetComponent<Camera>().backgroundColor = new Color32(0, 0, 0, 255);

    //     // throw bonfire used event 
    //     GetComponent<BonfireBehaviour>().bonfireUsedEvent.Invoke();
    // }

    public void Respawn() 
    {
        lastCheckPoint = GetComponent<BonfireBehaviour>().lastBonfireRestedAt;
        Vector3 newPos = lastCheckPoint.transform.position;
        newPos.x += 1;
        newPos.z = transform.position.z;
        transform.position = newPos;

        pState.dead = false;
        anim.ResetTrigger("Death");
        GetComponent<BonfireBehaviour>().restAtBonfire(lastCheckPoint);

        mainCam.GetComponent<Camera>().backgroundColor = new Color32(0, 0, 0, 255);
    }
}

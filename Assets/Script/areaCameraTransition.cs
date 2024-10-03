using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class areaCameraTransition : MonoBehaviour
{
    // 1, 2, 3, 4, 5 are normal rooms, 6 is hallway to boss room, 7 is boss room 
    private List<PolygonCollider2D> level;
    [SerializeField] private PolygonCollider2D level1;
    [SerializeField] private PolygonCollider2D level2;
    [SerializeField] private PolygonCollider2D level3;
    [SerializeField] private PolygonCollider2D level4;
    [SerializeField] private PolygonCollider2D level5;
    [SerializeField] private PolygonCollider2D level6;
    [SerializeField] private PolygonCollider2D level7;

    [SerializeField] private GameObject cineCam;
    private CinemachineConfiner2D confiner;

    private PlayerStateList pState;

    private bool isInTransitionTrigger = false;
    private float prevX;
    private float prevY;


    void Start()
    {
        confiner = cineCam.GetComponent<CinemachineConfiner2D>();
        level = new List<PolygonCollider2D> {level1, level2, level3, level4, level5, level6, level7};
        pState = GetComponent<PlayerStateList>();

        switchToRoom(pState.currentLevel);
    }
    
    public void switchToRoom(int levelNum)
    {   
        pState.currentLevel = levelNum;
        confiner.m_BoundingShape2D = level[levelNum - 1];
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("TransitionTrigger"))
        {
            isInTransitionTrigger = true;

            // save positions when entering collider
            prevX = transform.position.x;
            prevY = transform.position.y;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("TransitionTrigger") && isInTransitionTrigger)
        {
            isInTransitionTrigger = false;
            
            // get positions 
            float newX = transform.position.x,
                  newY = transform.position.y,
                  triggerX = collision.gameObject.transform.position.x,
                  triggerY = collision.gameObject.transform.position.y;

            bool crossedTheTrigger = false;

            // we check if player has switched sides
            if (collision.gameObject.GetComponent<TransitionTriggerInformation>().isHorizontal) 
            {
                // check x for horizontal
                crossedTheTrigger = (prevX < triggerX && newX > triggerX) || (prevX > triggerX && newX < triggerX);
            }
            else if (collision.gameObject.GetComponent<TransitionTriggerInformation>().isVertical)
            {
                // check y for vertical
                crossedTheTrigger = (prevY < triggerY && newY > triggerY) || (prevY > triggerY && newY < triggerY);
            }

            if (crossedTheTrigger)
            {
                int levelA = collision.gameObject.GetComponent<TransitionTriggerInformation>().levelA,
                    levelB = collision.gameObject.GetComponent<TransitionTriggerInformation>().levelB;

                int nextLevel = (levelA != pState.currentLevel) ? levelA : levelB;
                switchToRoom(nextLevel);
            }
        }
    }

    /*
    !!CHANGELATER: add camera teleport to new scene
    
    on collision enter, i am going to set a variable called isInTransitionTrigger to true
    im going to set a float prevX = player.transform.x
    im also going to set float prevY = player.transform.y 
    
    on collision exit, i am first going to set isInTransitionTrigger to false
    next, i am going to check whether player has actually crossed the trigger
        for horizontal transitiontrigger i am going to check if (prevX < transitiontrigger.x && newX > transitiontrigger.x) 
        or (prevX > transitiontrigger.x && newX < transitiontrigger.x)
        for vertical transitiontrigger i am going to check if (prevX < transitiontrigger.x && newX > transitiontrigger.x) 
        or (prevX > transitiontrigger.x && newX < transitiontrigger.x)

    if the player has crossed the trigger i am going to switch rooms
    else do nothing

    compare level bounds
    */
}

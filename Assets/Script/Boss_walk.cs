using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_walk : StateMachineBehaviour
{
    private BossMain bossComponent;
    private Transform player;
    private Rigidbody2D rb;

    private bool canDoAttack1 = false;
    private bool canDoAttack2 = false; 

    private float bossSpeed;

    private float beenWalkingAroundFor = 0;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossComponent = animator.GetComponent<BossMain>();
        player = bossComponent.player.transform;
        rb = animator.GetComponent<Rigidbody2D>();
        bossSpeed = bossComponent.bossSpeed;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!bossComponent.playerFound)
        {
            animator.SetBool("PlayerFound", false);
            return;
        }
        bossComponent.LookAtPlayer();
        Vector2 target = new Vector2(player.position.x, rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, bossSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        beenWalkingAroundFor += Time.deltaTime;

        canDoAttack1 = (beenWalkingAroundFor <= 5f ? bossComponent.canDoAttack1InRange : bossComponent.canDoAttack1InHitzone);
        canDoAttack2 = (beenWalkingAroundFor <= 5f ? bossComponent.canDoAttack2InRange : bossComponent.canDoAttack2InHitzone);
        
        if (beenWalkingAroundFor > 5f && (canDoAttack1 || canDoAttack2))
        {
            beenWalkingAroundFor = 0;
        }

        if (canDoAttack1 && canDoAttack2) 
        {
            int a = Convert.ToInt32(Mathf.Round(UnityEngine.Random.Range(0, 2)));
            if (a % 2 == 0)
            {
                animator.SetTrigger("Attack2");
                canDoAttack1 = false;
            }
            else {
                animator.SetTrigger("Attack1");
                canDoAttack2 = false;
            }
        } 
        else if (canDoAttack1) 
        {
            animator.SetTrigger("Attack1");
        }
        else if (canDoAttack2)
        {
            animator.SetTrigger("Attack2");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (canDoAttack1)
            animator.ResetTrigger("Attack1");
        if (canDoAttack2)
            animator.ResetTrigger("Attack2");
    }
}

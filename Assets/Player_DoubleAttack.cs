using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_DoubleAttack : StateMachineBehaviour
{
    private bool didDoubleAttack = false;
    private PlayerController player;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.GetComponent<PlayerController>();
        didDoubleAttack = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!Input.GetKey(KeyCode.W) && player.attack && !didDoubleAttack)
        {
            Debug.Log("hewo");
            didDoubleAttack = true;
            animator.SetTrigger("DoubleAttack");

            player.rb.velocity = Vector2.zero;
            
            player.isAttacking = true;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        didDoubleAttack = false;
    }
}

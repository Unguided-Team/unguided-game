using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skelly : Enemy
{
   
    [SerializeField] private float chaseDistance = 5f;
    [SerializeField] private float stunDuration = 2f;
    [SerializeField] private float attackCooldown = 5f;

    private float timer;
    private float lastAttackTime;
    private int playerLayerMask;

    void Start()
    {
        base.Start();
        playerLayerMask = LayerMask.GetMask("Player"); // Ensure this matches your player layer
        ChangeState(EnemyStates.fly_Idle);
    }

    protected override void UpdateEnemyStates()
    {
        Vector2 playerPosition = PlayerController.Instance.transform.position;
        float distanceToPlayer = Vector2.Distance(transform.position, playerPosition);

        switch (currentEnemyState)
        {
            case EnemyStates.fly_Idle:
                if (distanceToPlayer < chaseDistance)
                {
                    ChangeState(EnemyStates.fly_Chase);
                }
                break;

            case EnemyStates.fly_Chase:
                rb.MovePosition(Vector2.MoveTowards(transform.position, playerPosition, Time.deltaTime * speed));
                Flipfly();
                
                // Check if within attack range and only attack if not in cooldown
                if (distanceToPlayer < 2f && Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack();
                }
                
                // Check if player is out of chase distance
                if (distanceToPlayer > chaseDistance)
                {
                    ChangeState(EnemyStates.fly_Idle);
                }
            break;

            case EnemyStates.fly_Stunned:
                timer += Time.deltaTime;
                if (timer > stunDuration)
                {
                    ChangeState(EnemyStates.fly_Idle);
                    timer = 0;
                }
                break;

            case EnemyStates.fly_Death:
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("fly_Death") &&
                    anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    Debug.Log("Enemy is dead. Triggering respawn.");
                    TriggerRespawn(); // Call respawn after animation
                }
                break;
        }
    }

    public void Die()
    {
        anim.SetBool("isDead", true); // Set death parameter
        ChangeState(EnemyStates.fly_Death);
    }

    public override void EnemyHit(float damageDone, Vector2 hitDirection, float hitForce)
{
    base.EnemyHit(damageDone, hitDirection, hitForce);
    
    // Instantiate blood particles
    Instantiate(bloodParticlePrefab, transform.position, Quaternion.identity);

    if (health > 0)
    {
        ChangeState(EnemyStates.fly_Stunned);
    }
    else
    {
        Die();
    }
}


    private void Flipfly()
    {
        float playerPositionX = PlayerController.Instance.transform.position.x;

        if (playerPositionX < transform.position.x)
        {
            sr.flipX = true; // Flip left
        }
        else
        {
            sr.flipX = false; // Flip right
        }
    }

    protected virtual void Attack()
{
    // Check if enough time has passed since the last attack
    if (Time.time >= lastAttackTime + attackCooldown)
    {
        anim.SetTrigger("AttackTrigger"); // Trigger attack animation
        lastAttackTime = Time.time; // Reset the last attack time
    }
}

    // This method is called via Animation Event
    public void PerformAttack()
    {
        Vector2 attackPosition = transform.position; // Center of the enemy
        float attackRadius = 2f; // Adjust as needed

        Collider2D playerCollider = Physics2D.OverlapCircle(attackPosition, attackRadius, playerLayerMask);
        
        if (playerCollider != null && !PlayerController.Instance.pState.invincible)
        {
            Debug.Log("fly enemy attacking player!");
            PlayerController.Instance.TakeDamage(damage);
        }
    }
    protected virtual void HandleDeath()
{
    // DisableComponents(); // Call the method to disable components
    base.HandleDeath(); // Call the base class method for additional handling
}

// private void DisableComponents()
// {
//     sr.enabled = false; // Disable the SpriteRenderer
//     GetComponent<BoxCollider2D>().enabled = false; // Disable the BoxCollider2D
// }


    private void OnDrawGizmos()
    {
        // Visualize the chase distance and attack radius in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 2f); // Attack radius
    }
}

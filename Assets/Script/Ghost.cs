using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Enemy
{
    [SerializeField] private float flipWaitTime = 1f;
    [SerializeField] private float ledgeCheckX = 0.5f;
    [SerializeField] private float ledgeCheckY = 1f;
    [SerializeField] private LayerMask whatIsGround;

    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        rb.gravityScale = 12f;
    }

    protected override void Update()
    {
        base.Update();
        if (!isRecoiling)
        {
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 targetPosition = new Vector2(PlayerController.Instance.transform.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    protected override void UpdateEnemyStates()
    {
        if(health <= 0)
        {
            Destroy(gameObject, 0.05f);
        }


        switch (currentEnemyState)
        {
            case EnemyStates.Ghost_Idle:
                HandleIdleState();
                break;

            case EnemyStates.Ghost_Flip:
                HandleFlipState();
                break;
        }
    }

    private void HandleIdleState()
    {
        Vector3 ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
        Vector2 wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;

        // Check for ledge and wall
        if (!Physics2D.Raycast(transform.position + ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround) ||
            Physics2D.Raycast(transform.position, wallCheckDir, ledgeCheckX, whatIsGround))
        {
            ChangeState(EnemyStates.Ghost_Flip);
        }

        // Set velocity based on direction
        rb.velocity = new Vector2(transform.localScale.x > 0 ? speed : -speed, rb.velocity.y);
    }

    private void HandleFlipState()
    {
        timer += Time.deltaTime; // Accumulate time
        if (timer > flipWaitTime)
        {
            timer = 0;
            // Flip the ghost's scale
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            ChangeState(EnemyStates.Ghost_Idle);
        }
    }
}

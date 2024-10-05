using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EnemyStates
{
    Ghost_Idle,
    Ghost_Flip,
    fly_Idle,
    fly_Chase,
    fly_Stunned,
    fly_Death,
    Skelly_Idle,
    Skelly_Chase,
    Skelly_Stunned,
    Skelly_Death,

}

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] private float startingHealth;
    
    [SerializeField] protected float recoilLength = 0.5f;
    [SerializeField] protected float recoilFactor = 1.0f;
    [SerializeField] protected bool isRecoiling = false;

    [SerializeField] protected float speed = 2f;
    [SerializeField] protected float damage = 10f;

    protected float recoilTimer = 0f;
    protected Rigidbody2D rb;
    protected EnemyStates currentEnemyState;
    protected SpriteRenderer sr;
    protected Animator anim; 

    [SerializeField] protected GameObject bloodParticlePrefab;

    [SerializeField] public float respawnTime = 3f; // Time before the enemy respawns
    [SerializeField] public Vector3 spawnPoint; // Set this in the Inspector or determine it programmatically

    [SerializeField] private GameObject playerObject;
    private AudioManager audioManager;
    private UnityEvent bonfireUsedEvent;

    public bool isDead = false;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>(); // Initialize here
        startingHealth = health; // Store the starting health
        spawnPoint = transform.position;

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        bonfireUsedEvent = playerObject.GetComponent<BonfireBehaviour>().bonfireUsedEvent;

        BoxCollider2D[] a = GetComponents<BoxCollider2D>(); 
        for (int i = 0; i < 2; ++i)
        {
            if (!a[i].isTrigger)
                Physics2D.IgnoreCollision(a[i], playerObject.GetComponent<BoxCollider2D>());
        }
    }

    protected virtual void Update()
    {
        if (isRecoiling)
        {
            if (recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
        else
        {
            UpdateEnemyStates();
        }

        if (health <= 0)
        {
            DisableComponents();
            HandleDeath();
        }
    }

    protected virtual void HandleDeath()
    {
        anim.SetBool("isDead", true);
        isDead = true;
        ChangeState(EnemyStates.fly_Death);

        TriggerRespawn();
    }

    public void DisableComponents()
    {
        sr.enabled = false; // Disable the SpriteRenderer
        BoxCollider2D[] a = GetComponents<BoxCollider2D>(); // Disable the BoxCollider2D
        for (int i = 0; i < 2; ++i)
        {
            a[i].enabled = false;
        }
    }

    protected virtual void Respawn()
    {
        health = startingHealth; // Reset health to starting value
        transform.position = spawnPoint; // Reset position
        anim.SetBool("isDead", false); // Reset death animation
        isDead = false;
        
        rb.velocity = Vector2.zero;
        ChangeState(EnemyStates.fly_Idle); // Set to idle state


        // Re-enable components
        sr.enabled = true; // Enable the SpriteRenderer
        BoxCollider2D[] a = GetComponents<BoxCollider2D>(); // Reenable the BoxCollider2Ds
        for (int i = 0; i < 2; ++i)
        {
            a[i].enabled = true;
        }
    }


    public void TriggerRespawn()
    {
        // StartCoroutine(RespawnCoroutine());
        bonfireUsedEvent.AddListener(Respawn);
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnTime);
        Respawn();
    }

    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        
        // [LOG2: SH]
        audioManager.PlaySFX(audioManager.takedamage);

        // Instantiate blood particles
        Instantiate(bloodParticlePrefab, transform.position, Quaternion.identity);
        
        if (!isRecoiling)
        {
            rb.velocity = _hitForce * recoilFactor * _hitDirection;
            isRecoiling = true;
        }
    }

    protected void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible)
        {
            Attack();
            // PlayerController.Instance.HitStopTime(0, 5, 0.5f); THIS IS CALLED IN PLAYERCONTROLLER.TAKEDAMAGE()
        }
    }

    protected virtual void UpdateEnemyStates()
    {
        // Implement state logic here
    }

    protected void ChangeState(EnemyStates _newState)
    {
        currentEnemyState = _newState;
        // Add any state transition logic here if needed
    }

    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }
}

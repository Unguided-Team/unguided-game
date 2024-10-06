
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossMain : MonoBehaviour
{
    public UnityEvent bossFightStarted;
    public UnityEvent bossKilledPlayer;
    public UnityEvent bossDied;

    private bool fightStarted = false;

    public UnityEvent bonfireUsedEvent;
    private AudioManager audioManager;
    public float health;
    public float maxHealth = 500;
    
    [SerializeField] public GameObject player;
    private BoxCollider2D playerCollider;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public BoxCollider2D bossPhysicsBodyCollider;

    // for damaging player
    [SerializeField] public GameObject bossBodyCollider;

    // for damaging player by attack1
    [SerializeField] public GameObject bossAttack1RangeCollider;
    [SerializeField] public GameObject bossAttack1HitCollider;

    // for damaging player by attack2
    [SerializeField] public GameObject bossAttack2RangeCollider;
    [SerializeField] public GameObject bossAttack2HitCollider;

    [SerializeField] public GameObject bossRoomDoor;

    [SerializeField] protected GameObject bloodParticlePrefab;

    [HideInInspector] public Animator anim;
    [HideInInspector] public SpriteRenderer sr;

    [HideInInspector] public bool isInvulnerable = false;
    [HideInInspector] public bool playerFound = false;
    private bool isFlipped = false;

    public bool canDoAttack1InRange = false;
    public bool canDoAttack1InHitzone = false;
    public bool canDoAttack2InRange = false;
    public bool canDoAttack2InHitzone = false;

    [SerializeField] public float bossDamage = 1f;
    [SerializeField] public float bossSpeed = 6f;
    [SerializeField] public float secondPhaseDamageMultiplier = 2f;
    [SerializeField] public float secondPhaseSpeedMultiplier = 1.5f;

    Vector3 originalPosition;

    public bool isHealing = false;
    public bool used20PHeal = false;
    public bool used40PHeal = false;
    public bool used60PHeal = false;
    public bool used80PHeal = false;

    public bool isDead = false;
    private bool playerKilled = false;

    private float healInterruptionTimer = 0;


    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        bossPhysicsBodyCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        
        playerCollider = player.GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(bossPhysicsBodyCollider, playerCollider);
        health = maxHealth;

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
	    originalPosition = transform.position;

        bonfireUsedEvent = player.GetComponent<BonfireBehaviour>().bonfireUsedEvent;
        bonfireUsedEvent.AddListener(resetBoss);
    }

    // Update is called once per frame
    void Update()
    {
        if (bossBodyCollider.GetComponent<Collider2D>().IsTouching(playerCollider))
            DamagePlayer();

        canDoAttack1InRange = CheckIfPlayerInAttack1Range();
        canDoAttack1InHitzone = CheckIfPlayerInAttack1Hitzone();

        canDoAttack2InRange = CheckIfPlayerInAttack2Range();
        canDoAttack2InHitzone = CheckIfPlayerInAttack2Hitzone();

        if (playerFound && !fightStarted) 
        {
            fightStarted = true;
            bossFightStarted.Invoke();
        }

        if (isHealing)
            healInterruptionTimer += Time.deltaTime;
    }

    public void LookAtPlayer()
    {
        Vector3 flipped = transform.localScale;
		flipped.z *= -1f;

		if (transform.position.x < player.transform.position.x && isFlipped)
		{
			transform.localScale = flipped;
			transform.Rotate(0f, 180f, 0f);
			isFlipped = false;
		}
		else if (transform.position.x > player.transform.position.x && !isFlipped)
		{
			transform.localScale = flipped;
			transform.Rotate(0f, 180f, 0f);
			isFlipped = true;
		}
	}

    public void PerformAttack1()
    {
        if (canDoAttack1InHitzone) 
        {
            DamagePlayer();
            if (canDoAttack2InHitzone) 
                anim.SetTrigger("Attack2");
        }
    }

    public void PerformAttack2()
    {
        if (canDoAttack1InHitzone) 
        {
            DamagePlayer();
            anim.ResetTrigger("Attack2");
        }
    }

    public bool CheckIfPlayerInAttack1Range()
    {
        return bossAttack1RangeCollider.GetComponent<Collider2D>().IsTouching(playerCollider);
    }

    public bool CheckIfPlayerInAttack1Hitzone()
    {
        return bossAttack1HitCollider.GetComponent<Collider2D>().IsTouching(playerCollider);
    }

    public bool CheckIfPlayerInAttack2Range()
    {
        return bossAttack2RangeCollider.GetComponent<Collider2D>().IsTouching(playerCollider);
    }

    public bool CheckIfPlayerInAttack2Hitzone()
    {
        return bossAttack2HitCollider.GetComponent<Collider2D>().IsTouching(playerCollider);
    }

    public void Heal()
    {
        health = Mathf.Min(health + maxHealth * 0.25f, maxHealth);
        anim.ResetTrigger("Attack1");
        anim.ResetTrigger("Attack2");
        isHealing = false;
        anim.SetBool("isHealing", false);
        print("healed");
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable)
            return;

        health -= damage;
        Instantiate(bloodParticlePrefab, transform.position, Quaternion.identity);
        audioManager.PlaySFX(audioManager.takedamage);
        if (health <= 0) 
        {
            anim.SetBool("Dead", true);
            isDead = true;

            bossDied.Invoke();
        }

        if (isHealing && healInterruptionTimer > 1f)
        {
            // got interrupted while healing
            isHealing = false;
            anim.SetBool("isHealing", false);
            print("interrupted");
        }


        if (health < 0.2 * maxHealth && !used20PHeal) 
        {
            anim.SetBool("isHealing", true);
            isHealing = true;
            used20PHeal = true;
            healInterruptionTimer = 0;
        }
        else if (health < 0.4 * maxHealth && !used40PHeal) 
        {
            anim.SetBool("isHealing", true);
            isHealing = true;
            used40PHeal = true;
            healInterruptionTimer = 0;
        }
        else if (health < 0.6 * maxHealth && !used60PHeal) 
        {
            anim.SetBool("isHealing", true);
            isHealing = true;
            used60PHeal = true;
            healInterruptionTimer = 0;
        }
        else if (health < 0.8 * maxHealth && !used80PHeal) 
        {
            anim.SetBool("isHealing", true);
            isHealing = true;
            used80PHeal = true;
            healInterruptionTimer = 0;
        }
    }

    public void resetBoss()
    {
        fightStarted = false;

        transform.position = originalPosition;
        health = maxHealth;
        resetHeals();

        anim.SetBool("Dead", false);
        anim.Rebind();
        anim.Update(0f);

        isDead = false;
        isInvulnerable = false;
        playerFound = false;
        playerKilled = false;

        sr.enabled = true;

        bossBodyCollider.GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = true;
        Physics2D.IgnoreCollision(bossBodyCollider.GetComponent<BoxCollider2D>(), playerCollider, false);
        bossRoomDoor.GetComponent<BoxCollider2D>().enabled = true;
        bossRoomDoor.GetComponent<BossRoomTransitionController>().deactivateDoorLock();
    }

    public void resetHeals()
    {
        isHealing = false;
        used20PHeal = false;
        used40PHeal = false;
        used60PHeal = false;
        used80PHeal = false;
    }
    
    public void RemoveBoss()
    {
        // sr.enabled = false;
        Physics2D.IgnoreCollision(bossBodyCollider.GetComponent<BoxCollider2D>(), playerCollider);

        bossBodyCollider.GetComponent<BoxCollider2D>().enabled = false;
        bossRoomDoor.GetComponent<BossRoomTransitionController>().deactivateDoorLock();
    }

    public void DamagePlayer() 
    {
        PlayerController.Instance.TakeDamage(1f);

        // died to boss = start fade to default bgm
        if (!playerKilled && player.GetComponent<PlayerController>().Health <= 0) 
        {
            bossKilledPlayer.Invoke();
            playerKilled = true;
        }
    }

    void OnEnterCollision2D(Collision2D collision)
    {
        
    }
}

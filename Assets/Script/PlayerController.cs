using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Camera: ")]
    [SerializeField] public GameObject mainCam;

    [HideInInspector] public PlayerStateList pState;
    public Animator anim;
    public Rigidbody2D rb;
    public BoxCollider2D playerCollider;
    public SpriteRenderer sr;

    //Input Variables
    public float xAxis, yAxis;
    public bool attack = false;
    private bool attack2 = false;

    public bool isAttacking = false;

    //creates a singleton of the PlayerController
    public static PlayerController Instance;


    [Header("Horizontal Movement Settings:")]
    [SerializeField] private float walkSpeed; //sets the players movement speed on the ground
    private float oldxAxis;
    [Space(5)]



    [Header("Vertical Movement Settings")]
    [SerializeField] private float jumpForce; //sets how hight the player can jump
    private bool canDoubleJump = false;

    private float gravity; //stores the gravity scale at start
    [Space(5)]



    [Header("Ground Check Settings:")]
    [SerializeField] private Transform groundCheckPoint; //point at which ground check happens
    [SerializeField] private float groundCheckY = 0.2f; //how far down from ground chekc point is Grounded() checked
    [SerializeField] private float groundCheckX = 0.5f; //how far horizontally from ground chekc point to the edge of the player is
    [SerializeField] private LayerMask groundLayer; //sets the ground layer
    [Space(5)]



    [Header("Wall Jump Settings: ")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallJumpCooldown;
    [SerializeField] private float wallPushPower;
    [SerializeField] private float wallJumpPower;
    private float timeSinceLastWallJump = 0;
    [Space(5)]


    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed; //speed of the dash
    [SerializeField] private float dashTime; //amount of time spent dashing
    [SerializeField] private float dashCooldown; //amount of time between dashes
    [SerializeField] GameObject dashEffect;
    private bool canDash = true, dashed;
    [Space(5)]



    [Header("Attack Settings:")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    [SerializeField] public LayerMask attackableLayer; //the layer the player can attack and recoil off of
    [SerializeField] public LayerMask attackableBossLayer; //[LOG1:SH]
    [SerializeField] private float timeBetweenAttack;
    [SerializeField] private float timeBetweenSpecialAttack;
    private float timeSinceAttack;
    private float timeSinceSpecialAttack;
    [SerializeField] private float damage; //the damage the player does to an enemy
    [SerializeField] private float specialAttackDamage;

    [Header("Up Attack Settings")]
    [SerializeField] private GameObject slashEffectPrefab;
    [SerializeField] private GameObject bloodParticlesPrefab;
    [SerializeField] private Transform upAttackPoint; // point for the up attack
    [SerializeField] private Vector2 upAttackArea; // size of the up attack area
    // [SerializeField] private Transform SideAttackTransform; //the middle of the side attack area
    // [SerializeField] private Vector2 SideAttackArea; //how large the area of side attack is

    // [SerializeField] private Transform UpAttackTransform; //the middle of the up attack area
    // [SerializeField] private Vector2 UpAttackArea; //how large the area of side attack is

    // [SerializeField] private Transform DownAttackTransform; //the middle of the down attack area
    // [SerializeField] private Vector2 DownAttackArea; //how large the area of down attack is

    // [SerializeField] private GameObject slashEffect; //the effect of the slashs

    bool restoreTime;
    float restoreTimeSpeed;
    [Space(5)]



    [Header("Recoil Settings:")]
    [SerializeField] private int recoilXSteps = 5; //how many FixedUpdates() the player recoils horizontally for
    [SerializeField] private int recoilYSteps = 5; //how many FixedUpdates() the player recoils vertically for

    [SerializeField] private float recoilXSpeed = 100; //the speed of horizontal recoil
    [SerializeField] private float recoilYSpeed = 100; //the speed of vertical recoil

    private int stepsXRecoiled, stepsYRecoiled; //the no. of steps recoiled horizontally and verticall
    [Space(5)]

    [Header("Health Settings")]
    [SerializeField] public int health;
    [SerializeField] public int maxHealth;
    // [SerializeField] GameObject bloodSpurt;
    [SerializeField] float hitFlashSpeed;


    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallback;

    
    AudioManager audioManager;
    private bool wasGrounded = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        Health = maxHealth;
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }


    // Start is called before the first frame update
    void Start()
    {
        pState = GetComponent<PlayerStateList>();

        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        
        gravity = rb.gravityScale;
        oldxAxis = xAxis;
    }

    private void OnDrawGizmos()
    {
    if (attackPoint == null || upAttackPoint == null)
        return;

    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    
    Gizmos.color = Color.blue; // Change color for up attack
    Gizmos.DrawWireSphere(upAttackPoint.position, attackRange);
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();

        RestoreTimeScale();
        FlashWhileInvincible();

        if (pState.dashing || pState.dead || pState.immobile) return;
        
        if (!pState.dead) 
        {
            Flip();
            Move();

            WallStick();
            Jump();

            UpdateGravity();
            UpdateJumpVariables();
            
            StartDash();

            Attack();
            SpecialAttack();
            TopAttack();
        }
    }

    private void FixedUpdate()
    {
        if (pState.dashing) return;
        Recoil();
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");
        attack2 = Input.GetButtonDown("AttackA");
        
    }

    public void Flip()
    {
        // if (oldxAxis != xAxis)
        // {
        //     GetComponent<ParticleBehaviour>().setDustCloud();
        //     oldxAxis = xAxis;
        // }

        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
            pState.lookingRight = false;
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            pState.lookingRight = true;
        }
    }

    private void Move()
    {
        if (onWall() && !Grounded()) {
            return;
        }
        
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
        anim.SetBool("Walking", rb.velocity.x != 0 && Grounded()); 
    }

    public void PlayMoveSound()
    {
        audioManager.PlaySFX(audioManager.playerWalkStep);
    }

    public void MakePlayerInvincible()
    {
        pState.invincible = true;
    }

    public void MakePlayerNotInvincible()
    {
        pState.invincible = false;
    }

    public void MakePlayerImmobile()
    {
        pState.immobile = true;
    }

    public void MakePlayerMobile()
    {
        pState.immobile = false;
    }

    void StartDash()
    {
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            audioManager.PlaySFX(audioManager.dash);
            StartCoroutine(Dash());
            dashed = true;
        }

        if (Grounded() || onWall())
        {
            dashed = false;
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;

        anim.SetTrigger("Dashing");
        anim.ResetTrigger("TopAttack");
        anim.ResetTrigger("SpecialAttack");

        rb.gravityScale = 0;
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        if (Grounded()) Instantiate(dashEffect, transform);

        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.2f);
        pState.invincible = true;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Attackable"), true);

        // [LOG1:SH]
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Boss"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("BossExtras"), true);
        
        yield return new WaitForSeconds(dashTime);
        
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
        pState.invincible = false;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Attackable"), false);

        // [LOG1:SH]
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Boss"), false);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("BossExtras"), false);

        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void playNormalAttackSound()
    {
        audioManager.PlaySFX(audioManager.attack);
    }

    public void playSpecialAttackSound()
    {
        audioManager.PlaySFX(audioManager.specialAttack);
    }

    // set special attack sound here [LOG2: SH] [attach to animation]

    void Attack()
    {
        timeSinceAttack += Time.deltaTime;

        if ((!Input.GetKey(KeyCode.W) && attack) && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            if (Grounded())
                anim.SetTrigger("Attacking");
            else
                anim.SetTrigger("JumpAttack");
            anim.ResetTrigger("TopAttack");
            anim.ResetTrigger("SpecialAttack");
            isAttacking = true; // Set attacking state
        }
    }

    void TopAttack()
    {
        timeSinceAttack += Time.deltaTime;

        // Check if the "W" key is pressed and the left mouse button is clicked
        if ((Input.GetKey(KeyCode.W) && Input.GetButtonDown("Attack")) && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("TopAttack"); // Trigger the UpAttack animation
            anim.ResetTrigger("Attacking");
            anim.ResetTrigger("SpecialAttack");
            isAttacking = true;
        }
    }

    void SpecialAttack()
    {
        timeSinceSpecialAttack += Time.deltaTime;
        if ((!Input.GetKey(KeyCode.W) && attack2) && timeSinceSpecialAttack >= timeBetweenSpecialAttack)
        {
            timeSinceSpecialAttack = 0;
            anim.SetTrigger("SpecialAttack");
            anim.ResetTrigger("Attacking");
            anim.ResetTrigger("TopAttack");

            rb.velocity = Vector2.zero;
            
            isAttacking = true; // Set attacking state
        }
    }

    void performAttack()
    {
        // Attack Logic
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, attackableLayer);
        
        // [LOG1:SH]
        Collider2D[] hitBosses = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, attackableBossLayer);
        HandleAttack(hitEnemies);
        HandleAttack(hitBosses);
    }

    void performTopAttack()
    {
        // Up Attack Logic
        Collider2D[] hitEnemiesUp = Physics2D.OverlapCircleAll(upAttackPoint.position, attackRange, attackableLayer);
        
        // [LOG1:SH]
        Collider2D[] hitBosses = Physics2D.OverlapCircleAll(upAttackPoint.position, attackRange, attackableBossLayer);
        HandleTopAttack(hitEnemiesUp);
        HandleTopAttack(hitBosses);
    }

    // called in the animation [LOG2: SH]
    void performSpecialAttack()
    {
        // Special Attack Logic [LOG1:SH]
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, attackableLayer);
        Collider2D[] hitEnemies2 = Physics2D.OverlapCircleAll(upAttackPoint.position, attackRange, attackableLayer);

        // [LOG1:SH]
        Collider2D[] hitBosses = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, attackableBossLayer);
        Collider2D[] hitBosses2 = Physics2D.OverlapCircleAll(upAttackPoint.position, attackRange, attackableBossLayer);

        // up attack register first for special attack
        HandleSpecialAttack(hitEnemies2);
        HandleSpecialAttack(hitBosses2);

        HandleSpecialAttack(hitEnemies);
        HandleSpecialAttack(hitBosses);
    }

    public void StopAttack()
    {
        isAttacking = false; // Stop dealing damage
    }

    private void InstantiateSlashEffect()
    {
        // Create the slash effect instance at the player's position
        GameObject slashEffect = Instantiate(slashEffectPrefab, attackPoint.position, Quaternion.identity);
        // Optionally, you can set the effect to face the right direction
        slashEffect.transform.localScale = new Vector3(transform.localScale.x, 1, 1);
        // Optionally, destroy the effect after some time (if it's a particle system, it may auto-destroy)
        Destroy(slashEffect, 1f); // Adjust time as needed
    }

    // [LOG1:SH]
    public void HandleAttack(Collider2D[] hitEntities)
    {
        if (!isAttacking) return; // Return if not attacking

        bool hit = false; // Flag to check if any enemy was hit

        foreach (Collider2D entity in hitEntities)
        {
            if (entity.GetComponent<Enemy>() != null)
            {
                Enemy enemyComponent = entity.GetComponent<Enemy>();
                Vector2 hitDirection = (entity.transform.position - transform.position).normalized;
                float hitForce = 10f;

                enemyComponent.EnemyHit(damage, hitDirection, hitForce);
                hit = true; // Set the flag to true since we hit an enemy
            }
            else if (entity.GetComponent<BossMain>() != null)
            {
                entity.GetComponent<BossMain>().TakeDamage(damage);
                hit = true; // Set the flag to true since we hit a boss
            }
        }

        if (hit)
        {
            // Instantiate the slash effect only if we hit an enemy
            InstantiateSlashEffect();

            HitStopTime(0.1f, 10, 0.035f); 

            // Camera Shake
            GetComponent<CameraShake>().lightCameraShakeEvent.Invoke();
        }
    }

    private void HandleTopAttack(Collider2D[] hitEntities)
    {
        if (!isAttacking) return; // Return if not attacking

        bool hit = false; // Flag to check if any enemy was hit

        foreach (Collider2D entity in hitEntities)
        {
            if (entity.GetComponent<Enemy>() != null)
            {
                Enemy enemyComponent = entity.GetComponent<Enemy>();
                Vector2 hitDirection = (entity.transform.position - transform.position).normalized;
                float hitForce = 10f;

                enemyComponent.EnemyHit(damage, hitDirection, hitForce);
                hit = true; // Set the flag to true since we hit an enemy
            }
            else if (entity.GetComponent<BossMain>() != null)
            {
                entity.GetComponent<BossMain>().TakeDamage(damage);
                hit = true; // Set the flag to true since we hit a boss
            }
        }

        if (hit)
        {
            HitStopTime(0.1f, 10, 0.035f); 

            // Camera Shake
            GetComponent<CameraShake>().lightCameraShakeEvent.Invoke();
        }
    }

    private void HandleSpecialAttack(Collider2D[] hitEntities)
    {
        if (!isAttacking) return; // Return if not attacking

        bool hit = false; // Flag to check if any enemy was hit

        foreach (Collider2D entity in hitEntities)
        {
            if (entity.GetComponent<Enemy>() != null)
            {
                Enemy enemyComponent = entity.GetComponent<Enemy>();
                Vector2 hitDirection = (entity.transform.position - transform.position).normalized;
                float hitForce = 10f;

                enemyComponent.EnemyHit(specialAttackDamage, hitDirection, hitForce);
                hit = true; // Set the flag to true since we hit an enemy
            }
            else if (entity.GetComponent<BossMain>() != null)
            {
                entity.GetComponent<BossMain>().TakeDamage(specialAttackDamage);
                hit = true; // Set the flag to true since we hit a boss
            }
        }

        if (hit)
        {
            // Instantiate the slash effect only if we hit an enemy
            // InstantiateSlashEffect();

            HitStopTime(0.1f, 10, 0.05f); 

            // Camera Shake
            GetComponent<CameraShake>().heavyCameraShakeEvent.Invoke();
        }
    }

    // void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilBool, Vector2 _recoilDir, float _recoilStrength)
    // {
    //     Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);
    //     List<Enemy> hitEnemies = new List<Enemy>();

    //     if (objectsToHit.Length > 0)
    //     {
    //         _recoilDir = pState.lookingRight ? Vector2.right : Vector2.left; // Determine the recoil direction
    //     } 

    //     for (int i = 0; i < objectsToHit.Length; i++)
    //     {
    //         Enemy e = objectsToHit[i].GetComponent<Enemy>();
    //         if (e && !hitEnemies.Contains(e))
    //         {
    //             e.EnemyHit(damage, _recoilDir, _recoilStrength);
    //             hitEnemies.Add(e);
    //         }
    //     }
    // }
    // void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    // {
    //     _slashEffect = Instantiate(_slashEffect, _attackTransform);
    //     _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
    //     _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    // }
    void Recoil()
    {
        if (pState.recoilingX)
        {
            if (pState.lookingRight)
            {
                rb.velocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(recoilXSpeed, 0);
            }
        }

        if (pState.recoilingY)
        {
            rb.gravityScale = 0;
            if (yAxis < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
            }
        }
        else
        {
            rb.gravityScale = gravity;
        }

        //stop recoil
        if (pState.recoilingX && stepsXRecoiled < recoilXSteps)
        {
            stepsXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }
        if (pState.recoilingY && stepsYRecoiled < recoilYSteps)
        {
            stepsYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }

        if (Grounded())
        {
            StopRecoilY();
        }
    }
    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }
    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }
    public void TakeDamage(float _damage)
    {
        // [LOG1:SH]
        if (pState.invincible || pState.resting || pState.dead)
            return;

        Health -= Mathf.RoundToInt(_damage);

        // Camera Shake
        GetComponent<CameraShake>().heavyCameraShakeEvent.Invoke();

        if (Health > 0) 
        {
            anim.SetTrigger("TakeDamage");
            audioManager.PlaySFX(audioManager.takedamage);
            StartCoroutine(StopTakingDamage());

            // Call HitStopTime for a dramatic effect
            HitStopTime(0.1f, 10, 0.35f); 
            FlashHit();
            
            // Instantiate blood particles when the player takes damage
            if (bloodParticlesPrefab != null)
            {
                GameObject bloodParticles = Instantiate(bloodParticlesPrefab, transform.position, Quaternion.identity);
                Destroy(bloodParticles, 1.5f); // Adjust time as needed
            }
            
            // Stop player movement for a brief moment [LOG1:SH] [FIX THIS??]
            StartCoroutine(StopMovementTemporarily(0.1f));
        } 
        else 
        {
            // Die logic
            if (!pState.dead)
            {
                // Call HitStopTime for a dramatic effect
                HitStopTime(0.1f, 10, 0.35f); 

                mainCam.GetComponent<Camera>().backgroundColor = new Color32(217, 217, 217, 255);
                pState.dead = true;
                anim.SetBool("WallSticking", false); // wallsticking can cause issues
                anim.SetTrigger("Death");
                audioManager.PlaySFX(audioManager.death);
                rb.velocity = Vector2.zero;
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Attackable"), true);
            }
        }
    }

private IEnumerator StopMovementTemporarily(float duration)
{
    // Freeze movement
    float originalWalkSpeed = walkSpeed;
    walkSpeed = 0; // Set walk speed to 0 to stop movement

    yield return new WaitForSeconds(duration); // Wait for the specified duration

    // Restore movement speed
    walkSpeed = originalWalkSpeed;
}




    IEnumerator StopTakingDamage()
    {
        // [sh4dman23] implement better iframes here

        pState.invincible = true;
        // GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        // Destroy(_bloodSpurtParticles, 1.5f);
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }

    private void FlashHit()
{
    StartCoroutine(HitFlash());
}

private IEnumerator HitFlash()
{
    Color originalColor = sr.color;
    Color hitColor = Color.red; // Change this color to whatever you want for hitAttack
    
    float flashDuration = 0.1f; // Duration of the flash
    float elapsedTime = 0f;

    while (elapsedTime < flashDuration)
    {
        sr.color = Color.Lerp(originalColor, hitColor, elapsedTime / flashDuration);
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    sr.color = originalColor; // Reset to original color
}
    void FlashWhileInvincible()
    {
        sr.material.color = pState.invincible ? Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, 1.0f)) : Color.white;
    }

    void RestoreTimeScale()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.unscaledDeltaTime * restoreTimeSpeed;
            }
            else
            {
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }

   public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
{
    restoreTimeSpeed = _restoreSpeed;

    if (_delay > 0)
    {
        StopCoroutine(StartTimeAgain(_delay));
        StartCoroutine(StartTimeAgain(_delay));
    }
    else
    {
        restoreTime = true;
    }
    
    Time.timeScale = _newTimeScale; // Set the time scale to the new value
}

IEnumerator StartTimeAgain(float _delay)
{
    yield return new WaitForSecondsRealtime(_delay);
    restoreTime = true; // This will start restoring the time scale in the RestoreTimeScale method
}


    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);

                if (onHealthChangedCallback != null)
                {
                    onHealthChangedCallback.Invoke();
                }
            }
        }
    }
    
    public bool Grounded()
    {
        return (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, groundLayer) 
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, groundLayer) 
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, groundLayer) 
            || Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, wallLayer) 
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, wallLayer) 
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, wallLayer));
    }

    public bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            playerCollider.bounds.center, playerCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer
        ); 
        return (raycastHit.collider != null);
    }

    void WallStick() 
    {
        if (timeSinceLastWallJump > wallJumpCooldown) {
            if (onWall() && !Grounded()) 
            {
                rb.velocity = Vector2.zero;
            } 
        } 
        else
        {
            timeSinceLastWallJump += Time.deltaTime;
        } 

        anim.SetBool("WallSticking", onWall() && !Grounded());
        anim.ResetTrigger("Attacking");
        anim.ResetTrigger("TopAttack");
        anim.ResetTrigger("SpecialAttack");
    }

   void Jump() 
{
    if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
    {
        pState.jumping = false;
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f); 
    }

    // Check if the player is grounded
    bool currentlyGrounded = Grounded();

    // Play landing sound when transitioning from airborne to grounded
    if (currentlyGrounded && !wasGrounded)
    {
        audioManager.PlaySFX(audioManager.land); // Play landing sound
    }

    if (Input.GetButtonDown("Jump") && Input.GetAxisRaw("Vertical") >= 0) 
    {
        if (currentlyGrounded || canDoubleJump) 
        {
            if (currentlyGrounded)
                canDoubleJump = true;
            else if (canDoubleJump)
                canDoubleJump = false;

            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (onWall() && !currentlyGrounded) {
            timeSinceLastWallJump = 0;
            rb.velocity = new Vector2((transform.localScale.x > 0 ? -1 : 1) * wallPushPower, wallJumpPower);
        }
    }

    // Update wasGrounded state
    wasGrounded = currentlyGrounded;

    anim.SetBool("Jumping", (!currentlyGrounded && !onWall() && rb.velocity.y > 0));
    anim.SetBool("Falling", (!currentlyGrounded && !onWall() && rb.velocity.y <= 0));
}

    void UpdateJumpVariables()
    {

        if (Grounded() && !Input.GetButton("Jump")) 
        {
            canDoubleJump = true;
        } 
    }

    void UpdateGravity() {
        // gravity while falling
        if (!Grounded() && !onWall() && rb.velocity.y >= 0) 
        {
            rb.gravityScale = gravity * 2f;
        } 
        // gravity while wallsticking
        else if (onWall() && !Grounded()) 
        {
            rb.gravityScale = gravity * 1.75f;
        }
        // normal
        else
            rb.gravityScale = gravity;
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if (pState.dead && collision.gameObject.CompareTag("Enemy"))  
        {
            Physics2D.IgnoreCollision(playerCollider, collision.gameObject.GetComponent<BoxCollider2D>());
        }
    }

    private void OnCollisionExit2D(Collision2D collision) 
    {
        if (pState.dead && collision.gameObject.CompareTag("Enemy")) 
        {
            Physics2D.IgnoreCollision(playerCollider, collision.gameObject.GetComponent<BoxCollider2D>(), false);
        }
    }
}
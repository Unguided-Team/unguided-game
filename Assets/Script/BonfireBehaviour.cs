using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class BonfireBehaviour : MonoBehaviour
{
    public UnityEvent bonfireUsedEvent;
    [SerializeField] private TMP_Text bonfireRestText;
    [SerializeField] private float maxAlpha;

    private PlayerStateList pState;
    private Animator anim;

    [SerializeField] public GameObject defaultBonfire;
    private GameObject lastBonfireCloseTo;
    public GameObject lastBonfireRestedAt;

    private bool inRange;
    private bool tooClose;

    AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        pState = GetComponent<PlayerStateList>();
        anim = GetComponent<Animator>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        if (lastBonfireCloseTo == null)
            lastBonfireCloseTo = defaultBonfire;
        if (lastBonfireRestedAt == null)
            lastBonfireRestedAt = defaultBonfire;

        bonfireRestText.SetText("");

        // start by resting 
        pState.resting = true;
        pState.immobile = true;
        pState.dead = false;

        // rest at default bonfire when starting game 
        // [!!CHANGELATER: change this to rest at the last saved bonfire when implementing saves]
        restAtBonfire(defaultBonfire);
    }

    // Update is called once per frame
    void Update()
    {
        checkPlayerRest();
    }

    void checkPlayerRest() 
    {
        if (pState.dead || pState.dashing)
            return;

        if (!inRange || tooClose)
        {
            // remove prompt to rest
            bonfireRestText.SetText("");
            pState.canRest = false;
        }
        else if (!pState.resting)
        {
            // add prompt to rest
            bonfireRestText.SetText("PRESS Q TO REST");
            pState.canRest = true;
        }
        
        if (pState.resting) 
        {
            bonfireRestText.SetText("PRESS Q TO GET UP");
        }

        if (Input.GetKeyDown(KeyCode.Q) && (pState.canRest || pState.resting) && !PauseMenu.GameIsPaused)
        {
            if (!pState.resting) 
            {
                restAtBonfire(lastBonfireCloseTo);
            }
            else
            {
                // remove promt to exit rest
                bonfireRestText.SetText("PRESS Q TO REST");

                GetComponent<PlayerController>().MakePlayerMobile();
                pState.resting = false;
                pState.canRest = false;
            }
        }

        anim.SetBool("Resting", pState.resting);
    }

    public void restAtBonfire(GameObject bonfire)
    {
        GetComponent<PlayerController>().Health = GetComponent<PlayerController>().maxHealth;
        
        GetComponent<PlayerController>().MakePlayerImmobile();

        if (bonfire.transform.position.x < transform.position.x)
            GetComponent<PlayerController>().xAxis = -1;
        else 
            GetComponent<PlayerController>().xAxis = 1;

        GetComponent<PlayerController>().Flip();

        pState.resting = true;
        
        // add prompt to exit rest
        bonfireRestText.SetText("PRESS Q TO GET UP");
        pState.dead = false;
        lastBonfireRestedAt = bonfire;
        anim.SetBool("Resting", true);
        bonfireUsedEvent.Invoke();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CheckpointRangeCheck"))
        {
            inRange = true;
            lastBonfireCloseTo = collision.gameObject;
        }
        if (collision.gameObject.CompareTag("Checkpoint"))
        {
            tooClose = true;
            lastBonfireCloseTo = collision.gameObject;
        }
        if (collision.gameObject.CompareTag("SoundRange"))
        {
            inRange = true;
            audioManager.PlaySFX(audioManager.bonfireidle);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CheckpointRangeCheck"))
        {
            inRange = false;
        }
        if (collision.gameObject.CompareTag("Checkpoint"))
        {
            tooClose = false;
        }
        if (collision.gameObject.CompareTag("SoundRange"))
        {
            inRange = false;
            audioManager.PlaySFX(audioManager.bonfireidle);
        }

    }
    
}

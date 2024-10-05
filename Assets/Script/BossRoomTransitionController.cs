using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossRoomTransitionController : MonoBehaviour
{
    private AudioManager audioManager;

    public UnityEvent bonfireUsedEvent;
    [SerializeField] private GameObject boss;

    private bool isInTransitionTrigger = false;
    [SerializeField] private GameObject player;
    private Vector2 prevPos;
    private GameObject roomLocker;

    // Start is called before the first frame update
    void Start()
    {
        roomLocker = transform.GetChild(0).gameObject;   
        roomLocker.GetComponent<BoxCollider2D>().enabled = false;

        if (boss == null)
            boss = GameObject.FindGameObjectsWithTag("Boss")[0];

        bonfireUsedEvent = player.GetComponent<BonfireBehaviour>().bonfireUsedEvent;
        bonfireUsedEvent.AddListener(deactivateDoorLock);

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInTransitionTrigger = true;
            prevPos = new Vector2(player.transform.position.x, player.transform.position.y);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // !!CHANGELATER: add check for boss not dead
        if (collision.gameObject.CompareTag("Player") && isInTransitionTrigger && !boss.GetComponent<BossMain>().isDead)
        {
            isInTransitionTrigger = false;

            Vector2 newPos = new Vector2(player.transform.position.x, player.transform.position.y);
            Vector2 triggerPos = new Vector2(transform.position.x, transform.position.y);

            bool crossedTheTrigger = false;

            // since boss room is to the right
            if (prevPos.x < triggerPos.x && newPos.x > triggerPos.x)
                crossedTheTrigger = true;

            if (crossedTheTrigger)
                activateDoorLock();
        }
    }

    public void activateDoorLock()
    {
        // trigger the boss
        boss.GetComponent<BossMain>().playerFound = true;
        boss.GetComponent<Animator>().SetBool("PlayerFound", true);

        // lock room
        roomLocker.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void deactivateDoorLock()
    {
        // untrigger the boss
        boss.GetComponent<BossMain>().playerFound = false;
        boss.GetComponent<Animator>().SetBool("PlayerFound", false);

        // unlock room
        roomLocker.GetComponent<BoxCollider2D>().enabled = false;
    }
}

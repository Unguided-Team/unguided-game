using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private GameObject currentOneWayPlatform;
    private BoxCollider2D playerCollider;

    void Start() 
    {
        playerCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {   
        if ((Input.GetAxisRaw("Vertical") < 0 && Input.GetButtonDown("Jump"))) 
        {
            if (currentOneWayPlatform != null) {
                StartCoroutine(DisableCollision());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("OneWayPlatform")) 
        {
            currentOneWayPlatform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("OneWayPlatform")) 
        {
            currentOneWayPlatform = null;
        }
    }

    private IEnumerator DisableCollision() 
    {
        BoxCollider2D platformCollider = currentOneWayPlatform.GetComponent<BoxCollider2D>();
        PolygonCollider2D polygonPlatformCollider = currentOneWayPlatform.GetComponent<PolygonCollider2D>();
        
        if (platformCollider != null) 
            Physics2D.IgnoreCollision(playerCollider, platformCollider);
        else
            Physics2D.IgnoreCollision(playerCollider, polygonPlatformCollider);

        yield return new WaitForSeconds(0.25f);

        if (platformCollider != null) 
            Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
        else    
            Physics2D.IgnoreCollision(playerCollider, polygonPlatformCollider, false);
    }
}

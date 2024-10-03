using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBehaviour : MonoBehaviour
{
    [Header("Particles: ")]
    [SerializeField] private GameObject groundCheckPoint;
    [SerializeField] private ParticleSystem dustCloud;
    [SerializeField] private ParticleSystem wallSlideDustCloud;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        dustCloudOnMove();
        dustCloudOnSlide();
    }

    private void dustCloudOnMove() 
    {
        if (GetComponent<Rigidbody2D>().velocity.x != 0 && GetComponent<PlayerController>().Grounded()) {
            dustCloud.Play();
        }
    }

    // needs fixing
    private void dustCloudOnSlide()
    {
        if (!GetComponent<PlayerController>().Grounded() && GetComponent<PlayerController>().onWall())
        {
            wallSlideDustCloud.Play();
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if ((collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")) || collision.gameObject.layer.Equals(LayerMask.NameToLayer("Wall"))) 
            && GetComponent<PlayerController>().Grounded())
        {
            dustCloud.Play();
        }
    }
}

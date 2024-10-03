using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEnemyBloodParticle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(destroyParticles());
    }

    private IEnumerator destroyParticles()
    {
        float duration = GetComponent<ParticleSystem>().main.duration;
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossFader : MonoBehaviour
{
    [SerializeField] private GameObject fadeImage;
    // Start is called before the first frame update
    void Start()
    {
        fadeImage.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

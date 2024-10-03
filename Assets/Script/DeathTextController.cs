using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeathTextController : MonoBehaviour
{
    [SerializeField] private GameObject deathTextContainer;
    [SerializeField] private TMP_Text deathText;
    private float maxAlpha;
    private float maxFontSize;

    // Start is called before the first frame update
    void Start()
    {
        maxFontSize = deathText.fontSize;
        maxAlpha = deathTextContainer.GetComponent<Image>().color.a;
        removeOverlay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startFadeIn()
    {
        StartCoroutine(fadeInDeathText());
    }

    public void startFadeOut()
    {
        StartCoroutine(fadeOutDeathText());
    }

    public void removeOverlay() 
    {
        Color c = deathTextContainer.GetComponent<Image>().color;
        c.a = 0f;
        deathTextContainer.GetComponent<Image>().color = c;
        
        c = deathText.color;
        c.a = 0;
        deathText.color = c;
    }

    // [sh4dman23: fades in the text container; called by animator in keyframes window]
    public IEnumerator fadeInDeathText()
    {
        deathText.fontSize = maxFontSize - 10;
    
        for (float alpha = 0.05f; alpha < maxAlpha; alpha += 0.05f)
        {
            Color c = deathTextContainer.GetComponent<Image>().color;
            c.a = alpha;
            deathTextContainer.GetComponent<Image>().color = c;
            c = deathText.color;
            c.a = alpha;
            deathText.color = c;
            deathText.fontSize += 0.5f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public IEnumerator fadeOutDeathText()
    {
        deathText.fontSize = maxFontSize;

        for (float alpha = 0.05f; alpha < maxAlpha; alpha += 0.05f)
        {
            Color c = deathTextContainer.GetComponent<Image>().color;
            c.a -= alpha;
            deathTextContainer.GetComponent<Image>().color = c;
            c = deathText.color;
            c.a -= alpha;
            deathText.color = c;
            deathText.fontSize -= 0.5f;
            yield return new WaitForSeconds(0.05f);
        }
    }
}

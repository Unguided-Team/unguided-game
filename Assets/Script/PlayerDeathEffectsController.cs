using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerDeathEffectsController : MonoBehaviour
{
    [SerializeField] private GameObject deathTextContainer;
    [SerializeField] private TMP_Text deathText;
    [SerializeField] private GameObject deathFadeOverlay;
    [SerializeField] private float deathOverlayFadeTime = 0.5f;

    private float maxAlpha;
    private float maxFontSize;

    private bool justFadedIn = false;

    // Start is called before the first frame update
    void Start()
    {
        maxFontSize = deathText.fontSize;
        maxAlpha = deathTextContainer.GetComponent<Image>().color.a;
        removeDeathTextOverlay();

        deathFadeOverlay.GetComponent<Image>().color = new Color(0, 0, 0, 0);

        GetComponent<BonfireBehaviour>().bonfireUsedEvent.AddListener(startDeathOverlayFadeOut);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startDeathTextFadeIn()
    {
        StartCoroutine(fadeInDeathText());
    }

    public void startDeathTextFadeOut()
    {
        StartCoroutine(fadeOutDeathText());
    }

    public void removeDeathTextOverlay() 
    {
        Color c = deathTextContainer.GetComponent<Image>().color;
        c.a = 0f;
        deathTextContainer.GetComponent<Image>().color = c;
        
        c = deathText.color;
        c.a = 0;
        deathText.color = c;
    }

    // [sh4dman23: fades in the text container; called by animator in keyframes window]
    private IEnumerator fadeInDeathText()
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

    private IEnumerator fadeOutDeathText()
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

    public void startDeathOverlayFadeIn()
    {
        StartCoroutine(fadeInDeathOverlay());
    }

    public void startDeathOverlayFadeOut()
    {
        StartCoroutine(fadeOutDeathOverlay());
    }

    private IEnumerator fadeInDeathOverlay()
    {
        deathFadeOverlay.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        deathFadeOverlay.SetActive(true);

        for (float alpha = 0, waitTime = (deathOverlayFadeTime / (1 / 0.05f)); alpha < 1; alpha += 0.05f)
        {
            deathFadeOverlay.GetComponent<Image>().color = new Color(0, 0, 0, Mathf.Min(1, alpha));
            yield return new WaitForSeconds(waitTime);
        }

        deathFadeOverlay.GetComponent<Image>().color = new Color(0, 0, 0, 1);
        justFadedIn = true;
    }

    // make sure to keep at least deathOverlayFadeTime seconds of gap 
    // between fadein start and respawn (in player_death animation)
    private IEnumerator fadeOutDeathOverlay()
    {
        yield return new WaitForSeconds(deathOverlayFadeTime);

        if (justFadedIn) {
            for (float alpha = deathFadeOverlay.GetComponent<Image>().color.a, waitTime = (deathOverlayFadeTime / (1 / 0.05f)); alpha > 0; alpha -= 0.05f)
            {
                deathFadeOverlay.GetComponent<Image>().color = new Color(0, 0, 0, Mathf.Max(0, alpha));
                yield return new WaitForSeconds(waitTime);
            }
        }

        deathFadeOverlay.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        justFadedIn = false;
        deathFadeOverlay.SetActive(false);
    }
}

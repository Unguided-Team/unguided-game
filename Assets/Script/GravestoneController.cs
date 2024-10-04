using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GravestoneBehaviour : MonoBehaviour
{
    [SerializeField] private TMP_Text gravestoneTextBox;
    [SerializeField] private string gravestoneTextLine1 = "";
    [SerializeField] private string gravestoneTextLine2 = "";
    [SerializeField] private string gravestoneTextLine3 = "";
    [SerializeField] private string gravestoneTextLine4 = "";

    // Start is called before the first frame update
    void Start()
    {
        gravestoneTextBox.SetText("");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            gravestoneTextBox.SetText(
                gravestoneTextLine1 + "\n" + gravestoneTextLine2 + "\n" + gravestoneTextLine3 + "\n" + gravestoneTextLine4
            );
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gravestoneTextBox.SetText("");
        }
    }
}

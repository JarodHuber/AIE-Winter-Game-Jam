using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialoge : MonoBehaviour
{
    public enum PersonTalking
    {
        None = -1,
        Scrooge,
        Other
    }

    [Tooltip("How many seconds until the next char appears")]
    public Timer textSpeed = new Timer(.01f);
    [Header("Fill these with the dialoge and whose speaking")]
    [TextArea]
    public string[] dialoge;
    public PersonTalking[] personTalking;
    [Header("Do Not Touch, some talking overlays")]
    public GameObject[] fades;

    Text text = null;
    bool textFinished = false;
    int charCounter = 0;
    int stringCounter = 0;

    private void Start()
    {
        text = GetComponentInChildren<Text>();
    }

    private void Update()
    {
        for (int x = 0; x < fades.Length; x++)
        {
            if(x != (int)personTalking[stringCounter])
            {
                fades[x].SetActive(true);
            }
            else
            {
                fades[x].SetActive(false);
            }
        }

        if(!textFinished)
        {
            if (Input.GetMouseButtonDown(0))
            {
                text.text = dialoge[stringCounter];

                textSpeed.Reset();
                charCounter = 0;

                textFinished = true;

                return;
            }

            if (textSpeed.Check())
            {
                if (charCounter < dialoge[stringCounter].Length)
                {
                    text.text += dialoge[stringCounter][charCounter];
                    charCounter++;
                }
                else
                {
                    text.text = dialoge[stringCounter];
                    charCounter = 0;

                    textFinished = true;
                }
            }

            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            stringCounter++;

            if(stringCounter >= dialoge.Length)
            {
                // Exit dialoge

                GameObject.FindGameObjectWithTag("GameController").GetComponent<EnemyManager>().paused = false;

                Destroy(gameObject);
                return;
            }

            text.text = "";
            textFinished = false;
        }
    }
}

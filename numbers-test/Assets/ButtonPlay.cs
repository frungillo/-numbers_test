using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonPlay : MonoBehaviour
{
    public Text txt;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckInternetConnection((isConnected) => {
            if(!isConnected)
            {
                showToast("Connessione internet assente...", 2);
            } else
            {
                showToast("Connessione internet OK", 2);
            }
        }));
    }

   public void ClickOptions()
    {

    }

   public  void TaskOnClickPlaySolo()
    {
        SceneManager.LoadScene("ScenaDownload");
    }
    
    public void TaskOnClickMutiplayer()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public System.Collections.IEnumerator CheckInternetConnection(Action<bool> action)
    {

        UnityWebRequest www = UnityWebRequest.Get("http://numbers.jemaka.it");
        yield return www.SendWebRequest();

        if (www.error != null)
        {
            action(false);
        }
        else
        {
            action(true);
        }

    }

   

    void showToast(string text, int duration)
    {
        StartCoroutine(showToastCOR(text, duration));
    }

    private IEnumerator showToastCOR(string text,int duration)
    {
        Color orginalColor = txt.color;

        txt.text = text;
        txt.enabled = true;

        //Fade in
        yield return fadeInAndOut(txt, true, 0.5f);

        //Wait for the duration
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        //Fade out
        yield return fadeInAndOut(txt, false, 0.5f);

        txt.enabled = false;
        txt.color = orginalColor;
    }

    IEnumerator fadeInAndOut(Text targetText, bool fadeIn, float duration)
    {
        //Set Values depending on if fadeIn or fadeOut
        float a, b;
        if (fadeIn)
        {
            a = 0f;
            b = 1f;
        }
        else
        {
            a = 1f;
            b = 0f;
        }

        Color currentColor = Color.clear;
        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(a, b, counter / duration);

            targetText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }
    }



}

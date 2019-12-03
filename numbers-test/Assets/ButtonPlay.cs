using System;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;

public class ButtonPlay : MonoBehaviour
{
    public Text txtToastLabel;
    public Text txtMonitor;

    public Button btnSolo;
    public Button btnMultiPlay;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckInternetConnection((isConnected) => {
            if(!isConnected)
            {
                showToast("Connessione internet assente...", 2);
                btnSolo.enabled = false;
                btnMultiPlay.enabled = false;
            } else
            {
                //showToast("Connessione internet OK", 2);
            }
        }));
#if !PLATFORM_IOS
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
        SignIn();
#endif
        
    }

#if !PLATFORM_IOS
    void SignIn()
    {
        Social.localUser.Authenticate(success => {
            showToast("Stato connessione:" + success.ToString(), 2);
            
            PlayGamesPlatform.Instance.Authenticate(suc => {
                showToast("Stato Play auth:" + suc.ToString(), 2);
                txtMonitor.text = "USER: " + PlayGamesPlatform.Instance.GetUserDisplayName().ToUpper();
                StartCoroutine(SetUser(PlayGamesPlatform.Instance.GetIdToken()));
            });
        });
    }
#endif

#region Risultati

#if !PLATFORM_IOS
    public void SbloccaRisultati(string id)
    {
        Social.ReportProgress(id, 100, success => { });
    }

    public static void IncrementaRisultati(string id, int StepDaIncrementare)
    {
        PlayGamesPlatform.Instance.IncrementAchievement(id, StepDaIncrementare, success => { });
    }

    public static void MostraRisultatiUI()
    {
        Social.ShowAchievementsUI();
        Debug.Log("UI ATTIVATA");
    }
#endif
#endregion /Risultati

    /*Pulsante Opzioni*/
    public void ClickOptions()
    {
        Debug.Log("Options Clicked!");
#if !PLATFORM_IOS
        MostraRisultatiUI();
#endif

    }

    /*Pulsante SOLO*/
   public  void TaskOnClickPlaySolo()
    {
        SceneManager.LoadScene("ScenaDownload");
    }

    /*Pulsante SFIDA*/
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

    private IEnumerator SetUser(string uuid)
    {
        bool state = false;
        using (UnityWebRequest request = UnityWebRequest.Get("http://numbers.jemaka.it/api/Utenti?uuid=" + uuid))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError("Request Error: " + request.error);
            }
            else
            {
                yield return new WaitForSeconds(1);
                string JsonText = request.downloadHandler.text;
                state = JsonConvert.DeserializeObject<bool>(JsonText);
            }

        }

        if (!state)
        {
            WWWForm form = new WWWForm();
            
            form.AddField("Id_user", "");
            form.AddField("Nickname", PlayGamesPlatform.Instance.GetUserDisplayName().ToUpper());
            form.AddField("Imei", "no");
            form.AddField("Uuid", PlayGamesPlatform.Instance.GetIdToken());
            form.AddField("Data_setup", DateTime.UtcNow.ToString());
            form.AddField("Email", PlayGamesPlatform.Instance.GetUserEmail());
            form.AddField("Service_id", PlayGamesPlatform.Instance.GetUserId());
            form.AddField("Note", "Google Play User");
            form.AddField("Score1", "0");
            form.AddField("Score2", "0");
            form.AddField("Bonus1", "0");
            form.AddField("Bonus2", "0");
            
            /*
            form.AddField("Id_user", "");
            form.AddField("Nickname", "Genny Frungillo");
            form.AddField("Imei", "no");
            form.AddField("Uuid", "ABCDE13999293CFED39209mfds930192jd9832d21de");
            form.AddField("Data_setup", DateTime.UtcNow.ToString() );
            form.AddField("Email", "gennyfrungillo@gmail.com");
            form.AddField("Service_id", "654465156516");
            form.AddField("Note", "Google Play User");
            form.AddField("Score1", "0");
            form.AddField("Score2", "0");
            form.AddField("Bonus1", "0");
            form.AddField("Bonus2", "0");
            */
            using (UnityWebRequest request = UnityWebRequest.Post("http://numbers.jemaka.it/api/Utenti", form))
            {
                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError("Request Error: " + request.error);
                }
               

            }
        }
    }




    void showToast(string text, int duration)
    {
        StartCoroutine(showToastCOR(text, duration));
    }

    private IEnumerator showToastCOR(string text,int duration)
    {
        Color orginalColor = txtToastLabel.color;

        txtToastLabel.text = text;
        txtToastLabel.enabled = true;

        //Fade in
        yield return fadeInAndOut(txtToastLabel, true, 0.5f);

        //Wait for the duration
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        //Fade out
        yield return fadeInAndOut(txtToastLabel, false, 0.5f);

        txtToastLabel.enabled = false;
        txtToastLabel.color = orginalColor;
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

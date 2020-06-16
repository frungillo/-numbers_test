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



public class scene_0_script : MonoBehaviour
{
    [Header("Campi Funzionali")]
    public Text txtOutComic;
    public InputField txtNomeGiocatore;
    public Button btnInviaNome;
    public Button btnFacebook;
    public Button btnGoogle;



    private void Awake()
    {
        txtNomeGiocatore.enabled = false;
        btnInviaNome.enabled = false;
        btnFacebook.enabled = false;
        btnGoogle.enabled = false;

        DatiGioco.user = new Users();
        DatiGioco.user.Imei = SystemInfo.deviceUniqueIdentifier;
        Debug.Log($"ID_Device:{DatiGioco.user.Imei}");
        StartCoroutine(CheckUserByDevice());

        
       

        
    }

   

    #region FacebookConnector
    private void InitCallback()
    {
       
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        string msg = "Ciao!--Sono Albert!--Voglio presentarti il mio mondo fatto di numeri--Iniziamo a conoscerci...--Puoi scegliere se presentarti con il tuo social--...oppure mi dici semplicemente il tuo nome!";
        ScriviComic(msg);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


   
    public void btnPlayGameClick()
    {
      #if !PLATFORM_IOS
      //  PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
       // PlayGamesPlatform.InitializeInstance(config);
       // PlayGamesPlatform.Activate();
        SignIn();
      #endif

    }
    public void btnInviaNomeClick()
    {
        if(txtNomeGiocatore.text.Trim() =="")
        {
            ScriviComic("HEY! Dimmi almeno il tuo nome!");
            return;
        }
        DatiGioco.user.Service_id = SystemInfo.deviceUniqueIdentifier;
        DatiGioco.user.Nickname = txtNomeGiocatore.text;
        DatiGioco.user.Note = "ND";
        StartCoroutine(SetUser(DatiGioco.user, "ND"));

        //SceneManager.LoadScene("ScenaMenu");
    }


#if !PLATFORM_IOS
    public void SbloccaRisultati(string id)
    {
        Social.ReportProgress(id, 100, success => { });
    }

    public static void IncrementaRisultati(string id, int StepDaIncrementare)
    {
        //PlayGamesPlatform.Instance.IncrementAchievement(id, StepDaIncrementare, success => { });
    }

    public static void MostraRisultatiUI()
    {
        Social.ShowAchievementsUI();
        Debug.Log("UI ATTIVATA");
    }
#endif

    /*Pulsante Opzioni*/
    public void ClickOptions()
    {
        Debug.Log("Options Clicked!");
#if !PLATFORM_IOS
        MostraRisultatiUI();
#endif

    }

    private void ScriviComic(string msg)
    {
        StartCoroutine(scrivi(msg, 0.08F));
    }

    IEnumerator scrivi(string msg, float interval)
    {
        yield return new WaitForSeconds(1F);
        txtOutComic.text = "";

        string[] arrTxt = msg.Split(new string[] { "--" }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string item in arrTxt)
        {
            txtOutComic.text = "";
            foreach (char itm in item.ToCharArray())
            {
                yield return new WaitForSeconds(interval);
                txtOutComic.text += itm;
            }
            yield return new WaitForSeconds(1.3F);
        }

    }

#if !PLATFORM_IOS
    void SignIn()
    {
        Social.localUser.Authenticate(success => {
            //showToast("Stato connessione:" + success.ToString(), 2);
            /*
            PlayGamesPlatform.Instance.Authenticate(suc => {
                // showToast("Stato Play auth:" + suc.ToString(), 2);
                //DatiGioco.UserID= PlayGamesPlatform.Instance.GetUserDisplayName().ToUpper();
                StartCoroutine(LoadGooglePlayProfileImage(PlayGamesPlatform.Instance.GetUserImageUrl()));
                Users us = new Users();
                us.Nickname = PlayGamesPlatform.Instance.GetUserDisplayName().ToUpper();
                us.Uuid = PlayGamesPlatform.Instance.GetIdToken();
                us.Email = PlayGamesPlatform.Instance.GetUserEmail();
                us.Service_id = PlayGamesPlatform.Instance.GetUserId();
                us.Imei = SystemInfo.deviceUniqueIdentifier;
                DatiGioco.user = us;
                StartCoroutine(SetUser(us, "Google Play Games"));
            });
            */
        });
    }
#endif

    private IEnumerator LoadGooglePlayProfileImage(string urlImage)
    {
        using (UnityWebRequest req  = UnityWebRequest.Get(urlImage))
        {
            yield return req.SendWebRequest();
            DatiGioco.user.UserProfileImage.GetComponent<Image>().sprite = Sprite.Create( ((DownloadHandlerTexture)req.downloadHandler).texture, new Rect(0, 0, 128, 128), new Vector2());

        }
        
    }

    /**/
    private IEnumerator CheckUserByDevice()
    {
        int state;
        using (UnityWebRequest request = UnityWebRequest.Get("http://numbers.jemaka.it/api/Utenti?imei=" + SystemInfo.deviceUniqueIdentifier))
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
                state = JsonConvert.DeserializeObject<Int32>(JsonText);
                Debug.Log($"Id_Giocatore:{state.ToString()}");
                if(state > -1)
                {
                    Debug.Log("Giocatore Esistente");
                    /*Gestione dello stato di attivazione social dell'utente*/
                    /*è necessario disattivare i tasti social*/
                    /*se invece non entra qui dentro, significa che nonn è attivo e bisogna attivare i tasti.*/
                    /*Il giocatore esiste gia. Va scaricato*/

                    StartCoroutine(GetUserByID(state));
                }
                else
                {
                    Debug.Log("Giocatore non esistente");
                    /*Utente non registrato, attivo pulsanti e intefaccia.*/
                    txtNomeGiocatore.enabled = true;
                    btnInviaNome.enabled = true;
                    btnFacebook.enabled = true;
                    btnGoogle.enabled = true;
                }
            }

        }
    }
    private IEnumerator GetUserByID(int idUser)
    {
        using (UnityWebRequest request = UnityWebRequest.Get("http://numbers.jemaka.it/api/Utenti/"+idUser.ToString()))
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
                DatiGioco.user = JsonConvert.DeserializeObject<Users>(JsonText);

                
                if (DatiGioco.user.Note== "Google Play Games")
                {
                    Debug.Log("GooglePlayUser");
 #if !PLATFORM_IOS
                  //  StartCoroutine(LoadGooglePlayProfileImage(PlayGamesPlatform.Instance.GetUserImageUrl()));
                    yield return new WaitForSeconds(3);
#endif
                }
              
                SceneManager.LoadScene("ScenaMenu");

            }
        }
    }

    public void onClikPolicyButton()
    {
        Application.OpenURL("http://www.obbar.it/homepage/number-privacy-policy/");
    }


    private IEnumerator SetUser(Users u, String platform)
    {
          WWWForm form = new WWWForm();

     
        form.AddField("Id_user", "");

        form.AddField("Nickname", u.Nickname);
        form.AddField("Imei", u.Imei);
        form.AddField("Uuid", u.Uuid);
        form.AddField("Data_setup", DateTime.UtcNow.ToString());
        form.AddField("Email", u.Email);
        form.AddField("Service_id", u.Service_id);
        form.AddField("Note", platform);
        form.AddField("Single_Score", "0");
        form.AddField("Levels", "0");
        form.AddField("Money", "3");
        form.AddField("Match_score", "1");

           
            using (UnityWebRequest request = UnityWebRequest.Post("http://numbers.jemaka.it/api/Utenti", form))
            {
                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError("Request Error: " + request.error);
                }


            }
        
        DatiGioco.user = u;
       
        SceneManager.LoadScene("ScenaMenu");
    }
}

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
using Facebook.Unity;

public class scene_0_script : MonoBehaviour
{
    [Header("Campi Funzionali")]
    public Text txtOutComic;
    public InputField txtNomeGiocatore;
    public Button btnInviaNome;
    public Button btnFacebook;
    public Button btnGoogle;

    [Header("Musica")]
    public List<AudioClip> Temi;

    AudioSource audioS;


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

        
        if (!FB.IsInitialized)
        {
            Debug.Log("Facebook non inizializzato...");
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            FB.ActivateApp();
            //FB.API("me/picture?type=med", HttpMethod.GET, GetUserPhoto_Face);
            //è necessario capire se il giocatore gia ha fatto una login social.
            Debug.Log("ApiFacebook inizializata");
            //CreateFacebookUser();

        }

        audioS = GetComponent<AudioSource>();
        audioS.PlayOneShot(Temi[0], 1F);
        
    }

    void GetUserPhoto_Face(IGraphResult result)
    {
        if (result.Error==null)
        {
            DatiGioco.user.UserProfileImage.GetComponent<Image>().sprite = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());
            Debug.Log("Immegine Profilo caricata");
        } else
        {
            Debug.Log("Errore FaceSDK:" + result.Error);
        }
    }

    #region FacebookConnector
    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            Debug.Log("Facebook inizializzato");
        }
        else
        {
            Debug.Log("Impossibile inizializzare Facebook SDK");
        }
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

    private void CreateFacebookUser()
    {
       // FB.API("me/picture?type=med", HttpMethod.GET, GetUserPhoto_Face);
        FB.API("me?fields=address,birthday,friends{name},name", HttpMethod.GET, GetUserData_Face);
    }

    private void GetUserData_Face(IGraphResult result)
    {
        Debug.Log("Inizio carticamento dati utente facebook");
        FacebookUser u = JsonUtility.FromJson<FacebookUser>(result.RawResult);
        DatiGioco.user.Nickname = u.name;
        DatiGioco.user.Service_id = SystemInfo.deviceUniqueIdentifier; // u.id //int.Parse( u.id);
        DatiGioco.user.Uuid = u.id;//SystemInfo.deviceUniqueIdentifier;
        Debug.Log("Dati Utente Facebook Scaricati");
        Debug.Log($"Dati Utente{DatiGioco.user.Nickname}");
        Debug.Log($"Dati Utente{DatiGioco.user.Service_id}");
        Debug.Log($"Dati Utente{DatiGioco.user.Uuid}");
        StartCoroutine(SetUser(DatiGioco.user, "Facebook User"));
    }


    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
           // showToast(aToken.UserId, 3);
            DatiGioco.user.Uuid = aToken.UserId;
            // Print current access token's granted permissions
            CreateFacebookUser();
            
        }
        else
        {
            Debug.Log("User cancelled login");
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


    public void btnFacebookClick()
    {
        /*Routine di connessione con facebook*/
        var perms = new List<string>() { "public_profile", "email" };
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }

    public void btnPlayGameClick()
    {
      #if !PLATFORM_IOS
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
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
        PlayGamesPlatform.Instance.IncrementAchievement(id, StepDaIncrementare, success => { });
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
 #if !PLATFORM_IOS
                    StartCoroutine(LoadGooglePlayProfileImage(PlayGamesPlatform.Instance.GetUserImageUrl()));
                    yield return new WaitForSeconds(3);
#endif
                }
                else if(DatiGioco.user.Note== "Facebook User")
                {
                    FB.API("me/picture?type=med", HttpMethod.GET, GetUserPhoto_Face);
                    yield return new WaitForSeconds(3);
                }
                SceneManager.LoadScene("ScenaMenu");

            }
        }
    }


    private IEnumerator SetUser(Users u, String platform)
    {
          WWWForm form = new WWWForm();

        Debug.Log("1");

        form.AddField("Id_user", "");

        Debug.Log("2");
        form.AddField("Nickname", u.Nickname);
        Debug.Log("3"); 
        form.AddField("Imei", u.Imei);
        Debug.Log("4");
        form.AddField("Uuid", u.Uuid);
        Debug.Log("5");
        form.AddField("Data_setup", DateTime.UtcNow.ToString());
        Debug.Log("6");
        form.AddField("Email", u.Email);
        Debug.Log("7");
        form.AddField("Service_id", u.Service_id);
        Debug.Log("8");
        form.AddField("Note", platform);
        Debug.Log("9");
        form.AddField("Score1", "0");
        Debug.Log("10");
        form.AddField("Score2", "0");
        Debug.Log("11");
        form.AddField("Bonus1", "3");
        Debug.Log("12");
        form.AddField("Bonus2", "1");

           
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

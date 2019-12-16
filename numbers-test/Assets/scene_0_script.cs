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

    public Text txtOutComic;
    public InputField txtNomeGiocatore;
    public Button btnInviaNome;

    private void Awake()
    {
        DatiGioco.user = new Users();
        if (!FB.IsInitialized)
        {
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            FB.ActivateApp();
            FB.API("me/picture?type=med", HttpMethod.GET, GetUserPhoto_Face);

        }
    }

    void GetUserPhoto_Face(IGraphResult result)
    {
        if (result.Error==null)
        {
            DatiGioco.user.UserProfileImage.GetComponent<Image>().sprite = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());

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
            // Continue with Facebook SDK
            CreateFacebookUser();
            // ...
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
        FB.API("me/picture?type=med", HttpMethod.GET, GetUserPhoto_Face);
        FB.API("me?fields=address,birthday,friends{name},name", HttpMethod.GET, GetUserData_Face);
    }

    private void GetUserData_Face(IGraphResult result)
    {
        Debug.Log("RES1:" + result.RawResult);
        Debug.Log("Res2:" + result.ResultDictionary.ToJson());
        Debug.Log("Res3:" + result.ResultDictionary.Count);
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
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }
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
        SceneManager.LoadScene("ScenaMenu");
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

    private IEnumerator SetUser(Users u, String platform)
    {
        bool state = false;
        string uuid = u.Uuid;
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
            form.AddField("Nickname", u.Nickname);
            form.AddField("Imei", u.Imei);
            form.AddField("Uuid", u.Uuid);
            form.AddField("Data_setup", DateTime.UtcNow.ToString());
            form.AddField("Email", u.Email);
            form.AddField("Service_id", u.Service_id);
            form.AddField("Note", platform);
            form.AddField("Score1", "0");
            form.AddField("Score2", "0");
            form.AddField("Bonus1", "0");
            form.AddField("Bonus2", "0");

           
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using UnityEngine.UI;

public class matchmaking_script : MonoBehaviour
{
    public Text txtGiocatoreTrovato;
    public Button btnAnnulla;
    public Button btnGioca;

    private bool _annulla = false;
    private bool _trovato = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(searchMatches());
    }

    public void btnAnnulla_click()
    {
        _annulla = true;
    }

    private IEnumerator searchMatches()
    {
        while (!_trovato || !_annulla)
        {

            string url = "http://numbers.jemaka.it/api/Versus?IdPlayer=" + DatiGioco.user.Id_user;
            UnityWebRequest request = UnityWebRequest.Get(url);
            if (_annulla)
                SceneManager.LoadScene("ScenaMenu");
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError("Request Error: " + request.error);
            }
            else
            {
                yield return new WaitForSeconds(1);
                string JsonText = request.downloadHandler.text;
                versus match = JsonConvert.DeserializeObject<versus>(JsonText);
               // DatiGioco.matchCorrente = match;
                if (match.IdPlayer2 > 0) /*Se esiste anche il player 2*/
                {
                    DatiGioco.matchCorrente = match;
                    if (match.IdPlayer1 != DatiGioco.user.Id_user)
                    {
                        StartCoroutine(GetPlayerData(match.IdPlayer1));
                       
                    } else
                    {
                        StartCoroutine( GetPlayerData(match.IdPlayer2));
                        
                    }
                    btnGioca.gameObject.SetActive(true);
                    _trovato = true;
                    
                    /*avviare la form di gioco*/



                }
               
               
            }
            yield return new WaitForSeconds(4);

        }
    }

    private IEnumerator GetPlayerData(int IDPlayer)
    {
        string url = "http://numbers.jemaka.it/api/Utenti/" + IDPlayer;
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SendWebRequest();
        //string JsonText = request.downloadHandler.text;
        yield return new WaitForSeconds(1);
        Users ut = JsonConvert.DeserializeObject<Users>(request.downloadHandler.text);
        txtGiocatoreTrovato.text = ut.Nickname;
        DatiGioco.lastUserVerus = ut;

        yield return ut;
    }

    public void onClickStartButton()
    {
        DatiGioco.isVersusStart = true;
        SceneManager.LoadScene("ScenaDownload");
    }



}

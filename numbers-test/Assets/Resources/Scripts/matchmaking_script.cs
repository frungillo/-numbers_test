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

            string url = "http://numbers.jemaka.it/api/Matchmaking?level=" + DatiGioco.user.Levels + "&currentPlayerID=" + DatiGioco.user.Id_user;
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
                Matchmaking match = JsonConvert.DeserializeObject<Matchmaking>(JsonText);
                DatiGioco.matchCorrente = match;
                if (match.Id_matchmaking > 0)
                {

                    txtGiocatoreTrovato.text = DatiGioco.matchCorrente.UsersData.Nickname;
                    btnGioca.gameObject.SetActive(true);
                    _trovato = true;
                    
                    /*avviare la form di gioco*/



                }
                else
                {
                    yield return new WaitForSeconds(4);
                    StartCoroutine(searchMatches());
                }

            }
        }
    }



}

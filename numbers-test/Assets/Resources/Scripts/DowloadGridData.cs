using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class DowloadGridData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Livello:" + DatiGioco.LivelloCorrente);
        string GridSelection = "";
        if(DatiGioco.LivelloCorrente > 0)
        {
            GridSelection = "?idGrid=" + DatiGioco.GrigliaDiGioco.Id_grid;
        } 
        StartCoroutine(GetGrids("http://numbers.jemaka.it/api/grids" + GridSelection));
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator GetGrids(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
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

                Grids gr = (Grids) JsonUtility.FromJson(JsonText, typeof(Grids) );
                DatiGioco.GrigliaDiGioco = gr;
                if (DatiGioco.LivelloCorrente == 0) DatiGioco.LivelloCorrente = 1;
                StartCoroutine(GetSolutions("http://numbers.jemaka.it/api/Soluzioni/" + gr.Id_grid.ToString()+"?liv="+DatiGioco.LivelloCorrente));

               
            }
        }
    }

    private IEnumerator GetSolutions(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
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
                Solutions[] sols = JsonConvert.DeserializeObject<Solutions[]>(JsonText);
                DatiGioco.soluzioni = sols;
                StartCoroutine(GetTimePlay(DatiGioco.LivelloCorrente));
                //MusicTemeScript.Instance.gameObject.GetComponent<AudioSource>().Stop();
                //SceneManager.LoadScene("ScenaDiGioco");
            }
        }
    }

    private IEnumerator GetTimePlay(int lvl)
    {
        string url = "http://numbers.jemaka.it/api/TimeLevel?level="+lvl;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
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
                int time = JsonConvert.DeserializeObject<int>(JsonText);
                DatiGioco.TempoPerLivelloCorrente = time;
                if (DatiGioco.TempoAvanzato > 0) { DatiGioco.TempoPerLivelloCorrente += DatiGioco.TempoAvanzato; DatiGioco.TempoAvanzato = 0; }
                MusicTemeScript.Instance.gameObject.GetComponent<AudioSource>().Stop();
                SceneManager.LoadScene("ScenaDiGioco");
            }
        }
    }
}

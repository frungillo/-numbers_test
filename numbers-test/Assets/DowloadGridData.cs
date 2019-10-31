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
        StartCoroutine(GetGrids("http://numbers.jemaka.it/api/grids"));
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
                
                string JsonText = request.downloadHandler.text;

                Grids gr = (Grids) JsonUtility.FromJson(JsonText, typeof(Grids) );
                DatiGioco.GrigliaDiGioco = gr;
                StartCoroutine(GetSolutions("http://numbers.jemaka.it/api/Soluzioni/" + gr.Id_grid.ToString()));

               
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
                string JsonText = request.downloadHandler.text;

                Solutions[] sols = JsonConvert.DeserializeObject<Solutions[]>(JsonText);
                DatiGioco.soluzioni = sols;
                SceneManager.LoadScene("ScenaDiGioco");
            }
        }
    }
}

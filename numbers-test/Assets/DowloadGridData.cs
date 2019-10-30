using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DowloadGridData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SendRequest("http://numbers.jemaka.it/api/grids"));
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator SendRequest(string url)
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
                DatiGioco.GrigliaDiGioco = JsonUtility.FromJson<Grids>(request.downloadHandler.text);
                SceneManager.LoadScene("ScenaDiGioco");
            }
        }
    }
}

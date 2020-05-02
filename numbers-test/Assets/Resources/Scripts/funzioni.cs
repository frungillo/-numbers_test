using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class funzioni : MonoBehaviour
{
    public static IEnumerator SetUser(Users u, String loadSceneName = "")
    {
        WWWForm form = new WWWForm();

        form.AddField("Id_user", u.Id_user);

        form.AddField("Nickname", u.Nickname);
        form.AddField("Imei", u.Imei);
        form.AddField("Uuid", u.Uuid);
        form.AddField("Data_setup", DateTime.UtcNow.ToString());
        form.AddField("Email", u.Email);
        form.AddField("Service_id", u.Service_id);
        form.AddField("Note", u.Note);
        form.AddField("Single_Score",  u.Single_score.ToString());
        form.AddField("Levels", u.Levels.ToString());
        form.AddField("Money", u.Money.ToString());
        form.AddField("Match_score", u.Match_score.ToString());

        string userJson = JsonConvert.SerializeObject(u);


        UnityWebRequest request = UnityWebRequest.Put("http://numbers.jemaka.it/api/Utenti",userJson );
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
           Debug.LogError("Request Error: " + request.error);
        }


        

        DatiGioco.user = u;
        if(loadSceneName!="")
            SceneManager.LoadScene(loadSceneName);
    }



}

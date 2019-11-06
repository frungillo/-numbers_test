using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(att());
        
    }

    IEnumerator att()
    {
        
        yield return new WaitForSeconds(1);

        
        if (DatiGioco.GrigliaDiGioco.Difficulty == DatiGioco.LivelloCorrente)
            DatiGioco.LivelloCorrente = 0;
        else
            DatiGioco.LivelloCorrente++;
       // Debug.Log("Livello_fine:" + DatiGioco.LivelloCorrente);
        SceneManager.LoadScene("ScenaDownload");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

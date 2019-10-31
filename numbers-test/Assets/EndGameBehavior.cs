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
        
        yield return new WaitForSeconds(9);
        
        SceneManager.LoadScene("ScenaDownload");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

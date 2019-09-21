using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comp_Esagono : MonoBehaviour
{
    private Animator anim;
    private AudioSource aus;
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        aus = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
       


    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButton(0))
        {
            Debug.Log("Tasto Schiacciato");
            anim.SetBool("cliccato", true);
            if (!aus.isPlaying) aus.Play();
        }
       
    }
    private void OnMouseExit()
    {
        Debug.Log("Tasto NON Schiacciato");
        anim.SetBool("cliccato", false);
    }



}

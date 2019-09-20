using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comp_Esagono : MonoBehaviour
{
    private Animator anim;
    public bool selezionato;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
      if( Input.GetMouseButton(0) && selezionato)
        {
            Debug.Log("Tasto Schiacciato");
            anim.SetBool("cliccato", true);
        } else
        {
            Debug.Log("Tasto NON Schiacciato");
            anim.SetBool("cliccato", false);
        }
    }

    
}

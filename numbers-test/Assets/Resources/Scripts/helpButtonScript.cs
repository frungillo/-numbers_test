using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helpButtonScript : MonoBehaviour
{
    [Header("MenuHelp")]
    public GameObject HelpMenu;

    private bool statePressed = false;
    
    private void Awake()
    {
        
        HelpMenu.SetActive(false);
    }

    // Start is called before the first frame update
    private void OnMouseDown()
    {

        if (!statePressed)
        {
            Animator h_anim = HelpMenu.GetComponent<Animator>();
            HelpMenu.SetActive(true);
            h_anim.SetTrigger("fire");
            statePressed = true;
        } else
        {
            statePressed = false;
            HelpMenu.SetActive(false);
        }

    }
     
}

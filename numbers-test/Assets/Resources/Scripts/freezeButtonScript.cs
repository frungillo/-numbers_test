using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class freezeButtonScript : MonoBehaviour
{
    public GameObject HelpMenu;

    // Start is called before the first frame update
    private void OnMouseDown()
    {
        /*Qui va fatto subito il check dei gettoni disponibili*/
        if (DatiGioco.user.Money <= 0)
        {
            /*comunicare all'utente che non ci sono monete disponibili*/
            HelpMenu.SetActive(false);
            return;
        }
        DatiGioco.user.Money -= 1;
        DatiGioco.FreezeTime = true;
        
        HelpMenu.SetActive(false);

    }
}

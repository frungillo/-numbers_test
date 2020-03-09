using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class freezeButtonScript : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnMouseDown()
    {
        /*Qui va fatto subito il check dei gettoni disponibili*/

        DatiGioco.FreezeTime = true;

    }
}

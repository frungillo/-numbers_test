using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class suggestButtonScript : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnMouseDown()
    {
        /**/
        Solutions[] s = DatiGioco.soluzioni;
        Solutions solToSuggetst = null;
        DatiGioco.PercorsoSoluzioneDaSuggerire = new List<string>();

        foreach(Solutions itm in s)
        {
            if (!DatiGioco.SoluzioniTrovate.Contains(itm))
            {
                solToSuggetst = itm;
                break;
            }
        }

        string[] tmp = solToSuggetst.Sequence.Split(new string[] { ";" }, System.StringSplitOptions.RemoveEmptyEntries);
        
        StartCoroutine(MostraTile(tmp));

        DatiGioco.PercorsoSoluzioneDaSuggerire.AddRange(tmp);

    }

   

    IEnumerator MostraTile(string[] tmp)
    {
        foreach (string item in tmp)
        {
            yield return new WaitForSeconds(0.2F);

            GameObject tile = GameObject.Find(item);
            int num = tile.GetComponent<Comp_Esagono>().Number;
            if (tile.tag == "op")
            {
                tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/operand/" + num.ToString() + "_v");
            }
            else
            {
                tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Exs_Numbers/" + num.ToString() + "_v");
            }
        }

    }
}

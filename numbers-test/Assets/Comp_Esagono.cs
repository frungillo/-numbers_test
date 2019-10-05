using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comp_Esagono : MonoBehaviour
{
    private Animator anim;
    private AudioSource aus;
    private SpriteRenderer spr;
    public bool Selected;
    public int Number;
    GameManagerScript _manger;
    
    // Start is called before the first frame update
    void Start()
    {
        _manger = GameManagerScript.Instance;
        
        anim = GetComponent<Animator>();
        aus = GetComponent<AudioSource>();
        spr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseOver()
    {

        if (Input.GetMouseButton(0))
        {
            //Debug.Log("Tasto Schiacciato");
            anim.SetBool("cliccato", true);
            if (!aus.isPlaying && !Selected)
            {
                if(_manger.esagoniSelezionati.Count == 0 && this.tag == "op") { _manger.txtParziale.text = "Prima un numero"; return; }
                if(_manger.esagoniSelezionati.Count >0 )
                {
                    int idx_lastSel = _manger.esagoniSelezionati.Count - 1;
                    if(_manger.esagoniSelezionati[idx_lastSel].tag == this.tag)
                    {
                        //PlayerPrefs.SetString("tots", "Troppi numeri!");
                        _manger.txtParziale.text = "Troppi numeri!";
                        _manger.inError = true;
                        return;
                    }
                }
                if (_manger.inError) return;
                Selected = true;
                aus.Play();
                /*
                string toAdd = "";
                if (this.tag == "op")
                {
                    switch (Number)
                    {
                        case 1:
                            toAdd = "*";
                            break;
                        case 2:
                            toAdd = "/";
                            break;
                        case 3:
                            toAdd = "-";
                            break;
                        case 4:
                            toAdd = "+";
                            break;
                        default:
                            break;
                    }
                } */
                //else { toAdd = Number.ToString(); }
                //string tots = PlayerPrefs.GetString("tots");
                //tots = tots + toAdd;
                //PlayerPrefs.SetString("tots", tots);
                //PlayerPrefs.Save();
                if (this.tag != "op") spr.sprite = Resources.Load<Sprite>("Sprites/Exs_Numbers/" + this.Number + "_v");

                _manger.esagoniSelezionati.Add(this.gameObject);
            }
        }
         
       
    }
    private void OnMouseExit()
    {
        //Debug.Log("Tasto NON Schiacciato");
        anim.SetBool("cliccato", false);
    }

  

    private void OnMouseDown()
    {
        PlayerPrefs.SetString("Stato", "G");
    }

    private void OnMouseUp()
    {
        PlayerPrefs.SetString("Stato", "S");
    }


}

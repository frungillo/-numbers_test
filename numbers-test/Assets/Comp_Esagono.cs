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
            //Debug.Log("Cliccato:"+this.name.ToString());//
            if (!aus.isPlaying && !Selected)
            {
                if(_manger.esagoniSelezionati.Count == 0 && this.tag == "op") { _manger.txtParziale.text = "Prima un numero"; return; }
                if(_manger.esagoniSelezionati.Count >0 )
                {
                    int idx_lastSel = _manger.esagoniSelezionati.Count - 1; //indice utimo selezionato
                    string nomeLastSelected = _manger.esagoniSelezionati[idx_lastSel].name.ToString(); // nome ultimo selezionato
                    nomeLastSelected = nomeLastSelected.Remove(0, 1); //il nome è in formato tx_y, ora rimuovo la "t"
                    string[] arrNomeLastSelected = nomeLastSelected.Split(new string[] { "_" }, System.StringSplitOptions.RemoveEmptyEntries); //divido per "_"
                    int last_i = int.Parse(arrNomeLastSelected[0]);
                    int last_j = int.Parse(arrNomeLastSelected[1]);
                   // Debug.Log($"Last_IDX:{last_i}_{last_j}");

                    string nomeCorrente = this.name.ToString().Remove(0, 1); //il nome è in formato tx_y, ora rimuovo la "t"
                    string[] arrNomeCorrente = nomeCorrente.Split(new string[] { "_" }, System.StringSplitOptions.RemoveEmptyEntries); //divido per "_"
                    int corrente_i = int.Parse(arrNomeCorrente[0]);
                    int corrente_j = int.Parse(arrNomeCorrente[1]);
                   // Debug.Log($"Curr_IDX:{corrente_i}_{corrente_j}");

                    //Debug.Log($"Condizione::{(last_i - corrente_i > 1 && last_j - corrente_j > 1).ToString()}");
                    if (_manger.esagoniSelezionati[idx_lastSel].tag == this.tag)
                    {
                        //PlayerPrefs.SetString("tots", "Troppi numeri!");
                        _manger.txtParziale.text = "Troppi numeri!";
                        //_manger.inError = true;
                        return;
                    }

                    if ((last_i-corrente_i)*-1 > 1 || (last_j-corrente_j)*-1 > 1)
                    {
                        //_manger.txtParziale.text = "Troppi numeri!";
                        //_manger.inError = true;
                        Debug.Log("Troppi numeri");
                        return;
                    }
                    
                    
                    
                }
                if (_manger.inError) return;
                Selected = true;
                aus.Play();

                if (this.tag != "op") {
                    spr.sprite = Resources.Load<Sprite>("Sprites/Exs_Numbers/" + this.Number + "_v");
                } else {
                    spr.color = new Color(20, 231, 0, 255);
                }

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

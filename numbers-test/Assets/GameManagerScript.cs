﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class GameManagerScript : MonoBehaviour
{
    public GameObject exs;
    public GameObject ops;
    public Text txtParziale;
    public Text txtPunteggio;
    public Text txtObiettivo;
    public Text txtPuntiTotali;
    public static int MAX_PUNTEGGIO = 100;
    private int obiettivo;

    public Animator txtAnimator;
    public bool inError;
    //public ParticleSystem ps;
    ServizioNumbers srv;
    public List<GameObject> esagoniSelezionati;
    
    List<GameObject> esagoniInGriglia;
    private static GameManagerScript _instance;
    //private Vector3 _mousePos;
    //private bool _isMousePressed = false;

    public static GameManagerScript Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else
        {
            _instance = this;
        }
        PlayerPrefs.DeleteAll();
        esagoniInGriglia = new List<GameObject>();
        esagoniSelezionati = new List<GameObject>();
        inError = false;

    }

    // Start is called before the first frame update
    void Start()
    {


        srv = new ServizioNumbers();
        Grids g = srv.getGrid();
        Debug.Log("GRIGLIA:" + g.Item);
        txtParziale.text = "Griglia #" + g.Id_grid.ToString();// +"\r\n";

        string[] arrGridTmp = g.Item.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

        Solutions[] sol = srv.getSolutionsbyGrid(g.Id_grid);

        foreach (Solutions item in sol)
        {
          //  txtParziale.text += item.Number.ToString() + "->" +item.Difficulty.ToString()+";";

        }
        int idxArrayGrid = 0;
        for (int r = 0; r < 7; r++)
        {
            for (int c = 0; c < 5; c++)
            {
                GameObject octagono = GameObject.Find($"t{r}_{c}");
                SpriteRenderer spr = octagono.GetComponent<SpriteRenderer>();
                Comp_Esagono script_exs = octagono.GetComponent<Comp_Esagono>();
                if (octagono.tag != "op")
                {
                    spr.sprite = Resources.Load<Sprite>("Sprites/Exs_Numbers/" + arrGridTmp[idxArrayGrid] + "_g");//GOAL!
                    script_exs.Number = int.Parse(arrGridTmp[idxArrayGrid]);

                } else
                {
                    spr.sprite = Resources.Load<Sprite>("Sprites/operand/" + DecodeOperands(arrGridTmp[idxArrayGrid]));//GOAL!
                    script_exs.Number = int.Parse(DecodeOperands(arrGridTmp[idxArrayGrid]));
                }
                idxArrayGrid++;
                esagoniInGriglia.Add(octagono);
            }
            
        }
        
        /**/
       

    }
    
    
    private void valuta(bool endSequence=false)
    {
        try
        {
            double ev = Eval(txtParziale.text);
            if (!endSequence)
            {
                txtPunteggio.text = ev.ToString("#.##");
                PlayerPrefs.SetString("tots", ev.ToString("#.##"));
            }
            txtAnimator.SetTrigger("fire");
            /*Cambio Esagoni*/
        }
        catch (System.Exception ex)
        {
            
            txtPunteggio.text = ex.Message;
            txtAnimator.SetTrigger("fire");
        }
        
    }

    private string DecodeOperands(string simbol)
    {
        string toAdd="";
        switch (simbol)
        {
            case "*":
                toAdd = "1";
                break;
            case "/":
                toAdd = "2";
                break;
            case "-":
                toAdd = "3";
                break;
            case "+":
                toAdd = "4";
                break;
        }
        return toAdd;
    }

    private void Calcolo()
    {
        double tot=0;
        int cnt = 1;
        string operazione="";
        foreach (GameObject itm in esagoniSelezionati)
        {
            
            string toAdd = "";
            Comp_Esagono src = itm.GetComponent<Comp_Esagono>();
            if (itm.tag == "op") //è un box operando
            {
                switch (src.Number)
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
            } else { //è un box numerico
                toAdd = src.Number.ToString();
                itm.transform.position = new Vector3(itm.transform.position.x, itm.transform.position.y, -1);
            }

            if (cnt < 3) { operazione += toAdd; goto exit; }

            if (cnt==3) {
                operazione += toAdd;
                tot = Eval(operazione);
                operazione = tot.ToString();
            }

            if (cnt > 3)
            {
               
                if (itm.tag == "op")
                {
                    operazione +=  toAdd;
                    Debug.Log($"Aggiornamento: { operazione}");
                    goto exit;
                }
                operazione +=  toAdd;
                tot = Eval(operazione);
                operazione = tot.ToString("#.##");
            }

        exit:

            txtParziale.text = tot.ToString("#.##");
            cnt++;
        }
        txtPunteggio.text = tot.ToString("#.##");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Esagoni in Griglia:" + esagoniInGriglia.Count.ToString());
        if (PlayerPrefs.GetString("Stato") == "G")
        {
            Calcolo();
        }

            if (PlayerPrefs.GetString("Stato") == "S") //Dito del mouse alzato
        {
            try
            {
                txtPuntiTotali.text = (double.Parse(txtPuntiTotali.text) + double.Parse(calcolaPunteggio(txtPunteggio.text, esagoniSelezionati.Count, obiettivo))).ToString();
            }
            catch { }
            
            inError = false;
            PlayerPrefs.DeleteAll();
           // txtParziale.text = "";
            foreach (GameObject itm in esagoniInGriglia)
            {
                itm.transform.position = new Vector3(itm.transform.position.x, itm.transform.position.y, 1);
               
            }

            foreach (GameObject itm in esagoniSelezionati)
            {
                Comp_Esagono scr_e = itm.GetComponent<Comp_Esagono>();
                SpriteRenderer spr = itm.GetComponent<SpriteRenderer>();
                
                /*Disattivato il cambio di esagoni al rilascio del mouse*/
                /* if (scr_e.Selected)
                /*******************************************************/

                scr_e.Selected = false;

                //box_anim.Play("exs_idle");
                if (itm.tag != "op")
                {
                    spr.sprite = Resources.Load<Sprite>("Sprites/Exs_Numbers/" + scr_e.Number.ToString() + "_g");


                }

            }
            esagoniSelezionati.Clear();
        }


        

        //txtParziale.text = PlayerPrefs.GetString("tots");
    }


    IEnumerator myAttesa(GameObject itm)
    {
        Animator box_anim = itm.GetComponent<Animator>();
        if(itm.tag != "op") box_anim.Play("Exagon_destroy"); else box_anim.Play("operand_destroy");
        float cliplen = box_anim.runtimeAnimatorController.animationClips.First(clip => clip.name.Contains("_destroy")).length;
        yield return new  WaitForSeconds(cliplen);
        esagoniInGriglia.Remove(itm);
        GameObject.Destroy(itm);

        /*Generazione Nuovo Item*/
        SpriteRenderer spr = exs.GetComponent<SpriteRenderer>();
        System.Random rnd = new System.Random();
        SpriteRenderer spr_op = ops.GetComponent<SpriteRenderer>();
        System.Random rnd_op = new System.Random();

        Comp_Esagono script_exs = exs.GetComponent<Comp_Esagono>();
        Comp_Esagono script_ops = ops.GetComponent<Comp_Esagono>();

        if (itm.tag != "op")
        {
            int num = rnd.Next(0, 10);
            spr.sprite = Resources.Load<Sprite>("Sprites/Exs_Numbers/" + num.ToString() + "_g");//GOAL!
            script_exs.Number = num;
            esagoniInGriglia.Add(Instantiate(
                   exs,
                   itm.transform.position,
                   Quaternion.identity
                   )
               );
        }
        else
        {

            int num = rnd_op.Next(1, 5);
            spr_op.sprite = Resources.Load<Sprite>("Sprites/operand/" + num.ToString());//GOAL!
            script_ops.Number = num;
            esagoniInGriglia.Add(Instantiate(
                    ops,
                    itm.transform.position,
                    Quaternion.identity
                    )
              );
        }

    }
    
    private double Eval(string expression)  
    {
        expression = expression.Replace(",", ".");
        System.Data.DataTable table = new System.Data.DataTable();
        try
        {
            table.Columns.Add("expression", string.Empty.GetType(), expression);
            System.Data.DataRow row = table.NewRow();
            table.Rows.Add(row);
            return double.Parse((string)row["expression"]);
        }
        catch (System.Exception ex)
        {

            throw new System.Exception(ex.Message + "-->" + expression); //("????");
        }
        
    }


    private string calcolaPunteggio(string risultatoOperazione, int passaggi, int obiettivo, int tolleranza =2)
    {
        risultatoOperazione = risultatoOperazione.Replace(",", ".");
        double TestOp = 0;
        if (!double.TryParse(risultatoOperazione, out TestOp )) { return "Hey!"; }
        if (obiettivo - double.Parse(risultatoOperazione) == 0)
            return "100";
        if (System.Math.Abs(obiettivo - double.Parse(risultatoOperazione)) < tolleranza)
        {
            try
            {
                double risop = double.Parse(risultatoOperazione);
                double D = (MAX_PUNTEGGIO * 20) / 100;
                double punteggioOttenuto = ((obiettivo - risop) * (D - MAX_PUNTEGGIO) / tolleranza)
                    + (MAX_PUNTEGGIO - D) + (D * (passaggi - 1) / 34);
                if (punteggioOttenuto < 0) punteggioOttenuto = 0;
                return punteggioOttenuto.ToString("#.##");
            }
            catch (System.Exception ex)
            {
                return "ERRORE!";
            }
        } else { return "0"; }

    }
  
}

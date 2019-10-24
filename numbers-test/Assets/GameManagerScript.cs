using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class GameManagerScript : MonoBehaviour
{
    public Text txtParziale;
    public Text txtPunteggio;
    public Text txtTimer;
    
    public List<Text> GoalsTexts;
    public static int MAX_PUNTEGGIO = 100;
   // private int obiettivo;
    float timeleft = 120;
    private string numeroTrovatoDalGiocatore;

    public Animator txtAnimator;
    public bool inError;
    ServizioNumbers srv;
    public List<GameObject> esagoniSelezionati;
    private Time t;
    List<GameObject> esagoniInGriglia;
    Solutions[] soluzioniGriglia;
    private static GameManagerScript _instance;
    
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
        
        txtPunteggio.text = "0";
        srv = new ServizioNumbers();
        Grids g = srv.getGrid();
        
        Debug.Log("GRIGLIA:" + g.Item);
        txtParziale.text = "Griglia #" + g.Id_grid.ToString();// +"\r\n";

        string[] arrGridTmp = g.Item.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

        Solutions[] sol = srv.getSolutionsbyGrid(g.Id_grid);
        soluzioniGriglia = sol;

        int idxTxts = 0;
        foreach (Solutions item in sol)
        {
            GoalsTexts[idxTxts].text = item.Number.ToString();
            GameObject boxSolution = GameObject.Find("Goal_" + idxTxts.ToString());
            GoalsTexts[idxTxts].name = "txtGoal_"+item.Id_solution.ToString();
            boxSolution.name = "Goal_" + item.Id_solution.ToString();
            idxTxts++;
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
    
    
    private void Valuta(bool endSequence=false)
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

    private string Calcolo()
    {
        double tot=0;
        int cnt = 1;
        string operazione="";
        txtParziale.text = "";
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
                txtParziale.text += toAdd;
            } else { //è un box numerico
                toAdd = src.Number.ToString();
                itm.transform.position = new Vector3(itm.transform.position.x, itm.transform.position.y, -1);
                txtParziale.text += toAdd;
            }
            
            if (cnt < 3) { operazione += toAdd; goto exit; }

            if (cnt==3) {
                operazione += toAdd;
                tot = Eval(operazione);
                operazione = tot.ToString();
                txtParziale.text += "="+operazione;
            }

            if (cnt > 3)
            {
               
                if (itm.tag == "op")
                {
                    operazione +=  toAdd;
                    txtParziale.text = operazione;
                   
                    goto exit;
                }
                operazione +=  toAdd;
                tot = Eval(operazione);
                operazione = tot.ToString("#.##");
                txtParziale.text +="="+operazione;
            }
            
        exit:
            
            //txtParziale.text = tot.ToString("#.##");
            cnt++;
        }
        // txtPunteggio.text = tot.ToString("#.##");
        return operazione;
    }

    // Update is called once per frame
    void Update()
    {
        Animator timerAnim = txtTimer.GetComponent<Animator>();
        timeleft -= Time.deltaTime;
        txtTimer.text = Math.Truncate((timeleft)).ToString();
        if (timeleft>20 )
        {
            timerAnim.SetTrigger("tick");
        }
        if (timeleft<20)
        {

            timerAnim.SetBool("warn", true);
        }
        //Debug.Log("Esagoni in Griglia:" + esagoniInGriglia.Count.ToString());
        if (PlayerPrefs.GetString("Stato") == "G")
        {
           numeroTrovatoDalGiocatore= Calcolo();
        }

            if (PlayerPrefs.GetString("Stato") == "S") //Dito del mouse alzato
        {
            try
            {
                txtPunteggio.text = (double.Parse(txtPunteggio.text) + double.Parse(calcolaPunteggio(numeroTrovatoDalGiocatore, esagoniSelezionati.Count,soluzioniGriglia))).ToString();
            }
            catch { }
            
            inError = false;
            PlayerPrefs.DeleteAll();
            txtParziale.text = "";

            foreach (GameObject itm in esagoniInGriglia)
            {
               // itm.transform.position = new Vector3(itm.transform.position.x, itm.transform.position.y, 1);
               
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
        if (itm.tag != "op") box_anim.Play("Exagon_destroy"); else box_anim.Play("operand_destroy");
        float cliplen = box_anim.runtimeAnimatorController.animationClips.First(clip => clip.name.Contains("_destroy")).length;
        yield return new WaitForSeconds(cliplen);
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


    private string calcolaPunteggio(string risultatoOperazione, int passaggi, Solutions[] soluzioni)
    {
        risultatoOperazione = risultatoOperazione.Replace(",", ".");
        int punteggioAssegnatoAlGiocatore = 0;
        double numeroTrovatoDalGiocatore;
        if (!double.TryParse(risultatoOperazione, out numeroTrovatoDalGiocatore )) { return "Hey!"; }
        Solutions SoluzioneTrovata = null;
        foreach (Solutions item in soluzioni)
        {
            if(item.Number == numeroTrovatoDalGiocatore)
            {
                SoluzioneTrovata = item;
                break;
            }
        }
        if (SoluzioneTrovata!=null)
        {
            punteggioAssegnatoAlGiocatore = passaggi * (int)SoluzioneTrovata.Difficulty;
            GameObject box = GameObject.Find("Goal_"+SoluzioneTrovata.Id_solution.ToString());
            SpriteRenderer spr = box.GetComponent<SpriteRenderer>();
            spr.sprite = Resources.Load<Sprite>("Sprites/boxes/box_v");
        }
        return punteggioAssegnatoAlGiocatore.ToString();
    }
  
}

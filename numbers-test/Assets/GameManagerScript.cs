using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        
        //GameObject obj = exs.GetComponentInChildren<GameObject>();
        SpriteRenderer spr = exs.GetComponent<SpriteRenderer>(); //(SpriteRenderer)exs.GetComponentsInChildren(typeof(SpriteRenderer))[1];
        System.Random rnd = new System.Random();
        SpriteRenderer spr_op = ops.GetComponent<SpriteRenderer>(); //(SpriteRenderer)exs.GetComponentsInChildren(typeof(SpriteRenderer))[1];
        System.Random rnd_op = new System.Random();
        
        Comp_Esagono script_exs = exs.GetComponent<Comp_Esagono>();
        Comp_Esagono script_ops = ops.GetComponent<Comp_Esagono>();
        //txtAnimator = txt.GetComponentInChildren<Animator>();

        System.Random rnd_obiettivo = new System.Random();
        obiettivo = rnd_obiettivo.Next(10, 90);
        txtObiettivo.text = obiettivo.ToString();
        txtPuntiTotali.text = "0";

        float y = (float)5; //3.5
        
        for (int row =8;row >1; row--)
        {
            float x = (float)-5; //-7.3
            float x_op = (float)-3.53; //-7.3

            int max_col = 4;
            int max_col_op = 3;
            if (row % 2 != 0) { max_col = 3; x = (float)-3.51; }
            if (row % 2 != 0) { max_col_op = 4; x_op = (float)-4.99; }
            for (int col = 1; col < max_col; col++)
            {
                int num = rnd.Next(0, 10);
                spr.sprite = Resources.Load<Sprite>("Sprites/Exs_Numbers/" + num.ToString() + "_g");//GOAL!
                script_exs.Number = num;
                esagoniInGriglia.Add(Instantiate(
                       exs,
                       new Vector3(x, y, 1),
                       Quaternion.identity
                       )
                   );
 
                 x += (float)3.0;
            }

            for (int col = 1; col < max_col_op; col++)
            {
                int num = rnd_op.Next(1, 5);
                spr_op.sprite = Resources.Load<Sprite>("Sprites/operand/" + num.ToString());//GOAL!
                script_ops.Number = num;
               esagoniInGriglia.Add( Instantiate(
                       ops,
                       new Vector3(x_op, y, 1),
                       Quaternion.identity
                       )
                 );

                x_op += (float)3.0;
            }

            y -= (float)1.5;
        }
        foreach(GameObject itm in esagoniInGriglia)
        {
            Debug.Log("ITM:" + itm.GetInstanceID().ToString());
        }
       

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

    private void Calcolo()
    {
        double tot=0;
        int cnt = 1;
        string operazione="";
        foreach (GameObject itm in esagoniSelezionati)
        {
            
            string toAdd = "";
            Comp_Esagono src = itm.GetComponent<Comp_Esagono>();
            if (itm.tag == "op")
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
            } else { toAdd = src.Number.ToString(); }

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
        if (PlayerPrefs.GetString("Stato") == "G")
        {
            /*
            _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _mousePos.z = 0;
            
            ps.transform.SetPositionAndRotation(_mousePos, Quaternion.identity);
            */
            
            /*
            if (esagoniSelezionati.Count == 3)
            {
                valuta();
                
            }
            if (esagoniSelezionati.Count > 3 && (esagoniSelezionati.Count-3) % 2 == 0)
            {
                valuta();
            }
            */
            Calcolo();
        }

            if (PlayerPrefs.GetString("Stato") == "S") //Dito del mouse alzato
        {
            /*
            try
            {
                double ev = Eval(txtParziale.text);
                txtPunteggio.text = ev.ToString("#.##");
                txtAnimator.SetTrigger("fire");
              
            }
            catch (System.Exception ex)
            {

                txtPunteggio.text = ex.Message;
                txtAnimator.SetTrigger("fire");
            }
            */

            txtPuntiTotali.text = calcolaPunteggio(txtPunteggio.text, esagoniSelezionati.Count, obiettivo);

            esagoniSelezionati.Clear();
            inError = false;
            PlayerPrefs.DeleteAll();
            txtParziale.text = "";
            foreach (GameObject itm in esagoniInGriglia)
            {
                Comp_Esagono scr_e = itm.GetComponent<Comp_Esagono>();
                SpriteRenderer spr = itm.GetComponent<SpriteRenderer>(); 
                scr_e.Selected = false;
                if (itm.tag != "op") {
                    spr.sprite = Resources.Load<Sprite>("Sprites/Exs_Numbers/" + scr_e.Number.ToString() + "_g");
                   

                }

            }
        }


        

        txtParziale.text = PlayerPrefs.GetString("tots");
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
        
        try
        {
            double risop = double.Parse(risultatoOperazione);
            double D = MAX_PUNTEGGIO - risop;
            double punteggioOttenuto = ((obiettivo - risop) * (D - MAX_PUNTEGGIO) / tolleranza)
                + (MAX_PUNTEGGIO - D) + (D * (passaggi - 1) / 34);
            if (punteggioOttenuto < 0) punteggioOttenuto = 0;
            return punteggioOttenuto.ToString("#.##");
        } catch(System.Exception ex)
        {
            return "ERRORE!";
        }

    }
  
}

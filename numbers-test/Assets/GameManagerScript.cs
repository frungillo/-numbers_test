using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    public GameObject exs;
    public GameObject ops;
    public Text txt;
    public Text txtPunteggio;
    
    public Animator txtAnimator;
    
    List<GameObject> esagoniSelezionati;
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
        esagoniSelezionati = new List<GameObject>();
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
                esagoniSelezionati.Add(Instantiate(
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
               esagoniSelezionati.Add( Instantiate(
                       ops,
                       new Vector3(x_op, y, 1),
                       Quaternion.identity
                       )
                 );

                x_op += (float)3.0;
            }

            y -= (float)1.5;
        }
       

    }


    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetString("Stato") == "S") //Dito del mouse alzato
        {
            try
            {
                double ev = Eval(txt.text);
                txtPunteggio.text = ev.ToString();
                txtAnimator.SetTrigger("fire");
            }
            catch (System.Exception ex)
            {

                txtPunteggio.text = ex.Message;
                txtAnimator.SetTrigger("fire");
            }
          

            PlayerPrefs.DeleteAll();
            txt.text = "";
            foreach (GameObject itm in esagoniSelezionati)
            {
                Comp_Esagono scr_e = itm.GetComponent<Comp_Esagono>();
                SpriteRenderer spr = itm.GetComponent<SpriteRenderer>();
                scr_e.Selected = false;
                if (itm.tag != "op") spr.sprite = Resources.Load<Sprite>("Sprites/Exs_Numbers/" + scr_e.Number.ToString() + "_g");
            }
        }
        
                   
        
        
        txt.text = PlayerPrefs.GetString("tots");
    }

    private double Eval(string expression)  
    {
        System.Data.DataTable table = new System.Data.DataTable();
        try
        {
            table.Columns.Add("expression", string.Empty.GetType(), expression);
            System.Data.DataRow row = table.NewRow();
            table.Rows.Add(row);
            return double.Parse((string)row["expression"]);
        }
        catch (System.Exception)
        {

            throw new System.Exception("????");
        }
        
    }

}

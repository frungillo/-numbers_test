using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using UnityEngine.Advertisements;

public class ManagerSinglePlayer : MonoBehaviour
{
    
    [Tooltip("Testo punteggio parziale")]
    public Text txtParziale;

    [Tooltip("Punteggio totale")]
    public Text txtPunteggio;

    public Text txtCoins;

    [Tooltip("Timer")]
    public Text txtTimer;

    [Tooltip("Livello")]
    public Text txtLevel;

    [Header("Campi Soluzioni")]
    public List<Text> GoalsTexts;

    [Header("Lista Effetti Sonori")]
    public List<AudioClip> EffettiSonori;

    [Header("Canvas nascoste")]
    public GameObject CanvasGameOver;
    public GameObject CanvasLevelWin;

    

    /// <summary>
    /// Griglia di gioco
    /// </summary>
    public Grids griglia;

    private static int BASE_POINTS = 2;

    private int BONUS_X=1;
    private bool userUpdated = false;
    
   
    float timeleft = DatiGioco.TempoPerLivelloCorrente;
    private string numeroTrovatoDalGiocatore;

    /*Componenti Fumetto*/
    public GameObject Comic;
    private Animator ComicAnimator;
    private SpriteRenderer ComicSprite;

    public bool inError;
    UnityWebRequest srv;
    public List<GameObject> esagoniSelezionati;
    //private Time t;
    List<GameObject> esagoniInGriglia;
    Solutions[] soluzioniGriglia;
    List<Solutions> soluzioniTrovate;

    private bool pointAdded = false; //flag aggiunta punti
    private bool levelWin = false; //flag  di controllo vincita livello
    private bool overTime = false; //Flag di gestione fine tempo

    AudioSource audio_s;

    private GameObject bonusSpc;
    

    private static ManagerSinglePlayer _instance;
    
    public static ManagerSinglePlayer Instance { get { return _instance; } }

    /*ADV*/
    string gameId = "3651885";
    bool testMode = false;


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
        griglia = DatiGioco.GrigliaDiGioco;
        DatiGioco.PercorsoSoluzioneDaSuggerire = new List<string>();
        txtCoins.text = DatiGioco.user.Money.ToString();

        


    }


    // Start is called before the first frame update
    void Start()
    {
        Advertisement.Initialize(gameId, testMode);
        CanvasGameOver.SetActive(false);
        CanvasLevelWin.SetActive(false);

        DatiGioco.SoluzioniTrovate = new List<Solutions>();
        DatiGioco.FreezeTime = false;

        /*Elemento Bonus_spc*/
        bonusSpc = GameObject.Find("bonus_spc");
        SpriteRenderer spr_bonus = bonusSpc.GetComponent<SpriteRenderer>();
        spr_bonus.sprite = null; //azzero lo sprite
        /* ********************* */

        /*inizializzazione fumetto*/
        ComicAnimator = Comic.GetComponent<Animator>();
        ComicSprite = Comic.GetComponent<SpriteRenderer>();
        ComicSprite.sprite = null;
        /***/

        soluzioniTrovate = new List<Solutions>();

        audio_s = GetComponent<AudioSource>();

        System.Random rmt = new System.Random();
        int temeMusic = rmt.Next(5, 5);

        audio_s.clip = EffettiSonori[temeMusic];
        audio_s.loop = true;
        audio_s.Play();


        txtPunteggio.text = "0";
        if (DatiGioco.PuntiGiocatore > 0) txtPunteggio.text = DatiGioco.PuntiGiocatore.ToString();
        ScriviParziale("", false); // txtParziale.text = "";
        Grids g = griglia;

        ScriviParziale("Griglia #" + g.Id_grid.ToString()); //  txtParziale.text = "Griglia #" + g.Id_grid.ToString();

        string[] arrGridTmp = g.Item.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

        Solutions[] sol = DatiGioco.soluzioni; // g.Soluzioni.ToArray();
        soluzioniGriglia = sol;
        txtLevel.text = DatiGioco.LivelloCorrente.ToString();
        /*
        for (int l = 1; l < griglia.Difficulty + 1; l++)
        {
            GameObject levels = GameObject.Find("lvl_" + l);
            SpriteRenderer lvlspr = levels.GetComponent<SpriteRenderer>();
            if (DatiGioco.LivelloCorrente == l)
            {
                lvlspr.sprite = Resources.Load<Sprite>("Sprites/liv_selector/SVG/Verde/Verde " + l);
            }
            else
            {
                lvlspr.sprite = Resources.Load<Sprite>("Sprites/liv_selector/SVG/Giallo/Giallo " + l);
            }
        }
        */

        int idxTxts = 0;
        foreach (Solutions item in sol)
        {
            GoalsTexts[idxTxts].text = item.Number.ToString();
            GameObject boxSolution = GameObject.Find("Goal_" + idxTxts.ToString());
            GoalsTexts[idxTxts].name = "txtGoal_"+item.Id_solution.ToString();
            boxSolution.name = "Goal_" + item.Id_solution.ToString();
            idxTxts++;
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
           // ComicAnimator.SetTrigger("fire");
            /*Cambio Esagoni*/
        }
        catch (System.Exception ex)
        {
            
            txtPunteggio.text = ex.Message;
            //ComicAnimator.SetTrigger("fire");
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

    private bool FumettoVisibile = false;
    private void ScriviParziale(string msg, bool add = true)
    {
        if (msg != "")
        {
            if (!FumettoVisibile)
            {
                ComicSprite.sprite = Resources.Load<Sprite>("Sprites/albert/Fumetto-vuoto");
                ComicAnimator.SetTrigger("fire");
                FumettoVisibile = true;
            }
            

        } else
        {
            //
            
        }
        if (!add) txtParziale.text = msg; else txtParziale.text += msg;
    }

    private string Calcolo()
    {
        double tot=0;
        int cnt = 1;
        string operazione="";
        ScriviParziale("", false);
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
                 ScriviParziale(toAdd); //txtParziale.text += toAdd;
            } else { //è un box numerico
                toAdd = src.Number.ToString();
                itm.transform.position = new Vector3(itm.transform.position.x, itm.transform.position.y, -1);
                ScriviParziale(toAdd); // txtParziale.text += toAdd;
            }
            
            if (cnt < 3) { operazione += toAdd; goto exit; }

            if (cnt==3) {
                operazione += toAdd;
                tot = Eval(operazione);
                operazione = tot.ToString();
                ScriviParziale("=" + operazione); // txtParziale.text += "="+operazione;
            }

            if (cnt > 3)
            {
               
                if (itm.tag == "op")
                {
                    operazione +=  toAdd;
                    ScriviParziale(operazione, false); // txtParziale.text = operazione;
                   
                    goto exit;
                }
                operazione +=  toAdd;
                tot = Eval(operazione);
                operazione = tot.ToString("#.##");
                ScriviParziale("=" + operazione); // txtParziale.text +="="+operazione;
            }
            
        exit:
            
            //txtParziale.text = tot.ToString("#.##");
            cnt++;
        }
        // txtPunteggio.text = tot.ToString("#.##");
        return operazione;
    }



    #region Comportamento Bottoni Scena
    public void btnAbbandonaClick()
    {
        DatiGioco.LivelloCorrente = 0;
        SceneManager.LoadScene("ScenaMenu");
    }



    #endregion


    private bool inWarningTime = false;
    private float FreezeTime = 16;
    private bool startFreeze = true;
    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Time:" + Time.time);
        if (DatiGioco.FreezeTime)
        {
            Debug.Log("FreetTime:" + FreezeTime.ToString());
            
            Time.timeScale = 0.5f;
            FreezeTime -= Time.deltaTime;
            if (startFreeze)
            {
                audio_s.Stop();
                audio_s.PlayOneShot(EffettiSonori[6]);
                startFreeze = false;

            }
            if (Mathf.FloorToInt(FreezeTime) <= 0 )
            {
                DatiGioco.FreezeTime = false;
                startFreeze = true;
                FreezeTime = 30;
                audio_s.Play();
                Time.timeScale = 1f;
            }

        }

        /*Update del numero di coins disponibili*/
        txtCoins.text = DatiGioco.user.Money.ToString();

        GameObject timerGraph = GameObject.Find("timer");
        SpriteRenderer timerSpr = timerGraph.GetComponent<SpriteRenderer>();
        
        Animator timerAnim = txtTimer.GetComponent<Animator>();

        if (!overTime && !levelWin) { /*Si non si è superato il tempo limite e se non hai vinto il livello*/
            timeleft -= Time.deltaTime; // decremento il contatore del tempo della porzione di tempo trascorsa nel frame
            txtTimer.text = Mathf.FloorToInt(timeleft).ToString(); //passo solo la parte intera al campo testo
        }
        
        if(soluzioniTrovate.Count == 5 && !levelWin) //se sono state trovate le 5 soluzioni e il livello non è stato dichiarato vinto ancora.
        {

            audio_s.PlayOneShot(EffettiSonori[3], 1F);
            levelWin = true;
            StartCoroutine(GameWin_async());

            if (!pointAdded)
            {
                int TempoDaAggiungere = Mathf.FloorToInt(timeleft);
                if (TempoDaAggiungere > 10)
                    DatiGioco.TempoAvanzato = 10 + DatiGioco.LivelloCorrente;
                else
                    DatiGioco.TempoAvanzato = TempoDaAggiungere + DatiGioco.LivelloCorrente;
                DatiGioco.user.Match_score += TempoDaAggiungere;
                DatiGioco.user.Levels = DatiGioco.LivelloCorrente;
                if (!userUpdated)
                {
                    DatiGioco.user.Single_score += DatiGioco.PuntiGiocatore;
                    if (DatiGioco.LivelloCorrente > DatiGioco.user.Levels) DatiGioco.user.Levels = DatiGioco.LivelloCorrente;
                    StartCoroutine(funzioni.SetUser(DatiGioco.user)); /*Salvo lo stato del giocatore sul server.*/
                    userUpdated = true;
                }
                
                
            }
            pointAdded = true;
            DatiGioco.FreezeTime = false;
            startFreeze = true;
            FreezeTime = 30;
            audio_s.Play();
            Time.timeScale = 1f;
            return;

        }


        int spriteno=  ((Mathf.FloorToInt(timeleft) * 10) / (int)DatiGioco.TempoPerLivelloCorrente);
       
        Debug.Log("Sprite num:" + ((Mathf.FloorToInt(timeleft) * 10) / (int)DatiGioco.TempoPerLivelloCorrente));
        if (Mathf.FloorToInt(timeleft) > 0)
        {
            if (spriteno == 0) spriteno = 1;
            timerSpr.sprite = Resources.Load<Sprite>("Sprites/TimerAnim/t_" + spriteno.ToString());
        }
        else timerSpr.sprite = Resources.Load<Sprite>("Sprites/TimerAnim/t_0"); 
        if (Mathf.FloorToInt(timeleft) == 0)
            txtTimer.text = "";
       

        if (timeleft>20 ) //effetto rimbalzo del tempo
        {
            timerAnim.SetTrigger("tick");
        }

        if (timeleft<10)
        {
            if (!overTime)
            {
                timerAnim.SetBool("warn", true);
                if (!inWarningTime)
                {
                    inWarningTime = true;
                    audio_s.PlayOneShot(EffettiSonori[0], 1F);
                }
            }
        }

        if(timeleft <= 0 && !levelWin)
        {
            txtTimer.text = "";
            overTime = true;
            DatiGioco.FreezeTime = false;
            startFreeze = true;
            FreezeTime = 30;
            audio_s.Play();
            Time.timeScale = 1f;
            CanvasGameOver.SetActive(true);
            if (!userUpdated)
            {
                DatiGioco.user.Single_score += DatiGioco.PuntiGiocatore;
                if (DatiGioco.LivelloCorrente > DatiGioco.user.Levels) DatiGioco.user.Levels = DatiGioco.LivelloCorrente;
                StartCoroutine(funzioni.SetUser(DatiGioco.user)); /*Salvo lo stato del giocatore sul server.*/
                userUpdated = true;
            }
            return;
            
        }
        
        /*Soluzione suggerita*/
        if (DatiGioco.PercorsoSoluzioneDaSuggerire.Count > 1 && !MostraTilesBusy)
        {
            MostraTilesBusy = true;
            StartCoroutine(MostraTile(DatiGioco.PercorsoSoluzioneDaSuggerire.ToArray()));
        }
        /*********************/
        
        /*****************Valutazione in caso di stato SCHIACCIATO del dito giocatore*****************/
        if (PlayerPrefs.GetString("Stato") == "G") /*dito sullo schermo*/
        {
           numeroTrovatoDalGiocatore= Calcolo();
            StopAllCoroutines();
            
        }
        /*********************************************************************************************/

        /***************Valutazione in caso di stato ALTAZO del dito del giocatore********************/
        if (PlayerPrefs.GetString("Stato") == "S") //Dito alzato
        {

            ComicSprite.sprite = null;
            FumettoVisibile = false;

            try
            {
                DatiGioco.PuntiGiocatore += (int)double.Parse(CalcolaPunteggio(numeroTrovatoDalGiocatore, soluzioniGriglia));
                txtPunteggio.text = DatiGioco.PuntiGiocatore.ToString();
                
                
            }
            catch (Exception ex) 
            {
                Debug.Log("Erro:" + ex.Message);
                /*
                inError = false;
                PlayerPrefs.DeleteAll();
                ScriviParziale("", false);
                */
            }
            
            inError = false;
            PlayerPrefs.DeleteAll();
            ScriviParziale("", false); //txtParziale.text = "";

            foreach (GameObject itm in esagoniInGriglia)
            {
               /*Tutti gli esagoni*/
               
            }
            

           
        }
        /***********************************************************************************************/

        

        
    }

    private bool MostraTilesBusy = false; /*flag di controllo per non ripetere l'animazione del suggerimento prima della sua fine.*/
    IEnumerator MostraTile(string[] tmp)
    {
        foreach (string item in tmp)
        {
            yield return new WaitForSeconds(0.1F);

            GameObject tile = GameObject.Find(item);
            int num = tile.GetComponent<Comp_Esagono>().Number;
            bool sel = tile.GetComponent<Comp_Esagono>().Selected;
            if (tile.tag == "op")
            {
                if (!sel) tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/operand/" + num.ToString());
            }
            else
            {
                if (!sel) tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Exs_Numbers/" + num.ToString() + "_g");
            }
        }

        foreach (string item in tmp)
        {
            yield return new WaitForSeconds(0.2F);

            GameObject tile = GameObject.Find(item);
            int num = tile.GetComponent<Comp_Esagono>().Number;
            bool sel = tile.GetComponent<Comp_Esagono>().Selected;
            if (tile.tag == "op")
            {
                if (!sel) tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/operand/" + num.ToString() + "_v");
            }
            else
            {
                if (!sel) tile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Exs_Numbers/" + num.ToString() + "_v");
            }
        }
        MostraTilesBusy = false;
    }

    IEnumerator SlowMotion()
    {
        /*SlowMotio*/
        yield return new WaitForSeconds(0.2f);
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
            Debug.Log(ex.Message + "-->" + expression);
            throw new System.Exception(ex.Message + "-->" + expression); //("????");
        }
        
    }


    private string CalcolaPunteggio(string risultatoOperazione, Solutions[] soluzioni)
    {
        risultatoOperazione = risultatoOperazione.Replace(",", ".");
        int punteggioAssegnatoAlGiocatore = 0;
        if (!double.TryParse(risultatoOperazione, out double numeroTrovatoDalGiocatore)) { numeroTrovatoDalGiocatore=0; }
        Solutions SoluzioneTrovata = null;
        foreach (Solutions item in soluzioni)
        {
            
            if(item.Number == numeroTrovatoDalGiocatore && !soluzioniTrovate.Contains(item))
            {
                 
                SoluzioneTrovata = item;
                break;
            }
        }
        if (SoluzioneTrovata!=null)
        {
            
            soluzioniTrovate.Add(SoluzioneTrovata);
            DatiGioco.SoluzioniTrovate.Add(SoluzioneTrovata);

            punteggioAssegnatoAlGiocatore += BASE_POINTS*BONUS_X;
            GameObject box = GameObject.Find("Goal_"+SoluzioneTrovata.Id_solution.ToString());
            SpriteRenderer spr = box.GetComponent<SpriteRenderer>();
            spr.sprite = Resources.Load<Sprite>("Sprites/boxes/box_v");
            BONUS_X *= 2;
            audio_s.PlayOneShot(EffettiSonori[1], 1F);
            /**/
            DatiGioco.PercorsoSoluzioneDaSuggerire = new List<string>();
            MostraTilesBusy = false;
            StopAllCoroutines();
            /**/
            colora("v");
            StartCoroutine(ColoraSelezionati(.5F, "g"));
            if (BONUS_X > 1)
            {
                string spr_bonus_name = "x" + BONUS_X;
                SpriteRenderer spr_bonus = bonusSpc.GetComponent<SpriteRenderer>();
               
                Animator bonus_anim = bonusSpc.GetComponent<Animator>();
                bonus_anim.SetTrigger("test");
                spr_bonus.sprite = Resources.Load<Sprite>("Sprites/bonus/" + spr_bonus_name);
            }
            

        } else
        {
            BONUS_X = 1;
            audio_s.PlayOneShot(EffettiSonori[2], 1F);
            string spr_bonus_name = "x" + BONUS_X;
            SpriteRenderer spr_bonus = bonusSpc.GetComponent<SpriteRenderer>();
            spr_bonus.sprite = null;

            colora("r");
            StartCoroutine(ColoraSelezionati(.5F, "g"));
            
        }
        return punteggioAssegnatoAlGiocatore.ToString();
    }

    private void colora(string col)
    {
        foreach (GameObject itm in esagoniSelezionati)
        {
            Comp_Esagono scr_e = itm.GetComponent<Comp_Esagono>();
            SpriteRenderer spr = itm.GetComponent<SpriteRenderer>();

            scr_e.Selected = false;

            if (itm.tag != "op")
            {
                spr.sprite = Resources.Load<Sprite>("Sprites/Exs_Numbers/" + scr_e.Number.ToString() + "_"+col);
            }
            else
            {
                if (col=="g")
                    spr.sprite = Resources.Load<Sprite>("Sprites/operand/" + scr_e.Number.ToString());
                else
                    spr.sprite = Resources.Load<Sprite>("Sprites/operand/" + scr_e.Number.ToString() + "_" + col);
            }

        }
    }

    IEnumerator GameWin_async()
    {

        yield return new WaitForSeconds(4);


        if (DatiGioco.GrigliaDiGioco.Difficulty == DatiGioco.LivelloCorrente)
            DatiGioco.LivelloCorrente = 0;
        else
            DatiGioco.LivelloCorrente++;
        // Debug.Log("Livello_fine:" + DatiGioco.LivelloCorrente);
        
        if (Advertisement.IsReady()) Advertisement.Show("video");
        SceneManager.LoadScene("ScenaDownload");
    }

    IEnumerator ColoraSelezionati(float seconds, string col)
    {
        
        yield return new WaitForSeconds(seconds);
        colora(col);
        esagoniSelezionati.Clear();
    }

    public void OnVideobtnClick()
    {
        ShowOptions sh = new ShowOptions();
       
        if (Advertisement.IsReady()) Advertisement.Show("rewardedVideo");
    }


}


public class AdvList : IUnityAdsListener
{
    private returnStato st;
    public void OnUnityAdsDidError(string message)
    {
        st.stato = false;
        st.mess = message;

    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if(showResult != ShowResult.Finished)
        {
            st.stato = false;
            st.mess = "";
        } else
        {
            st.stato = true;
        }
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        //throw new NotImplementedException();
    }

    public void OnUnityAdsReady(string placementId)
    {
        //throw new NotImplementedException();
    }

    public returnStato status { get; set; }
}

public struct returnStato
{
  
    public bool stato { get; set; }
    public string mess { get; set; }
}
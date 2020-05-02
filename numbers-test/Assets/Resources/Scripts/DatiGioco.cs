using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DatiGioco 
{
    
    public static Grids GrigliaDiGioco { get; set; }
    public static Solutions[] soluzioni { get; set; }
    public static int LivelloCorrente { get; set; }
    public static int PuntiGiocatore { get; set; }

    public static bool FreezeTime { get; set; }

    public static List<Solutions> SoluzioniTrovate { get; set; }

    public static List<string> PercorsoSoluzioneDaSuggerire { get; set; }

    public static Users user;

    public static Matchmaking matchCorrente { get; set; }
    public static int TempoPerLivelloCorrente { get; set; }
    public static int TempoAvanzato { get; set; }

    public static int StripBonus { get; set; }




}

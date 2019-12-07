using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DatiGioco 
{
    public static Grids GrigliaDiGioco { get; set; }
    public static Solutions[] soluzioni { get; set; }
    public static int LivelloCorrente { get; set; }
    public static int PuntiGiocatore { get; set; }

    public static List<Solutions> SoluzioniTrovate { get; set; }

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class ServizioNumbers : UnityWebRequest
{
    private string _url;

    public UnityWebRequestAsyncOperation getGrid;


    /// <remarks/>
    public ServizioNumbers() {
        _url = "http://numbers.jemaka.it/NumberService.asmx";
    }
    
    /*
    public Solutions[] getSolutionsbyGrid(int idGrid) {
        object[] results = this.Invoke("getSolutionsbyGrid", new object[] {
                    idGrid});
        
        return ((Solutions[])(results[0]));
    }
    */



    public void GetGrid()
    {
        UnityWebRequest www = UnityWebRequest.Get(_url+"/getGrid");

        getGrid = www.SendWebRequest();
        string ret = "";
        op.completed += (aop) => {
            if (www.isNetworkError || www.isHttpError)
            {
                
                Debug.Log(www.error);
                 ret= www.error;
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                ret = www.downloadHandler.text;
            }
        };

        //return ret;
        
    }

    
    

   /*
    public Users getUser(string uuid) {
        object[] results = this.Invoke("getUser", new object[] {
                    uuid});
        return ((Users)(results[0]));
    }
    
    public string setUsers(Users u) {
        object[] results = this.Invoke("setUsers", new object[] {
                    u});
        return ((string)(results[0]));
    }
    
    public void updUserScore1(int id_user, float score1) {
        this.Invoke("updUserScore1", new object[] {
                    id_user,
                    score1});
    }
    
  */
}

public partial class Solutions {
    
    /// <remarks/>
    public int Id_solution;
    
    /// <remarks/>
    public int Id_grid;
    
    /// <remarks/>
    public float Number;
    
    /// <remarks/>
    public string Sequence;
    
    /// <remarks/>
    public float Difficulty;
    
    /// <remarks/>
    public string Note;
}

public partial class Users {
    
    /// <remarks/>
    public int Id_user;
    
    /// <remarks/>
    public string Nickname;
    
    /// <remarks/>
    public string Imei;
    
    /// <remarks/>
    public string Uuid;
    
    /// <remarks/>
    public System.DateTime Data_setup;
    
    /// <remarks/>
    public string Email;
    
    /// <remarks/>
    public string Service_id;
    
    /// <remarks/>
    public string Note;
    
    /// <remarks/>
    public float Score1;
    
    /// <remarks/>
    public float Score2;
    
    /// <remarks/>
    public float Bonus1;
    
    /// <remarks/>
    public float Bonus2;
}

public partial class Grids {
    
    /// <remarks/>
    public int Id_grid;
    
    /// <remarks/>
    public string Item;
    
    /// <remarks/>
    public System.DateTime Data_creation;
    
    /// <remarks/>
    public float Difficulty;
    
    /// <remarks/>
    public Solutions Solutions;
}


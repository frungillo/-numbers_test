

//using Boo.Lang;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Solutions {

    public int Id_solution;
    public int Id_grid;
    public float Number;
    public string Sequence;
    public float Difficulty;
    public string Note;

    public override bool Equals(object obj)
    {
        Solutions inputSol = obj as Solutions;
        if (inputSol.Id_solution == this.Id_solution) return true; else return false;
    }

    public override int GetHashCode()
    {
        return this.GetHashCode();
    }
}

public  class Users {
    
    private int id_userField;
    
    private string nicknameField;
    
    private string imeiField;
    
    private string uuidField;
    
    private System.DateTime data_setupField;
    
    private string emailField;
    
    private string service_idField;
    
    private string noteField;
    
    private float score1Field;
    
    private float score2Field;
    
    private float bonus1Field;
    
    private float bonus2Field;
    private Image _userProfileImage;

    
    /// <remarks/>
    public int Id_user {
        get {
            return this.id_userField;
        }
        set {
            this.id_userField = value;
        }
    }
    
    /// <remarks/>
    public string Nickname {
        get {
            return this.nicknameField;
        }
        set {
            this.nicknameField = value;
        }
    }
    
    /// <remarks/>
    public string Imei {
        get {
            return this.imeiField;
        }
        set {
            this.imeiField = value;
        }
    }
    
    /// <remarks/>
    public string Uuid {
        get {
            return this.uuidField;
        }
        set {
            this.uuidField = value;
        }
    }
    
    /// <remarks/>
    public System.DateTime Data_setup {
        get {
            return this.data_setupField;
        }
        set {
            this.data_setupField = value;
        }
    }
    
    /// <remarks/>
    public string Email {
        get {
            return this.emailField;
        }
        set {
            this.emailField = value;
        }
    }
    
    /// <remarks/>
    public string Service_id {
        get {
            return this.service_idField;
        }
        set {
            this.service_idField = value;
        }
    }
    
    /// <remarks/>
    public string Note {
        get {
            return this.noteField;
        }
        set {
            this.noteField = value;
        }
    }
    
    /// <remarks/>
    public float Score1 {
        get {
            return this.score1Field;
        }
        set {
            this.score1Field = value;
        }
    }
    
    /// <remarks/>
    public float Score2 {
        get {
            return this.score2Field;
        }
        set {
            this.score2Field = value;
        }
    }
    
    /// <remarks/>
    public float Bonus1 {
        get {
            return this.bonus1Field;
        }
        set {
            this.bonus1Field = value;
        }
    }
    
    /// <remarks/>
    public float Bonus2 {
        get {
            return this.bonus2Field;
        }
        set {
            this.bonus2Field = value;
        }
    }

    public Image UserProfileImage { get => _userProfileImage; set => _userProfileImage = value; }
}

[Serializable]
public  class Grids {

   

    public int Id_grid;
    public string Item;
    public DateTime Data_creation;
    public float Difficulty;
    public Solutions[] Soluzioni;

}
[Serializable]
public class Matchmaking
{
    public int Id_matchmaking;
    public int Idplayer;
    public DateTime Data_matching;
    public int Level_match;
    public DateTime Data_waiting;
}
[Serializable]
public class versus
{
    public int IdVersus;
    public int IdPlayer1;
    public int IdPlayer2;
    public DateTime DataStart;
    public int IdGrid;
    public DateTime DataStop;
}


public class Summary
{
    public int total_count { get; set; }
}

public class Friends
{
    public List<object> data { get; set; }
    public Summary summary { get; set; }
}

public class FacebookUser
{
    public string birthday { get; set; }
    public Friends friends { get; set; }
    public string name { get; set; }
    public string id { get; set; }
}

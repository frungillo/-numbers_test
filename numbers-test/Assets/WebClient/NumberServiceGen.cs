

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

    private int _id_user;
    private string _nickname;
    private string _imei;
    private string _uuid;
    private DateTime _data_setup;
    private string _email;
    private string _service_id;
    private string _note;
    private float _single_score;
    private float _levels;
    private float _money;
    private float _match_score;
    private Image _userProfileImage;
    

    public Users()
    {
        this.Id_user = 0;
        this.Nickname = "";
        this.Imei = "";
        this.Uuid = "";
        this.Data_setup = DateTime.Now;
        this.Email = "";
        this.Service_id = "";
        this.Note = "";
        this.Single_score = 0;
        this.Levels = 0;
        this.Money = 0;
        this.Match_score = 0;
        
    }
    public int Id_user { get => _id_user; set => _id_user = value; }
    public string Nickname { get => _nickname; set => _nickname = value; }
    public string Imei { get => _imei; set => _imei = value; }
    public string Uuid { get => _uuid; set => _uuid = value; }
    public DateTime Data_setup { get => _data_setup; set => _data_setup = value; }
    public string Email { get => _email; set => _email = value; }
    public string Service_id { get => _service_id; set => _service_id = value; }
    public string Note { get => _note; set => _note = value; }
    public float Single_score { get => _single_score; set => _single_score = value; }
    public float Levels { get => _levels; set => _levels = value; }
    public float Money { get => _money; set => _money = value; }
    public float Match_score { get => _match_score; set => _match_score = value; }
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
    public Users UsersData;
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

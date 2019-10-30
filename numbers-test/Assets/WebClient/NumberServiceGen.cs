

public class Solutions {
    
    private int id_solutionField;
    
    private int id_gridField;
    
    private float numberField;
    
    private string sequenceField;
    
    private float difficultyField;
    
    private string noteField;
    
    /// <remarks/>
    public int Id_solution {
        get {
            return this.id_solutionField;
        }
        set {
            this.id_solutionField = value;
        }
    }
    
    /// <remarks/>
    public int Id_grid {
        get {
            return this.id_gridField;
        }
        set {
            this.id_gridField = value;
        }
    }
    
    /// <remarks/>
    public float Number {
        get {
            return this.numberField;
        }
        set {
            this.numberField = value;
        }
    }
    
    /// <remarks/>
    public string Sequence {
        get {
            return this.sequenceField;
        }
        set {
            this.sequenceField = value;
        }
    }
    
    /// <remarks/>
    public float Difficulty {
        get {
            return this.difficultyField;
        }
        set {
            this.difficultyField = value;
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
}

public partial class Users {
    
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
}

public partial class Grids {
    
    private int id_gridField;
    
    private string itemField;
    
    private System.DateTime data_creationField;
    
    private float difficultyField;
    
    private Solutions[] solutionsField;
    
    /// <remarks/>
    public int Id_grid {
        get {
            return this.id_gridField;
        }
        set {
            this.id_gridField = value;
        }
    }
    
    /// <remarks/>
    public string Item {
        get {
            return this.itemField;
        }
        set {
            this.itemField = value;
        }
    }
    
    /// <remarks/>
    public System.DateTime Data_creation {
        get {
            return this.data_creationField;
        }
        set {
            this.data_creationField = value;
        }
    }
    
    /// <remarks/>
    public float Difficulty {
        get {
            return this.difficultyField;
        }
        set {
            this.difficultyField = value;
        }
    }
    
    /// <remarks/>
    public Solutions[] Soluzioni {
        get {
            return this.solutionsField;
        }
        set {
            this.solutionsField = value;
        }
    }
}


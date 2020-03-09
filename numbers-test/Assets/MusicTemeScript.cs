using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTemeScript : MonoBehaviour
{
    private static MusicTemeScript _instance = null;
    public static MusicTemeScript Instance
    {
        get { return _instance; }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        if(_instance!= null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        } else
        {
            _instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

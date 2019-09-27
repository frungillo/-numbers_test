using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public GameObject exs;
    
    // Start is called before the first frame update
    void Start()
    {
        //GameObject obj = exs.GetComponentInChildren<GameObject>();
        SpriteRenderer spr = (SpriteRenderer)exs.GetComponentsInChildren(typeof(SpriteRenderer))[1];
        System.Random rnd = new System.Random(); 
       
        float x = (float)-7.3;
        float y = (float)3.5;
        for (int i = 1; i < 10; i++)
        {
            int num = rnd.Next(0, 9);
            spr.sprite = Resources.Load<Sprite>("Sprites/" + num.ToString());//GOAL!
            Instantiate(
                   exs,
                   new Vector3(x, y, 1),
                   Quaternion.identity);
            if (i % 3 == 0 && i != 1)
            { y -= (float)2.4; x = (float)-7.3; }
            else
            {
                x += (float)2.4;
            }
            
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }


}

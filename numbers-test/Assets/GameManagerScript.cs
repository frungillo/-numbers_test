using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public GameObject exs;
    public GameObject ops;
    
    // Start is called before the first frame update
    void Start()
    {
        //GameObject obj = exs.GetComponentInChildren<GameObject>();
        SpriteRenderer spr = exs.GetComponent<SpriteRenderer>(); //(SpriteRenderer)exs.GetComponentsInChildren(typeof(SpriteRenderer))[1];
        System.Random rnd = new System.Random();
        SpriteRenderer spr_op = ops.GetComponent<SpriteRenderer>(); //(SpriteRenderer)exs.GetComponentsInChildren(typeof(SpriteRenderer))[1];
        System.Random rnd_op = new System.Random();

        float y = (float)5; //3.5
        
        for (int row =7;row >1; row--)
        {
            float x = (float)-5; //-7.3
            float x_op = (float)-3.53; //-7.3

            int max_col = 5;
            int max_col_op = 4;
            if (row % 2 == 0) { max_col = 4; x = (float)-3.51; }
            if (row % 2 == 0) { max_col_op = 5; x_op = (float)-4.99; }
            for (int col = 1; col < max_col; col++)
            {
                int num = rnd.Next(0, 9);
                spr.sprite = Resources.Load<Sprite>("Sprites/Exs_Numbers/" + num.ToString() + "_g");//GOAL!
                Instantiate(
                       exs,
                       new Vector3(x, y, 1),
                       Quaternion.identity
                       );
 
                 x += (float)3.0;
            }

            for (int col = 1; col < max_col_op; col++)
            {
                int num = rnd_op.Next(1, 4);
                spr_op.sprite = Resources.Load<Sprite>("Sprites/operand/" + num.ToString());//GOAL!
                Instantiate(
                       ops,
                       new Vector3(x_op, y, 1),
                       Quaternion.identity
                       );

                x_op += (float)3.0;
            }

            y -= (float)1.5;
        }
       

    }

    // Update is called once per frame
    void Update()
    {
        
    }


}

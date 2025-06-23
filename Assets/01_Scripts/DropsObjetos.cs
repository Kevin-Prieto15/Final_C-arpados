using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropsObjetos : MonoBehaviour
{
    // Start is called before the first frame update
    public float life;
    public GameObject Drop;
    public GameObject SecondDrop;



    // Update is called once per frame
    public void takeDamage(float damage)
    {
        life -= damage;
        if (life <= 0)
        {
            Instantiate(Drop, transform.position, Quaternion.identity);
            if (SecondDrop != null)
            {
                if (Random.Range(1, 4) == 1)
                {
                    Instantiate(SecondDrop, new Vector2(transform.position.x+0.3f, transform.position.y), Quaternion.identity);
                }
            }
            Destroy(gameObject);
        }
    }
}

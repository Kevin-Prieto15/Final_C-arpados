using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropsObjetos : MonoBehaviour
{
    // Start is called before the first frame update
    public float life;
    public GameObject Drop;

    

    // Update is called once per frame
    public void takeDamage(float damage)
    {
        life -= damage;
        if (life <= 0)
        {
            Instantiate(Drop, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeHit : MonoBehaviour
{
    int numberCubes = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (numberCubes >= 20)//molta gent
        {
            gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
        }
        else if (numberCubes <= 20 && numberCubes >= 10) //hi npasa poc queña
        {
            gameObject.GetComponent<Renderer>().material.color = new Color(0, 1, 0);
        }
        else//esta sool
        {
            gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 1);
        }

       
    }

    private void OnTriggerEnter(Collider other)
    {
       if (other.gameObject.CompareTag("Cube")) 
       {
            numberCubes++;
       }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Hitmap : MonoBehaviour
{
    public GameObject _cube;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void InstanceHeadMap(InfoQuery[] hitmap)
    {
        for (int i = 0; i < hitmap.Length; i++)
        {
            switch (hitmap[i].type)
            {
                case SPATIAL_EVENT_TYPE.POSITION:
                    Instantiate(_cube, new Vector3(hitmap[i].posX, hitmap[i].posY, hitmap[i].posZ), Quaternion.identity);
                    break;
                case SPATIAL_EVENT_TYPE.DEATH:
                    Instantiate(_cube, new Vector3(hitmap[i].posX, hitmap[i].posY, hitmap[i].posZ), Quaternion.identity);
                    break;
                case SPATIAL_EVENT_TYPE.DAMAGE:
                    Instantiate(_cube, new Vector3(hitmap[i].posX, hitmap[i].posY, hitmap[i].posZ), Quaternion.identity);
                    break;
                default:
                    break;
            }
        }
    }
}
public class CallbackHitmap
{
    public static Action<InfoQuery[]> OnDadesReceives;
}

using Gamekit3D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

public class DadesRecive : MonoBehaviour 
{
    // Start is called before the first frame update

    public struct InfoQuery
    {

       public SPATIAL_EVENT_TYPE type;
       public float posX;
       public float posY;
       public float posZ;
    }

    InfoQuery[] _query;
    int _queryCount = 0;
    
    public class QServer
    {

        public virtual WWWForm GetForm()
        {
            WWWForm form = new WWWForm();
            return form;
        }
    };

    public class QEvents : QServer
    {
        public enum SPATIAL_EVENT_TYPE
        {
            POSITION,
            DEATH,
            DAMAGE
        }

        public string query;
        public string type;


        public override WWWForm GetForm()
        {
            WWWForm form = new WWWForm();

            form.AddField("QueryFilter", query);
            form.AddField("TypeFilter", type);
            return form;
        }
    };

    QEvents qEvents = new QEvents();

    private void OnEnable()
    {
      
        //Ellen.onDamageMessageReceivers.Add(this);
    }

    private void OnDisable()
    {

    }


    void Start()
    {
        _query = new InfoQuery[100];
        _queryCount = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            NewSpatialEvent(Simulator.FiltreInfo(SPATIAL_EVENT_TYPE.POSITION));
        }

        //EnvironmentVariableTarget = UploadtoServer(qEvents, "CreateSpatialEvent");
    }

    IEnumerator UploadtoServer(QServer s)
    {
        _queryCount++;
        WWWForm form = new WWWForm();
        form = s.GetForm();

        UnityWebRequest www = UnityWebRequest.Post("https://citmalumnes.upc.es/~polrb/RecieveSpatialEventQuery.php", form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {   
            string responseText = www.downloadHandler.text;

            // Supongamos que el formato del string es " session_id,player_id,type,PositionX,PositionY,PositionZ from SpatialEvents where Type = " + "'" + type.ToString() + "'""
            string[] parts = responseText.Split('\n');
            Debug.Log(responseText);
            for (int i = 0; i < parts.Length; i++)
            {
                string[] temp = parts[i].Split(';');
                // Intentar convertir las partes del string a los tipos correctos
                if (Enum.TryParse(temp[0], out _query[_queryCount].type) &&
                    float.TryParse(temp[1], out _query[_queryCount].posX) &&
                    float.TryParse(temp[2], out _query[_queryCount].posY) &&
                    float.TryParse(temp[3], out _query[_queryCount].posZ))
                {
                    // Acceder a los datos en la estructura
                    Debug.Log("Tipo de evento: " + _query[_queryCount].type);
                    Debug.Log("Posición X: " + _query[_queryCount].posX);
                    Debug.Log("Posición Y: " + _query[_queryCount].posY);
                    Debug.Log("Posición Z: " + _query[_queryCount].posZ);
                }
                else
                {
                    Debug.LogError("Error al convertir valores en el string.");
                }
            }


        }
    }


    public void NewSpatialEvent(string query)
    {
        qEvents.query = "Select Type,PositionX,PositionY,PositionZ from SpatialEvents where Type = ";
        qEvents.type = query;
        StartCoroutine(UploadtoServer(qEvents));
    }

}

using Gamekit3D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;


public struct InfoQuery
{

    public SPATIAL_EVENT_TYPE type;
    public float posX;
    public float posY;
    public float posZ;
}

public class DadesRecive : MonoBehaviour 
{
    // Start is called before the first frame update



    InfoQuery[] _query;
    int _queryCount = 0;
    Hitmap heatmap;


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
        CallbackHitmap.OnDadesReceives += heatmap.InstanceHeadMap;
        //Ellen.onDamageMessageReceivers.Add(this);
    }

    private void OnDisable()
    {
        
    }


    void Start()
    {
        _query = new InfoQuery[2000];
        _queryCount = 0;
        heatmap = gameObject.GetComponent<Hitmap>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            _queryCount = 0;
            NewSpatialEvent(Simulator.FiltreInfo(SPATIAL_EVENT_TYPE.POSITION));

        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            _queryCount = 0;
            NewSpatialEvent(Simulator.FiltreInfo(SPATIAL_EVENT_TYPE.DEATH));
        
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            _queryCount = 0;
            NewSpatialEvent(Simulator.FiltreInfo(SPATIAL_EVENT_TYPE.DAMAGE));
   
        }

        //EnvironmentVariableTarget = UploadtoServer(qEvents, "CreateSpatialEvent");
    }

    IEnumerator UploadtoServer(QServer s)
    {
        //Acero de damasco <3
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
                _queryCount++;
                string[] temp = parts[i].Split(';');
                // Intentar convertir las partes del string a los tipos correctos
                if (Enum.TryParse(temp[0], out _query[_queryCount].type) &&
                    float.TryParse(temp[1], NumberStyles.Float, CultureInfo.InvariantCulture, out _query[_queryCount].posX) &&
                    float.TryParse(temp[2], NumberStyles.Float, CultureInfo.InvariantCulture, out _query[_queryCount].posY) &&
                    float.TryParse(temp[3], NumberStyles.Float, CultureInfo.InvariantCulture, out _query[_queryCount].posZ))
                {
                    // Acceder a los datos en la estructura
                    Debug.Log("Tipo de evento: " + _query[_queryCount].type);
                    Debug.Log("Posición X: " + _query[_queryCount].posX);
                    Debug.Log("Posición Y: " + _query[_queryCount].posY);
                    Debug.Log("Posición Z: " + _query[_queryCount].posZ);
                }
            }
            
            Debug.Log("Finished");
            heatmap.InstanceHeadMap(_query);
        }
    }


    public void NewSpatialEvent(string query)
    {
        qEvents.query = "Select Type,PositionX,PositionY,PositionZ from SpatialEvents where Type = ";
        qEvents.type = query;
        StartCoroutine(UploadtoServer(qEvents));
    }


}

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
       public DateTime dateEvent;
       
       public uint level;
       public uint userID;
       public uint sessionID;
       
       public float posX;
       public float posY;
       public float posZ;
       
       public int step;
       public int levelEventId;
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


        public override WWWForm GetForm()
        {
            WWWForm form = new WWWForm();

            form.AddField("QueryFilter", query);

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

        _queryCount = 0;

    }

    // Update is called once per frame
    void Update()
    {
        //EnvironmentVariableTarget = UploadtoServer(qEvents, "CreateSpatialEvent");
    }

    IEnumerator UploadtoServer(QServer s,)
    {
        _queryCount++;
        WWWForm form = new WWWForm();
        form = s.GetForm();

        UnityWebRequest www = UnityWebRequest.Post("https://citmalumnes.upc.es/~polrb/" + link + ".php", form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            string responseText = www.downloadHandler.text;

            // Supongamos que el formato del string es "Tipo|Fecha|Nivel|Usuario|Sesion|PosX|PosY|PosZ|Paso|IdEventoNivel"
            string[] parts = responseText.Split(',');

            if (parts.Length == 10)
            {
                 

                // Intentar convertir las partes del string a los tipos correctos
                if (Enum.TryParse(parts[0], out _query[_queryCount].type) &&
                    DateTime.TryParse(parts[1], out _query[_queryCount].dateEvent) &&
                    uint.TryParse(parts[2], out _query[_queryCount].level) &&
                    uint.TryParse(parts[3], out _query[_queryCount].userID) &&
                    uint.TryParse(parts[4], out _query[_queryCount].sessionID) &&
                    float.TryParse(parts[5], out _query[_queryCount].posX) &&
                    float.TryParse(parts[6], out _query[_queryCount].posY) &&
                    float.TryParse(parts[7], out _query[_queryCount].posZ) &&
                    int.TryParse(parts[8], out _query[_queryCount].step) &&
                    int.TryParse(parts[9], out _query[_queryCount].levelEventId))
                {
                    // Acceder a los datos en la estructura
                    Debug.Log("Tipo de evento: " + _query[_queryCount].type);
                    Debug.Log("Fecha del evento: " + _query[_queryCount].dateEvent);
                    Debug.Log("Nivel: " + _query[_queryCount].level);
                    Debug.Log("Usuario ID: " + _query[_queryCount].userID);
                    Debug.Log("Sesión ID: " + _query[_queryCount].sessionID);
                    Debug.Log("Posición X: " + _query[_queryCount].posX);
                    Debug.Log("Posición Y: " + _query[_queryCount].posY);
                    Debug.Log("Posición Z: " + _query[_queryCount].posZ);
                    Debug.Log("Paso: " + _query[_queryCount].step);
                    Debug.Log("ID del evento de nivel: " + _query[_queryCount].levelEventId);
                }
                else
                {
                    Debug.LogError("Error al convertir valores en el string.");
                }
            }
            else
            {
                Debug.LogError("Formato de string no válido para deserializar en InfoQuery.");
            }
        }
        // else
        // {
        //     Debug.Log("upload completed!");
        //     Debug.Log(www.downloadHandler.text);
        //
        //
        //      _query[_queryCount].type = www;
        //     _query[_queryCount].dateEvent = www;
        //
        //     _query[_queryCount].level = www;
        //     _query[_queryCount].userID = www;
        //     _query[_queryCount].sessionID = www;
        //
        //     _query[_queryCount].posX = www;
        //     _query[_queryCount].posY = www;
        //     _query[_queryCount].posZ = www;
        //
        //     _query[_queryCount].step = www;
        //     _query[_queryCount].levelEventId = www;
        //
        //     // switch (i)
        //     // {
        //     //     default:
        //     //     case 0:
        //     //         nSession.UserID = uint.Parse(www.downloadHandler.text);
        //     //         CallbackEvents.OnAddPlayerCallback.Invoke(uint.Parse(www.downloadHandler.text));
        //     //         break;
        //     //     case 1:
        //     //         nSession.sessionID = uint.Parse(www.downloadHandler.text);
        //     //         eSession.sessionID = uint.Parse(www.downloadHandler.text);
        //     //         CallbackEvents.OnNewSessionCallback.Invoke(uint.Parse(www.downloadHandler.text));
        //     //         break;
        //     //     case 2:
        //     //         eSession.sessionID = uint.Parse(www.downloadHandler.text);
        //     //         CallbackEvents.OnEndSessionCallback.Invoke(uint.Parse(www.downloadHandler.text));
        //     //         break;
        //     //     case 3:
        //     //
        //     //         //SPATIAL EVENT
        //     //         
        //     //         break;
        //     //
        //     // }         
        // }


    }


    public void NewSpatialEvent(string query)
    {
        StartCoroutine(UploadtoServer(qEvents, "CreateSpatialEvent"));
    }

}

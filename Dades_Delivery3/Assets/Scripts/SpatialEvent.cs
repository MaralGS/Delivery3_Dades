//h ttps://docs.unity3d.com/Manual/UnityWebRequest-SendingForm.html

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static DadesSender;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;


//public enum TYPES
//{
//    PLAYER,
//    NDATE,
//    EDATE,
//    DEATH,
//    DAMAGE
//}
public class SpatialEvent : MonoBehaviour
{
    // Start is called before the first frame update


    public class Server
    {

        public virtual WWWForm GetForm()
        {
            WWWForm form = new WWWForm();
            return form;
        }
    };

    public class SEvents : Server
    {
        public string name;
        public DateTime dateSession;

        public uint UserID;
        public uint sessionID;

        public DateTime deathDate;
        public float posX;
        public float posY;
        public float posZ;

        public int damageCount;

        public override WWWForm GetForm()
        {
            WWWForm form = new WWWForm();
            form.AddField("Name", name);
            form.AddField("DateTime", dateSession.ToString("yyyy-MM-dd hh:mm:ss"));
            form.AddField("UserID", (int)UserID);
            form.AddField("SessionID", (int)sessionID);
            form.AddField("PositionX", posX.ToString());
            form.AddField("PositionY", posY.ToString());
            form.AddField("PositionZ", posZ.ToString());
            form.AddField("DamageCount", damageCount);
            return form;
        }
    };

    
    SEvents sEvents = new SEvents();
    SEvents nSession = new SEvents();
    SEvents eSession = new SEvents();

    private void OnEnable()
    {
        Simulator.OnNewPlayer += SpEvents;
        Simulator.OnNewSession += Newserversession;
        Simulator.OnEndSession += Endserversession;
        // Simulator.OnBuyItem += DeathPlayer;
    }

    private void OnDisable()
    {
        Simulator.OnNewPlayer -= SpEvents;
        Simulator.OnNewSession -= Newserversession;
        Simulator.OnEndSession -= Endserversession;
        // Simulator.OnBuyItem -= DeathPlayer;
    }


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator UploadtoServer(Server s, string link, TYPES type)
    {
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
            Debug.Log("upload completed!");
            Debug.Log(www.downloadHandler.text);
            int i = (int)type;
            switch (i)
            {
                default:
                case 0:
                    nSession.UserID = uint.Parse(www.downloadHandler.text);

                    CallbackEvents.OnAddPlayerCallback.Invoke(uint.Parse(www.downloadHandler.text));
                    break;
                case 1:
                    nSession.sessionID = uint.Parse(www.downloadHandler.text);
                    eSession.sessionID = uint.Parse(www.downloadHandler.text);
                    CallbackEvents.OnNewSessionCallback.Invoke(uint.Parse(www.downloadHandler.text));
                    break;
                case 2:
                    eSession.sessionID = uint.Parse(www.downloadHandler.text);
                    CallbackEvents.OnEndSessionCallback.Invoke(uint.Parse(www.downloadHandler.text));
                    break;
                case 3:
                    //CallbackEvents.OnDeathCallback.Invoke(uint.Parse(www.downloadHandler.text));
                    break;
                case 4:
                    //CallbackEvents.OnDeathCallback.Invoke(uint.Parse(www.downloadHandler.text));
                    break;
            }



        }
    }

    public void SpEvents(string name, DateTime date, Transform position, int playerDmg)
    {

        
       sEvents.name = name;
       sEvents.dateSession = date;
       sEvents.posX = position.position.x;
       sEvents.posY = position.position.y;
       sEvents.posZ = position.position.z;
       sEvents.deathDate = date;
       sEvents.damageCount = playerDmg;

        StartCoroutine(UploadtoServer(sEvents, "SpatialEvents", TYPES.PLAYER));
    }

    public void Newserversession(DateTime date)
    {
        nSession.dateSession = date;
        StartCoroutine(UploadtoServer(nSession, "CreateLogSession", TYPES.NDATE));
    }

    public void Endserversession(DateTime date)
    {
        eSession.dateSession = date;
        StartCoroutine(UploadtoServer(eSession, "CreateEndSession", TYPES.EDATE));
    }

}

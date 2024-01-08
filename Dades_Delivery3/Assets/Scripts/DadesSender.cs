//h ttps://docs.unity3d.com/Manual/UnityWebRequest-SendingForm.html

using Gamekit3D;
using Gamekit3D.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;
using static DadesSender;
using static DadesSender.SEvents;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using Random = UnityEngine.Random;

public enum TYPES
{
    PLAYER,
    NDATE,
    EDATE,
    SPATIAL_EVENT,
}

public class DadesSender : MonoBehaviour, IMessageReceiver
{
    // Start is called before the first frame update

    public Damageable Ellen;

    public class Server
    {

        public virtual WWWForm GetForm()
        {
            WWWForm form = new WWWForm();
            return form;
        }
    };

    public class Player : Server
    {   
        
        public string name;
        public DateTime datePlayer;
 
        public override WWWForm GetForm()
        {
            WWWForm form = new WWWForm();
            form.AddField("Name", name);
            form.AddField("DateTime", datePlayer.ToString("yyyy-MM-dd hh:mm:ss"));
            return form;
        }
    };

    class Session : Server
    {
        public DateTime dateSession;
        public uint UserID;
        public uint sessionID;

        public override WWWForm GetForm()
        {
            WWWForm form = new WWWForm();
            form.AddField("DateTime", dateSession.ToString("yyyy-MM-dd hh:mm:ss"));
            form.AddField("UserID", (int)UserID);
            form.AddField("SessionID", (int)sessionID);
            return form;
        }
    };

    public class SEvents : Server
    {
        public enum SPATIAL_EVENT_TYPE
        {
            POSITION,
            DEATH,
            DAMAGE
        }

        public string name;
        public DateTime dateEvent;

        public uint level;
        public uint userID;
        public uint sessionID;

        public float posX;
        public float posY;
        public float posZ;

        public uint damageCount;

        public int step;
        public int levelEventId;

        public SPATIAL_EVENT_TYPE type;

        public override WWWForm GetForm()
        {
            WWWForm form = new WWWForm();

            form.AddField("LevelEventsID", levelEventId);
            form.AddField("Type", type.ToString());
            form.AddField("Level", (int)level);
            form.AddField("PositionX", posX.ToString(CultureInfo.InvariantCulture));
            form.AddField("PositionY", posY.ToString(CultureInfo.InvariantCulture));
            form.AddField("PositionZ", posZ.ToString(CultureInfo.InvariantCulture));
            form.AddField("UserID", (int)userID);
            form.AddField("SessionID", (int)sessionID);
            form.AddField("DateTime", dateEvent.ToString("yyyy-MM-dd hh:mm:ss"));
            form.AddField("Step", step);

            return form;
        }
    };

    Player player;
    Session nSession = new Session();
    Session eSession = new Session();
    SEvents sEvents = new SEvents();
    int userID = 0;
    DateTime date;
    private void OnEnable()
    {

        Ellen.onDamageMessageReceivers.Add(this);
    }

    private void OnDisable()
    {
       
    }


    void Start()
    {
        userID = Random.Range(1, 4);
        date = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator UploadtoServer(Server s, string link, TYPES type)
    {
       WWWForm form = new WWWForm();
        form = s.GetForm();
        
        UnityWebRequest www = UnityWebRequest.Post("https://citmalumnes.upc.es/~polrb/"+link+".php", form);
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

                    //SPATIAL EVENT
                    
                    break;
      
            }         
        }
    }

    public void Newserverplayer(string name, DateTime date)
    {
        
        player = new Player();
        player.name = name;
        player.datePlayer = date;
        
        StartCoroutine(UploadtoServer(player, "CreatePlayer",TYPES.PLAYER));
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

    public void NewSpatialEvent(SPATIAL_EVENT_TYPE _type, uint _level, float _positionX, float _positionY, float _positionZ, uint _userID, uint _sessionID, DateTime _eventDate,int _step)
    {

        sEvents.type = _type;
        sEvents.level = _level;
        sEvents.posX = _positionX;
        sEvents.posY = _positionY;
        sEvents.posZ = _positionZ;
        sEvents.userID = _userID;
        sEvents.sessionID = _sessionID;
        sEvents.dateEvent = _eventDate;
        sEvents.step = _step;

        StartCoroutine(UploadtoServer(sEvents, "CreateSpatialEvent", TYPES.SPATIAL_EVENT));
    }

    public void OnReceiveMessage(MessageType type, object sender, object msg)
    {
        if(msg == null && sender ==null) { throw new NotImplementedException(); }
        
        Damageable.DamageMessage damageMessage = (Damageable.DamageMessage)msg;

        if(type == MessageType.DAMAGED)
        {
            NewSpatialEvent(SPATIAL_EVENT_TYPE.DAMAGE, 0, damageMessage.direction.x, damageMessage.direction.y, damageMessage.direction.z, (uint)userID, 1, date, 1);
        }
        else if(type == MessageType.DEAD) 
        {
            NewSpatialEvent(SPATIAL_EVENT_TYPE.DEATH, 0, damageMessage.direction.x, damageMessage.direction.y, damageMessage.direction.z, (uint)userID, 1, date, 1);
        } 

    }

}

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

public enum SPATIAL_EVENT_TYPE
{
    POSITION,
    DEATH,
    DAMAGE
}

public class DadesSender : MonoBehaviour, IMessageReceiver
{
    // Start is called before the first frame update

    public Damageable Ellen;
    public PlayerController Ellen_step;
    int _ellen_step = 0;
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
     

        public DateTime dateEvent;

        public uint level;
        public uint userID;
        public uint sessionID;

        public float posX;
        public float posY;
        public float posZ;

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

    SEvents sEvents = new SEvents();
    int userID = 0;
    int sesionID = 0;
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
        sesionID = Random.Range(1, 99999);
        date = DateTime.Now;
        _ellen_step = 0;

        Ellen_step = Ellen.gameObject.GetComponent<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Ellen_step.footstepPlayer.playing == true)
        {
            _ellen_step++;
            NewSpatialEvent(SPATIAL_EVENT_TYPE.POSITION, 0, Ellen.gameObject.transform.position.x, Ellen.gameObject.transform.position.y, Ellen.gameObject.transform.position.z, (uint)userID, (uint)sesionID, date, _ellen_step);
        }

    }

    IEnumerator UploadtoServer(Server s, string link)
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
          
           // switch (i)
           // {
           //     default:
           //     case 0:
           //         nSession.UserID = uint.Parse(www.downloadHandler.text);
           //         CallbackEvents.OnAddPlayerCallback.Invoke(uint.Parse(www.downloadHandler.text));
           //         break;
           //     case 1:
           //         nSession.sessionID = uint.Parse(www.downloadHandler.text);
           //         eSession.sessionID = uint.Parse(www.downloadHandler.text);
           //         CallbackEvents.OnNewSessionCallback.Invoke(uint.Parse(www.downloadHandler.text));
           //         break;
           //     case 2:
           //         eSession.sessionID = uint.Parse(www.downloadHandler.text);
           //         CallbackEvents.OnEndSessionCallback.Invoke(uint.Parse(www.downloadHandler.text));
           //         break;
           //     case 3:
           //
           //         //SPATIAL EVENT
           //         
           //         break;
           //
           // }         
        }
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

        StartCoroutine(UploadtoServer(sEvents, "CreateSpatialEvent"));
    }

    public void OnReceiveMessage(MessageType type, object sender, object msg)
    {
        if(msg == null && sender ==null) { throw new NotImplementedException(); }
        
        Damageable.DamageMessage damageMessage = (Damageable.DamageMessage)msg;
        Damageable ellenPosition = (Damageable)sender;
        
        if(type == MessageType.DAMAGED)
        {
            NewSpatialEvent(SPATIAL_EVENT_TYPE.DAMAGE, 0, ellenPosition.gameObject.transform.position.x, ellenPosition.gameObject.transform.position.y, ellenPosition.gameObject.transform.position.z, (uint)userID, (uint)sesionID, date, _ellen_step);
        }
        else if(type == MessageType.DEAD) 
        {
            NewSpatialEvent(SPATIAL_EVENT_TYPE.DEATH, 0, ellenPosition.gameObject.transform.position.x, ellenPosition.gameObject.transform.position.y, ellenPosition.gameObject.transform.position.z, (uint)userID, (uint)sesionID, date, _ellen_step);
            _ellen_step = 0;
        } 

    }

}

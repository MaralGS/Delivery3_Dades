//h ttps://docs.unity3d.com/Manual/UnityWebRequest-SendingForm.html

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static DadesSender;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;


public enum TYPES
{
    PLAYER,
    NDATE,
    EDATE,
    DEATH,
    DAMAGE
}

public class DadesSender : MonoBehaviour
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

    class Death : Server
    {

        
        public uint userID;
        public DateTime deathDate;
        public uint sessionID;
        public float posX;
        public float posY;
        public float posZ;

        public override WWWForm GetForm()
        {
            WWWForm form = new WWWForm();
            form.AddField("DateItem", deathDate.ToString("yyyy-MM-dd hh:mm:ss"));
            form.AddField("UserID", (int)userID);
            form.AddField("SessionID", (int)sessionID);
            form.AddField("PositionX", posX.ToString());
            form.AddField("PositionY", posY.ToString());
            form.AddField("PositionZ", posZ.ToString());
            return form;
        }

    };
    
    class Damage : Server
    {

        
        public uint userID;
        public int DamageCount;
        public uint sessionID;
        public float posX;
        public float posY;
        public float posZ;

        public override WWWForm GetForm()
        {
            WWWForm form = new WWWForm();
            form.AddField("DamageCount", DamageCount);
            form.AddField("UserID", (int)userID);
            form.AddField("SessionID", (int)sessionID);
            form.AddField("PositionX", posX.ToString());
            form.AddField("PositionY", posY.ToString());
            form.AddField("PositionZ", posZ.ToString());
            return form;
        }

    };

    Player player;
    Session nSession = new Session();
    Session eSession = new Session();
    Death death = new Death();
    Damage damage = new Damage();

    private void OnEnable()
    {
        Simulator.OnNewPlayer += Newserverplayer;
        Simulator.OnNewSession += Newserversession;
        Simulator.OnEndSession+= Endserversession;
        // Simulator.OnBuyItem += DeathPlayer;
    }

    private void OnDisable()
    {
        Simulator.OnNewPlayer -= Newserverplayer;
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
                    death.userID = uint.Parse(www.downloadHandler.text);
                    damage.userID = uint.Parse(www.downloadHandler.text);
                    CallbackEvents.OnAddPlayerCallback.Invoke(uint.Parse(www.downloadHandler.text));
                    break;
                case 1:
                    nSession.sessionID = uint.Parse(www.downloadHandler.text);
                    death.sessionID = uint.Parse(www.downloadHandler.text);
                    damage.sessionID = uint.Parse(www.downloadHandler.text);
                    eSession.sessionID = uint.Parse(www.downloadHandler.text);
                    CallbackEvents.OnNewSessionCallback.Invoke(uint.Parse(www.downloadHandler.text));
                    break;
                case 2:
                    eSession.sessionID = uint.Parse(www.downloadHandler.text);
                    death.sessionID = uint.Parse(www.downloadHandler.text);
                    damage.sessionID = uint.Parse(www.downloadHandler.text);
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
    
   public void DeathPlayer(DateTime date, Transform position)
   {
       death.posX = position.position.x;
       death.posY = position.position.y;
       death.posZ = position.position.z;
       death.deathDate = date;
       StartCoroutine(UploadtoServer(death, "AddDeath", TYPES.DEATH));
   }
    
    public void PlayerDamage(int dmg, Transform position)
   {
       damage.posX = position.position.x;
       damage.posY = position.position.y;
       damage.posZ = position.position.z;
       damage.DamageCount = dmg;
       StartCoroutine(UploadtoServer(death, "AddDeath", TYPES.DEATH));
   }

    
}

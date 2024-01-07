//h ttps://docs.unity3d.com/Manual/UnityWebRequest-SendingForm.html

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static DadeServer;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;


public enum TYPES
{
    PLAYER,
    NDATE,
    EDATE,
    ITEM
}

public class DadeServer : MonoBehaviour
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
        public uint SessionID;

        public override WWWForm GetForm()
        {
            WWWForm form = new WWWForm();
            form.AddField("DateTime", dateSession.ToString("yyyy-MM-dd hh:mm:ss"));
            form.AddField("UserID", (int)UserID);
            form.AddField("SessionID", (int)SessionID);
            return form;
        }
    };

    class BuyItem : Server
    {

        public int Itemid;
        public DateTime dateItem;
        public uint UserID;
        public uint SessionID;

        public override WWWForm GetForm()
        {
            WWWForm form = new WWWForm();
            form.AddField("Id", Itemid);
            form.AddField("DateItem", dateItem.ToString("yyyy-MM-dd hh:mm:ss"));
            form.AddField("UserID", (int)UserID);
            form.AddField("SessionID", (int)SessionID);
            return form;
        }

    };

    Player player;
    Session NSession = new Session();
    Session ESession = new Session();
    BuyItem Item = new BuyItem();

    private void OnEnable()
    {
        Simulator.OnNewPlayer += Newserverplayer;
       // Simulator.OnNewSession += Newserversession;
       // Simulator.OnEndSession+= Endserversession;
       // Simulator.OnBuyItem += BuyItemServer;
    }
    
    private void OnDisable()
    {
        Simulator.OnNewPlayer -= Newserverplayer;
       // Simulator.OnNewSession -= Newserversession;
       // Simulator.OnEndSession -= Endserversession;
       // Simulator.OnBuyItem -= BuyItemServer;
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
                    NSession.UserID = uint.Parse(www.downloadHandler.text);
                    CallbackEvents.OnAddPlayerCallback.Invoke(uint.Parse(www.downloadHandler.text));
                    break;
                case 1:
                    NSession.SessionID = uint.Parse(www.downloadHandler.text);
                    ESession.SessionID = uint.Parse(www.downloadHandler.text);
                    CallbackEvents.OnNewSessionCallback.Invoke(uint.Parse(www.downloadHandler.text));
                    break;
                case 2:
                    ESession.SessionID = uint.Parse(www.downloadHandler.text);
                    CallbackEvents.OnEndSessionCallback.Invoke(uint.Parse(www.downloadHandler.text));
                    break;
                case 3:
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
    
   // public void Newserversession(DateTime date)
   // {
   //     NSession.dateSession = date;
   //     StartCoroutine(UploadtoServer(NSession, "CreateLogSession", TYPES.NDATE));
   // }
   // 
   // public void Endserversession(DateTime date)
   // {
   //     ESession.dateSession = date;
   //     StartCoroutine(UploadtoServer(ESession, "CreateEndSession", TYPES.EDATE));
   // }
   // 
   // public void BuyItemServer(int id, DateTime date)
   // {
   //     Item.Itemid = id;
   //     Item.dateItem = date;
   //     StartCoroutine(UploadtoServer(Item, "AddBuy", TYPES.ITEM));
   // }

    
}

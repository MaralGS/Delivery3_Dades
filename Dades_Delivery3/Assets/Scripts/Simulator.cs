using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using static DadesSender.SEvents;
using Random = UnityEngine.Random;

public class Simulator : MonoBehaviour
{

    public static Action<string, DateTime> OnNewPlayer; //Name, Country and date
    public static Action<DateTime> OnNewSession;
    public static Action<DateTime> OnEndSession;
    public static Action<int, DateTime> OnBuyItem; //Item id and date
    public static Action<SPATIAL_EVENT_TYPE, uint, float, float, float, uint, uint, DateTime, int> OnNewSpatialEvent;
                            // TYPE EVENT, LEVEL,POSX,POSY,POSZ,USERID,SESSIONID,EVENTDATE,STEP

    private DateTime _currentDate;

    public float ReplayChance => _currentDate.Month < 6 ? 0.7f : 0.95f;

    [SerializeField]
    private Lexic.NameGenerator namegen;

    #region Subscribe
    private void OnEnable()
    {
        CallbackEvents.OnAddPlayerCallback += OnPlayerAdded;
        //CallbackEvents.OnNewSessionCallback += OnNewSessionAdded;
        //CallbackEvents.OnEndSessionCallback += OnEndSessionAdded;
    }
   
    private void OnDisable()
    {
        CallbackEvents.OnAddPlayerCallback -= OnPlayerAdded;
       // CallbackEvents.OnNewSessionCallback -= OnNewSessionAdded;
       // CallbackEvents.OnEndSessionCallback -= OnEndSessionAdded;
    }

    #endregion

    void Start()
    {
        MakeOnePlayer();
    }

    void MakeOnePlayer()
    {
        _currentDate = GetNewPlayerDate();
        AddNewPlayer(_currentDate);
    }

    void AddNewPlayer(DateTime dateTime)
    {
        string name = namegen.GetNextRandomName();
        name = name.Replace("'", " ");
        //OnNewPlayer?.Invoke(name, dateTime);
    }

    void AddNewSession()
    {
        DateTime dateTime = _currentDate;
        OnNewSession?.Invoke(dateTime);
    }

    public static string FiltreInfo(SPATIAL_EVENT_TYPE type, int player_id = 0, int session_id = 0)
    {
        string request;

        request = "Select session_id,player_id,type,PositionX,PositionY,PositionZ from SpatialEvents where Type = " + "'" + type.ToString() + "'";

        if (player_id != 0)
        {
            request = request + " and player_id = " + player_id.ToString();
        }
        if (session_id != 0)
        {
            request = request + " and session_id = " + session_id.ToString();
        }

        return request;
    }


   // void EndSession()
   // {
   //     _currentDate = _currentDate.Add(GetSessionLength());
   //     DateTime dateTime = _currentDate;
   //     OnEndSession?.Invoke(dateTime);
   // }

    #region Probabilistic values
    DateTime GetNewPlayerDate()
    {
        int year = 2022;
        int month = Random.Range(1, 13);
        int numOfDays = DateTime.DaysInMonth(year, month);
        int day = Random.Range(1, numOfDays + 1);

        int hour = Random.Range(0, 24);
        int minut = Random.Range(0, 60);
        int second = Random.Range(0, 60);

        return new DateTime(year, month, day, hour, minut, second);
    }


    private void OnPlayerAdded(uint obj)
    {
        AddNewSession();
    }

    private void OnNewSessionAdded(uint obj)
    {
    }

   //private void OnEndSessionAdded(uint obj)
   //{
   //
   //    if (Random.value > ReplayChance)
   //    {
   //        MakeOnePlayer();
   //        return;
   //    }
   //    TimeSpan timeSpan = TimeTillNextSession();
   //
   //    _currentDate = _currentDate.Add(timeSpan);
   //    // Debug.Log(_currentDate.ToLongDateString());
   //
   //    if (_currentDate.Year == 2022)
   //        AddNewSession();
   //    else
   //        MakeOnePlayer();
   //}


    #endregion
}

public class CallbackEvents
{

    public static Action<uint> OnEndSessionCallback;
    public static Action<uint> OnNewSessionCallback;
    public static Action<uint> OnAddPlayerCallback;

}
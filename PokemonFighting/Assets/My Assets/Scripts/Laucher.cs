using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class Laucher : MonoBehaviourPunCallbacks
{
    void Start()
    {
        _StaticData.ar = false;
    }
    #region Private Serializable Fields
    [SerializeField]
    private LoaderAnime loaderAnime;

    [SerializeField]
    private Text roomID;

    [SerializeField]
    private Text gameMode;
    [SerializeField]
    private Text logText;

    #endregion

    #region Private Fields
    bool isConnecting;

    string gameVersion = "1";

    #endregion

    #region MonoBehaviour CallBacks
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = this.gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    #endregion


    #region Public Methods

    public void Connect()
    {
        Debug.Log("Connect room");
        if (gameMode.text.Equals("Arena") && _StaticData.ar)
        {
            logText.text = "Switch to 1vs1 Mode to play in AR.";
            loaderAnime.StopLoaderAnimation();
            return;
        }
        isConnecting = true;


        if (loaderAnime != null)
        {
            loaderAnime.StartLoaderAnimation();
        }

        if (PhotonNetwork.IsConnected)
        {
            if (roomID.text.Length != 0)
                PhotonNetwork.JoinRoom(roomID.text);
            else
                PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.GameVersion = this.gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void changeAR()
    {
        _StaticData.ar = !_StaticData.ar;
        Debug.Log(_StaticData.ar);
    }
    public void CreateRoom()
    {
        Debug.Log("Create room");
        if (gameMode.text.Equals("Arena") && _StaticData.ar)
        {
            logText.text = "Switch to 1vs1 Mode to play in AR.";
            loaderAnime.StopLoaderAnimation();
            return;
        }
        isConnecting = true;


        if (loaderAnime != null)
        {
            loaderAnime.StartLoaderAnimation();
        }

        if (PhotonNetwork.IsConnected)
        {
            if (roomID.text.Length != 0)
            {
                Debug.Log("Room name: " + roomID.text);
                byte maxPlayer = 2;
                int mode = 1;
                if (gameMode.text.Equals("Arena"))
                {
                    maxPlayer = 5;
                    mode = 2;
                } else if (_StaticData.ar)
                {
                    mode = 3;
                }
                RoomOptions options = new RoomOptions();
                options.MaxPlayers = maxPlayer;
                options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "mode", mode } };
                PhotonNetwork.CreateRoom(roomID.text, options);
            }
            else
            {
                logText.text = "Enter room ID to create";
                loaderAnime.StopLoaderAnimation();
            }
        }
        else
        {

            PhotonNetwork.GameVersion = this.gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
    }
    #endregion


    #region MonoBehaviourPunCallbacks CallBacks

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom });
        logText.text = "There is no room right now. Create new one";
        loaderAnime.StopLoaderAnimation();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        logText.text = "There no room name: " + roomID.text;
        loaderAnime.StopLoaderAnimation();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {

        loaderAnime.StopLoaderAnimation();

        isConnecting = false;
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            if (_StaticData.ar)
            {
                PhotonNetwork.LoadLevel("BattleAR");
            } else
            {
                PhotonNetwork.LoadLevel("BattleWithControl");
            }
        }
    }

    #endregion
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System;

public class BattleManager : MonoBehaviourPunCallbacks
{

    #region Private Fields
    [SerializeField]
    private GameObject pkmPrefab;

    #endregion

    #region MonoBehaviour CallBacks
    void Start()
    {
        pkmPrefab = Instantiate(Resources.Load("Controlable/" + _StaticData.choosenPKM.ToString()) as GameObject) ;
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("Laucher");
            return;
        }
        // Create pokemon
        if(BattleControllerScript.LocalPlayerInstance == null)
            PhotonNetwork.Instantiate("Controlable/" + _StaticData.choosenPKM.ToString(), transform.position, Quaternion.identity, 0);
    }
    void Update()
    {

    }

    #endregion
    #region Photon Callbacks

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            LoadArena();
        }
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            LoadArena();
        }
    }
    #endregion

    #region Public Methods

    public void LeaveRoom(string scene)
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(scene);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
    void LoadArena()
    {
        PhotonNetwork.LoadLevel("BattleWithControl");
    }
    public void ProcessDeath(GameObject go)
    {
        LeaveRoom("DeathScene");
        PhotonNetwork.Destroy(go);
    }

    public void ProcessDefeat(GameObject go)
    {
        LeaveRoom("LoseScene");
        PhotonNetwork.Destroy(go);
    }
    public void ProcessVictory(GameObject go)
    {
        LeaveRoom("VictoryScene");
        PhotonNetwork.Destroy(go);
    }
    #endregion
}

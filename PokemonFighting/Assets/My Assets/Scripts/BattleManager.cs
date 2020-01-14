using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class BattleManager : MonoBehaviourPunCallbacks
{

    #region Private Fields
    [SerializeField]
    private GameObject pkmPrefab;

    #endregion

    #region MonoBehaviour CallBacks
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("Laucher");
            return;
        }
        // Create pokemon
        if(BattleControllerScript.LocalPlayerInstance == null)
            PhotonNetwork.Instantiate(this.pkmPrefab.name, transform.position, Quaternion.identity, 0);
    }
    void Update()
    {

    }

    #endregion
    #region Photon Callbacks

    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Launcher");
    }

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

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
    void LoadArena()
    {
        PhotonNetwork.LoadLevel("BattleWithControl");
    }
    #endregion
}

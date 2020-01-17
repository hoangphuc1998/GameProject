using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public abstract class animationPKM : MonoBehaviourPunCallbacks, IPunObservable
{
    public abstract void Attack1(GameObject target=null, int st = 0);
    public GameObject target = null;
    public abstract void Attack2(GameObject target=null, int st = 0);
    public abstract string moveName1();
    public abstract string moveName2();

    public abstract void Attacked();
    public void calculateDamageAndScore(int damage)
    {
        // Calculate damage and score
        BattleControllerScript script = target.GetComponent<BattleControllerScript>();
        int score = script.score;
        int health = script.health;
        target.GetPhotonView().RPC("DecreaseHealth", RpcTarget.All, damage);
        if (health <= damage)
        {
            int gameMode = (int) PhotonNetwork.CurrentRoom.CustomProperties["mode"];
            if (gameMode != 1)
            {
                this.gameObject.GetPhotonView().RPC("IncreaseScore", RpcTarget.All, score / 2 + 1);
            }
            else
            {
                GameObject.Find("BattleManager").GetComponent<BattleManager>().ProcessVictory(this.gameObject);
            }
          
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {

        }
        else
        {

        }
    }
}

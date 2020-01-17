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

    public void Attacked(int power) {
        GetComponent<Animator>().SetTrigger("isDamage");
        if (power == 0)
        {
            photonView.RPC("Damage1Particle", RpcTarget.All);
        }
        else if (power == 1)
        {
            photonView.RPC("Damage2Particle", RpcTarget.All);
        }
    }

    public void calculateDamageAndScore(int damage, int power)
    {
        // Calculate damage and score
        BattleControllerScript script = target.GetComponent<BattleControllerScript>();
        int score = script.score;
        int health = script.health;
        target.GetPhotonView().RPC("DecreaseHealth", RpcTarget.All, damage, power);
        if (health <= damage)
        {
            int gameMode = (int) PhotonNetwork.CurrentRoom.CustomProperties["mode"];
            if (gameMode == 2)
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

    [PunRPC]
    public void Damage1Particle()
    {
        gameObject.transform.Find("Damage1").GetComponent<ParticleSystem>().Play();
    }

    [PunRPC]
    public void Damage2Particle()
    {
        gameObject.transform.Find("Damage2").GetComponent<ParticleSystem>().Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public abstract class animationPKM : MonoBehaviourPunCallbacks, IPunObservable
{
    public abstract void Attack1(GameObject target=null, int st = 0);
    
    public abstract void Attack2(GameObject target=null, int st = 0);
    public abstract string moveName1();
    public abstract string moveName2();
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

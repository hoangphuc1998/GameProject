using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sandbox : MonoBehaviour
{
    public GameObject ModelContainer;
    GameObject pkmPlayer;
    animationPKM pkmPlayerAnim;
    // Start is called before the first frame update
    void Start()
    {
        GameObject prefab1 = Instantiate(Resources.Load(_StaticData.player.curPkmID)) as GameObject;
        prefab1.transform.parent = ModelContainer.transform;
        prefab1.name = prefab1.name.Replace("(Clone)","");
        pkmPlayer = prefab1.transform.GetChild(0).gameObject;
        pkmPlayerAnim = pkmPlayer.GetComponent<animationPKM>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

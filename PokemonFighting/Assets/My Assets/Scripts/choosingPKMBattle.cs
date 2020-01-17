using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class choosingPKMBattle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changePKM(int id)
    {
        Destroy(gameObject.transform.GetChild(0).gameObject);
        GameObject pkm = Instantiate(Resources.Load("Static/" + id.ToString()), gameObject.transform) as GameObject;
        pkm.transform.localPosition = Vector3.zero;
        _StaticData.choosenPKM = id;
    }
}

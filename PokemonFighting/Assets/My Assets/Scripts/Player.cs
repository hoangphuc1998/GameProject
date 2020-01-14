using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {
    public string curPkmID;
    public List<PKM> lst;
    public string name;
    public PKM load(string id) {
        for (int i = 0; i < lst.Count; i++) {
            if (lst[i].id.Equals(id)) {
                return lst[i];
            }
        }
        return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.SceneManagement;

#if !UNITY_EDITOR && UNITY_WEBGL
WebGLInput.captureAllKeyboardInput = true;
#endif
public class RedeemScript : MonoBehaviour
{
    public Text input;
    public Text noti;
    private string gameDataFileName = "data.json";
    private string defaultGameDataFileName = "defaultData.json";
    // Start is called before the first frame update
    void Start()
    {
        loadPkmInfo(0);
        loadPkmInfo(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void returnChoosingPKM() {
        SceneManager.LoadScene("ChoosingPKM");
    }

    void loadPkmInfo(int stt) {
        if (stt == 0) {
            // Load default
            TextAsset file = Resources.Load(defaultGameDataFileName.Replace(".json","")) as TextAsset;
            if (true) {
                string content = file.ToString ();
                _StaticData.opponent.lst = new List<PKM>(JsonHelper.FromJson<PKM>(content));
            } else {
                Debug.Log("default file data load failed");
            }
        } else {
            // load data
            string filePath = Path.Combine(Application.persistentDataPath, gameDataFileName);
            if (File.Exists(filePath)) {
                string dataAsJson = File.ReadAllText(filePath);
                _StaticData.player.lst = new List<PKM>(JsonHelper.FromJson<PKM>(dataAsJson));
            } else {
                TextAsset file1 = Resources.Load(gameDataFileName.Replace(".json","")) as TextAsset;
                string content = file1.ToString ();
                _StaticData.player.lst = new List<PKM>(JsonHelper.FromJson<PKM>(content));
                save();
            }
        }
    }

    void save() {
        string json = JsonHelper.ToJson(_StaticData.player.lst.ToArray(), true);
        string filePath = Path.Combine(Application.persistentDataPath, gameDataFileName);
        File.WriteAllText(filePath, json);
    }

    public void check() {
        for (int i = 0; i < _StaticData.opponent.lst.Count; i++) {
            string checkInput = (getPkmName(_StaticData.opponent.lst[i].id)+ "123456789").Substring(0,8);
            if (input.text.ToLower().Equals(checkInput.ToLower())) {
                _StaticData.player.lst.Add(_StaticData.opponent.lst[i]);
                noti.text = "Congratulation ! You have " + getPkmName(_StaticData.opponent.lst[i].id);
                input.text="";
                save();
                return;
            }
        }
        noti.text = "Invalid Key!";
    }
    
    public string getPkmName(string id) {
        int _id = Int32.Parse(id);
        switch(_id) {
            case 25: return "Pikachu";
            case 182: return "Bellossom";
            case 197: return "Umbreon";
            case 778: return "Mimikyu";
            case 279: return "Pelipper";
            case 146: return "Moltres";
            case 415: return "Combee";
            case 51: return "Dugtrio";
            case 118: return "Goldeen";
        }
        return null;
    }
}

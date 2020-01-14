using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class choosingPKMModelController : MonoBehaviour
{
    public Text hpText;
    public Text atkText;
    public Text defText;
    public Image transition;
    private string gameDataFileName = "data.json";
    private string defaultGameDataFileName = "defaultData.json";
    public GameObject buttonContainer;
    public Image imgBg;
    // Start is called before the first frame update
    void Start()
    {
        transition.GetComponent<Image>().enabled = false;
        loadPkmInfo(1);
        loadPkmInfo(0);
        Debug.Log(_StaticData.player.lst.Count);
        for (int i = 0; i < _StaticData.player.lst.Count; i++) {
            int captureIndex = i;
            GameObject btn = Instantiate(Resources.Load("buttonPickPKM")) as GameObject;
            //btn.transform.GetChild(0).GetComponent<Text>().text = getPkmName(_StaticData.player.lst[i].id);
            btn.transform.SetParent(buttonContainer.transform);
            btn.transform.localScale = new Vector2(1,1);
            btn.transform.Find("Image").GetComponent<Image>().sprite = getButtonSprite(_StaticData.player.lst[captureIndex].id);
            btn.GetComponent<Button>().onClick.AddListener(() => {loadInfo(_StaticData.player.lst[captureIndex]);show(_StaticData.player.lst[captureIndex].id); changeBG(getSprite(_StaticData.player.lst[captureIndex].id)); });
        }
        loadInfo(_StaticData.player.lst[0]);
            show(_StaticData.player.lst[0].id);
            changeBG(getSprite(_StaticData.player.lst[0].id));
        
    }

    void loadInfo(PKM pkm) {
        hpText.text = Convert.ToString(pkm.hp);
        defText.text = Convert.ToString(pkm.def);
        atkText.text = Convert.ToString(pkm.attack);
    }

    GameObject gb;
    public void show(string id) {
        if (transform.childCount != 0) {
            Destroy(transform.GetChild(0).gameObject);
        }
        _StaticData.player.curPkmID = id;
        gb = Instantiate(Resources.Load("choosingModel/" + id)) as GameObject;
        gb.transform.parent = transform;
        gb.transform.localPosition = new Vector3(0,0,0);   
    }

    public void changeBG(Sprite bg) {
        imgBg.sprite = bg;
    }
    // Update is called once per frame
    bool isComplete = false;
    int cur = 0;
    void Update()
    {
        if (isComplete) {
            cur += 1;
            if (cur % 5 == 0) transition.fillAmount = (float)cur/100;
            if (cur == 100) {
                SceneManager.LoadScene("Battle");
            }
        }
    }

    public void goToBattle() {
        isComplete = true;
        transition.GetComponent<Image>().enabled = true;
    }

    public void randomPkmOpponent() {
        System.Random random = new System.Random();
        int rnd = random.Next(_StaticData.opponent.lst.Count);
        while (_StaticData.opponent.lst[rnd].id.Equals(_StaticData.player.curPkmID)) {
            rnd = random.Next(_StaticData.opponent.lst.Count);
        }
        _StaticData.opponent.curPkmID = _StaticData.opponent.lst[rnd].id;
        Debug.Log("Opponent choose " + getPkmName(_StaticData.opponent.curPkmID));
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

    public Sprite getSprite(string id) {
        int _id = Int32.Parse(id);
        switch(_id) {
            case 25: return Resources.Load<Sprite>("background_choosing/land");
            case 182: return Resources.Load<Sprite>("background_choosing/grass");
            case 197: return Resources.Load<Sprite>("background_choosing/night");
            case 778: return Resources.Load<Sprite>("background_choosing/night");
            case 279: return Resources.Load<Sprite>("background_choosing/sea");
            case 146: return Resources.Load<Sprite>("background_choosing/lava");
            case 415: return Resources.Load<Sprite>("background_choosing/forest");
            case 51: return Resources.Load<Sprite>("background_choosing/moutain");
            case 118: return Resources.Load<Sprite>("background_choosing/sea");
        }
        return null;
    }

    public Sprite getButtonSprite(string id) {
        return Resources.Load<Sprite>("pokemon button/" + id);
    }

    public void redeemPKM() {
        SceneManager.LoadScene("RedeemPKM");
    }

    public void sandbox() {
        SceneManager.LoadScene("sandbox");
    }
}

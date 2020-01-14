using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;  
using UnityEngine.SceneManagement;


public class ControlScript : MonoBehaviour
{
    private string gameDataFileName = "data.json";
    private string defaultGameDataFileName = "defaultData.json";
    int playerPkmHealthCurrent;
    int opponentPkmHealthCurrent;

    PKM pkmPlayerData;
    PKM pkmOpponentData;

    System.Random rnd = new System.Random();
    GameObject pkmPlayer;
    GameObject pkmOpponent;
    public LineRenderer lineRenderer;
    public GameObject ModelContainer;
    public RenderTexture avatarPlayerPkm;
    public RenderTexture avatarOpponentPkm;
    public Text pkmPlayerName;
    public Text pkmOpponentName;
    Camera cameraAvatarPkm1;
    Camera cameraAvatarPkm2;
    public Image playerHealth;
    public Image opponentHealth;
    public Text playerHealthText;
    public Text opponentHealthText;
    public Transform btnAttack1;
    public Transform btnAttack2;
    int turn = 0;
    bool isEnd = false;
    int loser = -1;
    int timeWaiting = 0;

    public Image borderPlayerAvatar;
    public Image borderOpponentAvatar;
    public Text gameTeller;
    Color32 colorWhite = new Color32(255,255,255,255);
    Color32 colorYellow = new Color32(255,243,0,255);
    Color32 colorHeath0 = new Color32(248,47,47,255);
    Color32 colorHeath1 = new Color32(248,231,47,255);
    Color32 colorHeath2 = new Color32(47,156,248,255);

    animationPKM pkmPlayerAnim;
    animationPKM pkmOppontentAnim;

    public Canvas endScreen;
    public Text winnerText;
    public Text newAttackText;
    public Text newDefenseText;
    public Text newHPText;


    // Start is called before the first frame update
    void Start()
    {   
        loadPkmInfo(0);
        Debug.Log("Loaded opponent's");
        loadPkmInfo(1);
        Debug.Log("Loaded player's");
        endScreen.gameObject.SetActive(false);

        //_StaticData.player.curPkmID = "182";
        //_StaticData.opponent.curPkmID = "146";

        pkmOpponentData = _StaticData.opponent.load(_StaticData.opponent.curPkmID);
        pkmPlayerData = _StaticData.player.load(_StaticData.player.curPkmID);

        //Debug.Log(pkmPlayerData.attack + " " + pkmPlayerData.def);

        playerPkmHealthCurrent = pkmPlayerData.hp;
        opponentPkmHealthCurrent = pkmOpponentData.hp;

        opponentHealth.fillAmount = 1;
        playerHealth.fillAmount = 1;
        playerHealth.color = colorHeath2;
        opponentHealth.color = colorHeath2;

        pkmPlayerName.text = getPkmName(_StaticData.player.curPkmID);
        pkmOpponentName.text = getPkmName(_StaticData.opponent.curPkmID);
        
        GameObject prefab1 = Instantiate(Resources.Load(_StaticData.player.curPkmID)) as GameObject;
        prefab1.transform.parent = ModelContainer.transform;
        prefab1.name = prefab1.name.Replace("(Clone)","");

        GameObject prefab2 = Instantiate(Resources.Load(_StaticData.opponent.curPkmID)) as GameObject;
        prefab2.transform.parent = ModelContainer.transform;
        prefab2.name = prefab2.name.Replace("(Clone)","");

        pkmPlayer = prefab1.transform.GetChild(0).gameObject;
        pkmOpponent = prefab2.transform.GetChild(0).gameObject;

        cameraAvatarPkm1 = pkmPlayer.transform.Find("Camera").GetComponent<Camera>();
        cameraAvatarPkm1.targetTexture = avatarPlayerPkm;

        cameraAvatarPkm2 = pkmOpponent.transform.Find("Camera").GetComponent<Camera>();
        cameraAvatarPkm2.targetTexture = avatarOpponentPkm;

        pkmPlayerAnim = pkmPlayer.GetComponent<animationPKM>();
        pkmOppontentAnim = pkmOpponent.GetComponent<animationPKM>();

        playerHealthText.text = Convert.ToString(max(playerPkmHealthCurrent, 0)) + "/" + Convert.ToString(pkmPlayerData.hp);
        opponentHealthText.text = Convert.ToString(max(opponentPkmHealthCurrent, 0)) + "/" + Convert.ToString(pkmOpponentData.hp);
       // Debug.Log(pkmPlayer.name);
    }

    int newDefense;
    int newAttack;
    int newHP;
    
    void Update()
    {
        if (isEnd) {
            return;
        }
        if (loser != -1) {
            
            endScreen.gameObject.SetActive(true);
            if (loser == 0) {
                winnerText.text = pkmOpponentName.text + " wins";
                newDefense = pkmPlayerData.def + (int) Math.Pow(pkmPlayerData.def, 0.2);
                newAttack = pkmPlayerData.attack + (int) Math.Pow(pkmPlayerData.attack,0.2);
                newHP = pkmPlayerData.hp + (int) Math.Pow(pkmPlayerData.hp,0.25);
            } else {
                winnerText.text = pkmPlayerName.text + " wins";
                newDefense = pkmPlayerData.def + (int) Math.Pow(pkmPlayerData.def, 0.4);
                newAttack = pkmPlayerData.attack + (int) Math.Pow(pkmPlayerData.attack,0.4);
                newHP = pkmPlayerData.hp + (int) Math.Pow(pkmPlayerData.hp,0.2);
            }
            newAttackText.text = Convert.ToString(newAttack) + "(+" + Convert.ToString(- pkmPlayerData.attack + newAttack) + ")";
            newDefenseText.text = Convert.ToString(newDefense) + "(+" + Convert.ToString(- pkmPlayerData.def + newDefense) + ")";
            newHPText.text = Convert.ToString(newHP) + "(+" + Convert.ToString(- pkmPlayerData.hp + newHP) + ")";
            changeStateButtonAttack(false);
            isEnd = true;
            return;
        }
        
        Facing();
        if (turn == 0) {
            borderPlayerAvatar.color = colorYellow;
            borderOpponentAvatar.color = colorWhite;
            changeStateButtonAttack(true);
        } else {
            borderPlayerAvatar.color = colorWhite;
            borderOpponentAvatar.color = colorYellow;
            timeWaiting+=1;
            if (timeWaiting < 50) return;
            timeWaiting = 0;
            randomOpponentAttack();
            changeStateButtonAttack(false);
        }
        
    }

    void changeStateButtonAttack(bool state) {
        btnAttack1.GetComponent<Button>().interactable  = state;
        btnAttack2.GetComponent<Button>().interactable  = state;
    }

    public void Attack1() {
        turn = 1 - turn;
        opponentTakeDamage(pkmPlayerData.power1);
        changeStateButtonAttack(false);
        if (pkmPlayerAnim.moveName1().Equals("")) {
            gameTeller.text = pkmPlayerName.text + " attacks miss!";
        } else {
            gameTeller.text = pkmPlayerName.text + " use " + pkmPlayerAnim.moveName1();
        }
        pkmPlayerAnim.Attack1();
    }

    public void Attack2() {
        turn = 1 - turn;
        opponentTakeDamage(pkmPlayerData.power2);
        changeStateButtonAttack(false);
        if (pkmPlayerAnim.moveName2().Equals("")) {
            gameTeller.text = pkmPlayerName.text + " attacks miss!";
        } else {
            gameTeller.text = pkmPlayerName.text + " use " + pkmPlayerAnim.moveName2();
        }
        pkmPlayerAnim.Attack2();
    }

    public void Facing() {
        //lineRenderer.SetPosition(0, pkmPlayer.transform.position );
        //lineRenderer.SetPosition(1, pkmOpponent.transform.position );
        pkmPlayer.transform.LookAt(pkmOpponent.transform.position); 
        pkmOpponent.transform.LookAt(pkmPlayer.transform.position); 
        //transform.rotation = Quaternion.Euler(transform.parent.gameObject.transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y,//transform.parent.gameObject.transform.rotation.eulerAngles.z);
        //transform.localEulerAngles = 
        //transform.rotation = new Quaternion(transform.parent.gameObject.transform.rotation.eulerAngles.x, transform.eulerAngles.y, transform.parent.gameObject.transform.rotation.eulerAngles.z);
        //Debug.Log(transform.eulerAngles.y);
        //transform.up = transform.parent.gameObject.transform.up;
        //transform.rotation = Quaternion.EulerAngles(0f, transform.rotation.y, 0f);
        pkmPlayer.transform.localEulerAngles = new Vector3(0f, pkmPlayer.transform.localRotation.eulerAngles.y, 0f);
        pkmOpponent.transform.localEulerAngles = new Vector3(0f, pkmOpponent.transform.localRotation.eulerAngles.y, 0f);
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
            return null;
        }
        return null;
    }

    public void randomOpponentAttack() {
        int rand = rnd.Next(2);
        if (rand == 0) {
            playerTakeDamage((int)(rnd.Next(3) + 1) / (rnd.Next(2) + 1) * pkmOpponentData.power1);
            pkmOppontentAnim.Attack1();
            if (pkmOppontentAnim.moveName1().Equals("")) {
            gameTeller.text = pkmOpponentName.text + " attacks miss!";
            } else {
                gameTeller.text = pkmOpponentName.text + " use " + pkmOppontentAnim.moveName1();
            }
        } else {
            playerTakeDamage((int)(rnd.Next(3) + 1) / (rnd.Next(2) + 1) * pkmOpponentData.power2);
            pkmOppontentAnim.Attack2();
            if (pkmOppontentAnim.moveName2().Equals("")) {
            gameTeller.text = pkmOpponentName.text + " attacks miss!";
            } else {
                gameTeller.text = pkmOpponentName.text + " use " + pkmOppontentAnim.moveName2();
            }
        }
        
        turn = 1 - turn;
    }

    int damageCal(int a1, int d1, int power) {
        return (int) (1.0 * (power * a1 / d1) / 50 + 2);
    }

    public void opponentTakeDamage(int power) {
        opponentPkmHealthCurrent -= damageCal(pkmOpponentData.attack, pkmOpponentData.def, power);
        float newHealth = (float)opponentPkmHealthCurrent/pkmOpponentData.hp;
        opponentHealth.fillAmount=newHealth;
        if (newHealth < 0.5 && newHealth >= 0.25) {
            opponentHealth.color = colorHeath1;
        } else if (newHealth < 0.25) {
            opponentHealth.color = colorHeath0;
        }
        opponentHealthText.text = Convert.ToString(max(opponentPkmHealthCurrent, 0)) + "/" + Convert.ToString(pkmOpponentData.hp);
        if (opponentPkmHealthCurrent <= 0) {
            pkmOpponent.GetComponent<Animator>().SetTrigger("isDie");
            loser = 1;
        }
    }

    void playerTakeDamage(int power) {
        playerPkmHealthCurrent -= damageCal(pkmPlayerData.attack, pkmPlayerData.def, power);
        float newHealth = (float)playerPkmHealthCurrent/pkmPlayerData.hp;
        playerHealth.fillAmount=newHealth;
        if (newHealth < 0.5 && newHealth >= 0.25) {
            playerHealth.color = colorHeath1;
        } else if (newHealth < 0.25) {
            playerHealth.color = colorHeath0;
        }
        playerHealthText.text = Convert.ToString(max(playerPkmHealthCurrent, 0)) + "/" + Convert.ToString(pkmPlayerData.hp);
        if (playerPkmHealthCurrent <= 0) {
            pkmPlayer.GetComponent<Animator>().SetTrigger("isDie");
            loser = 0;
        }
    }
    int max(int a, int b) {
        if (a>b) return a; return b;
    }

    public void exitGame() {
        // Save data
        pkmPlayerData.def = newDefense;
        pkmPlayerData.attack = newAttack;
        pkmPlayerData.hp = newHP;

        string json = JsonHelper.ToJson(_StaticData.player.lst.ToArray(), true);
        string filePath = Path.Combine(Application.persistentDataPath, gameDataFileName);
        File.WriteAllText(filePath, json);
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
}

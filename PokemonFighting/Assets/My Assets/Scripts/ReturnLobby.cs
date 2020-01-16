using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReturnLobby : MonoBehaviour
{
    public Text scoreText;
    // Start is called before the first frame update
    void Awake()
    {
        scoreText.text = "Your Score: " + PlayerPrefs.GetInt("score", 0).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ReturnToLobby()
    {
        SceneManager.LoadScene("Launcher");
    }
}

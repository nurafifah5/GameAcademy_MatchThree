using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //instance sebagai global access
    public static GameManager instance;
    int playerScore;
    public Text scoreText;

    // Singleton
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != null)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // update score dan ui
    public void GetScore(int point)
    {
        playerScore += point;
        scoreText.text = playerScore.ToString();
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    public TextMeshProUGUI ScoreText;

    // Start is called before the first frame update
    void Start()
    {
        int final_score = PlayerPrefs.GetInt("Score");
        string rank = "HEHEHE";

        if(final_score <= 500)
        {
            rank = "F";
        }
        else if(final_score <= 1000)
        {
            rank = "E";
        }
        else if (final_score <= 1500)
        {
            rank = "D";
        }
        else if(final_score <= 2000)
        {
            rank = "C";
        }
        else if (final_score <= 2500)
        {
            rank = "B";
        }
        else if (final_score <= 3000)
        {
            rank = "A";
        }
        else if (final_score <= 3500)
        {
            rank = "S";
        }
        else if (final_score <= 4000)
        {
            rank = "SS";
        }
        else if (final_score <= 5000)
        {
            rank = "SSS";
        }
        else 
        {
            rank = "SYSTEM ERROR";
        }
        ScoreText.text = "Score : " + final_score + "      " + rank;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}

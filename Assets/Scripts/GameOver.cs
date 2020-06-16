using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Text hud_score;

    // Start is called before the first frame update
    void Start()
    {
        int lastScore = PlayerPrefs.GetInt("lastScore");
        hud_score.text = lastScore.ToString();
        if (hud_score.text == null) hud_score.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

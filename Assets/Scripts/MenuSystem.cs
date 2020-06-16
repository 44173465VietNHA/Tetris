using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSystem : MonoBehaviour
{

    public void PlayAgain()
    {
        Game.ReTry();
        SceneManager.LoadScene("Level", LoadSceneMode.Single);
    }
}

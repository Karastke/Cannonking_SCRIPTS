using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{

    public int gameStartScene;

    public void GameStart()
    {
        SceneManager.LoadScene(gameStartScene);
    }

}

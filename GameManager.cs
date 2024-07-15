using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // 싱글톤을 할당할 전역 변수

    public bool isGameover = false; // 게임 오버 상태
    public Text scoreText; // 점수 출력할 UI 텍스트
    public GameObject gameoverUI; // 게임 오버시 활성화 할 UI 게임 오브젝트

    private int score = 0; // 게임 점수

    void Awake() {
        if (instance == null){//인스턴스가 비어있을 시 자기 자신 할당
            instance = this;
        }
        else{//싱글톤 오브젝트는 하나만 존재해야함
            Debug.LogWarning("씬에 두개 이상의 게임 매니저가 존재합니다!");
            Destroy(gameObject);
        }
    }

    void Update() {
        if(isGameover && Input.GetMouseButtonDown(0)){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    //점수를 증가시키는 메서드
    public void AddScore (int newScore) {
        if(isGameover){
            score += newScore;
            scoreText.text = "Score" + score;
        }
    }

    public void OnPlayerDead() {
        isGameover = true;
        gameoverUI.SetActive(true);
    }

}
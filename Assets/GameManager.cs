using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //게임메니저는 인스턴스화 시켜서 접근이 쉽게 만들어줌
    public static GameManager instance;

    // player
    [Header("Player")]
    public Car player;


    public float baseSpeed; // 랜덤 속도의 기준이 될 기본 속도

    [Header("GameObj")]
    public Car[] car; // 카트들을 담을 자리
    public Transform[] target; // 목적지들을 담을 배열

    [Header("Menu")]
    public GameObject startMenu;
    public GameObject selectMenu;

    //싱글톤 패턴, 이렇게 하면 다른 곳에서 쉽게 이것을 쓸 수 있음
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        SpeedSet(); // 시작할 때 이 기능도 함께 실행시킴
        player.player = true; // 플레이어 설정
    }
    // 속도를 랜덤으로 부여하는 기능
    void SpeedSet()
    {
        for(int i = 0; i < car.Length; i++)
        {
            car[i].carSpeed = Random.Range(baseSpeed, baseSpeed + 0.5f); // 0.5이상 차이 안나게
        }
    }

    public void StartBtn()
    {
        startMenu.SetActive(false);
        selectMenu.SetActive(true);
    }


}

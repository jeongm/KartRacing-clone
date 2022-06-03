using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //게임메니저는 인스턴스화 시켜서 접근이 쉽게 만들어줌
    public static GameManager instance;

    // player
    [Header("Player")]
    public Car player;

    public float baseSpeed; // 랜덤 속도의 기준이 될 기본 속도
    public int lap;
    public bool check;

    [Header("GameObj")]
    public Car[] car; // 카트들을 담을 자리
    public Transform[] target; // 목적지들을 담을 배열
    public Controller controllPad;
    public Transform cam;

    [Header("Menu")]
    public GameObject startMenu;
    public GameObject selectMenu;
    public GameObject ui;
    public GameObject finishMenu;

    [Header("Text")]
    public TextMeshProUGUI bestLapTimetext;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI curTimeText;
    public TextMeshProUGUI curSpeedText;
    public TextMeshProUGUI[] lapTimeText;

    float curTime;
    float bestLapTime;

    //싱글톤 패턴, 이렇게 하면 다른 곳에서 쉽게 이것을 쓸 수 있음
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        SpeedSet(); // 시작할 때 이 기능도 함께 실행시킴
        BestLapTimeSet();
    }

    public void GameStart()
    {
        StartCoroutine("StartCount");
    }

    void BestLapTimeSet()
    {
        bestLapTime = PlayerPrefs.GetFloat("BestLap");
        bestLapTimetext.text = string.Format("Best {0:00}:{1:00.00}",
                    (int)(bestLapTime / 60 % 60), bestLapTime % 60);

        // 게임 한 번도 안해서 best기록이 없음
        if (bestLapTime == 0)
            bestLapTimetext.text = "Best  -";
    }

    public void LapTime()
    {
        if(lap == 3)
        {
            SE_Manager.instance.PlaySound(SE_Manager.instance.goal);
            cam.parent = null; // 카트안에서 카메라 꺼냄, 더이상 카트 안따라가게
            StopCoroutine("Timer");// 타이머도 정지시킴
            finishMenu.SetActive(true);

            player.player = false;
            player.StartAI();
            player.transform.GetChild(3).gameObject.SetActive(false); // 재생중이던 사운드도 다 꺼줌

            if(curTime < bestLapTime | bestLapTime == 0)
            { // 신기록 바꿔주기
                bestLapTimetext.gameObject.SetActive(false);
                bestLapTimetext.text = string.Format("Best {0:00}:{1:00.00}",
                    (int)(curTime / 60 % 60), curTime % 60); 
                bestLapTimetext.gameObject.SetActive(true);

                // 게임 끄고 켜도 안사라져야 함
                PlayerPrefs.SetFloat("BestLap", curTime);
            }
        }

        lapTimeText[lap - 1].gameObject.SetActive(false);
        lapTimeText[lap-1].text =
            string.Format("{0:00}:{1:00.00}",
            (int)(curTime / 60 % 60), curTime % 60);
        lapTimeText[lap - 1].gameObject.SetActive(true);
    }

    IEnumerator StartCount()
    {
        selectMenu.SetActive(false);
        ui.SetActive(true);

        SE_Manager.instance.PlaySound(SE_Manager.instance.count[3]);
        countText.text = "3";
        countText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        SE_Manager.instance.PlaySound(SE_Manager.instance.count[2]);
        countText.gameObject.SetActive(false);
        countText.text = "2";
        countText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        SE_Manager.instance.PlaySound(SE_Manager.instance.count[1]);
        countText.gameObject.SetActive(false);
        countText.text = "1";
        countText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        SE_Manager.instance.PlaySound(SE_Manager.instance.count[0]);
        countText.gameObject.SetActive(false);
        countText.text = "GO!";
        countText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        countText.gameObject.SetActive(false);

        // controllPad.gameObject.gameObject.SetActive(true);
        player.player = true; // 플레이어 설정
        check = true;

        controllPad.StartController();
        for (int i = 0; i < car.Length; i++)
            car[i].StartAI();

        StartCoroutine("Timer");
    }

    IEnumerator Timer()
    {
        while(true)
        {
            curTime += Time.deltaTime;
            curTimeText.text = string.Format("{0:00}:{1:00.00}",
                (int)(curTime / 60 % 60), curTime % 60);
            yield return null;
        }
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
        
        SE_Manager.instance.PlaySound(SE_Manager.instance.btn);

        startMenu.SetActive(false);
        selectMenu.SetActive(true);
    }

    //재시작 버튼, 현재 씬을 다시 로드하는 방법을 사용
    public void ReStart()
    {
        SE_Manager.instance.PlaySound(SE_Manager.instance.btn);
        SceneManager.LoadScene("SampleScene");
    }
}

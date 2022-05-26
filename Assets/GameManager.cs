using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //게임메니저는 인스턴스화 시켜서 접근이 쉽게 만들어줌
    public static GameManager instance;

    public Transform[] target; // 목적지들을 담을 배열

    //싱글톤 패턴, 이렇게 하면 다른 곳에서 쉽게 이것을 쓸 수 있음
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }


}

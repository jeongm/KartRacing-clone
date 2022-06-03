using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Car : MonoBehaviour
{
    public float carSpeed;
    public Transform target;
    int nextTarget; // 목적지 순서
    public bool player; // 카트가 플레이어인지 체크

    public void StartAI()
    {
        // 지금까지 만든 기능이 플레이어가 아닐 때 실행되도록
        if(!player)
        {
            target = GameManager.instance.target[nextTarget];

            GetComponent<NavMeshAgent>().speed = carSpeed; // AI가 움직일 스피드를 car 스피드와 일치시킬거임
            StartCoroutine("AI_Move"); // 시작시 실행
            StartCoroutine("AI_Animation");
        }
        
    }
    
    //카트들이 스스로 레이싱하게 만들어 줌
    IEnumerator AI_Move() // AI 코르틴?
    {
        GetComponent<NavMeshAgent>().
            SetDestination(target.position); // 목적지로 AI 출발시킬거임, 목적지는 traget의 position
        
        while (true)
        {
            float dis = (target.position - transform.position).magnitude;

            if(dis <=1)
            {
                nextTarget += 1;

                if(nextTarget >= GameManager.instance.target.Length)
                        nextTarget = 0;

                target = GameManager.instance.target[nextTarget];
                GetComponent<NavMeshAgent>().
                    SetDestination(target.position); // 목적지로 AI 출발시킬거임, 목적지는 traget의 position
            }

            yield return null;
        }
    }

    // 애니매이션을 재생시켜 줄 코르틴
    IEnumerator AI_Animation()
    {
        Vector3 lastPosition;

        while(true)
        {
            lastPosition = transform.position; // 카트의 위치 넣어줌
            yield return new WaitForSecondsRealtime(0.03f);// 0.03초 후에 현재 포지션과 lastposition을 비교
                
            if ((lastPosition - transform.position).magnitude > 0)
            {
                Vector3 dir = transform.InverseTransformPoint(lastPosition);
                if(dir.x >= -0.01f && dir.x <= 0.01f)
                    GetComponent<Animator>().Play("Ani_Forward");
                if (dir.x < -0.01f)
                    GetComponent<Animator>().Play("Ani_Right");
                if (dir.x > 0.01f)
                    GetComponent<Animator>().Play("Ani_Left");
            }
            if ((lastPosition - transform.position).magnitude <= 0)
                GetComponent<Animator>().Play("Ani_Idle");

            yield return null;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(player)
        {
            if(other.gameObject.tag=="Finish")
            {
                if(GameManager.instance.check)
                {
                    GameManager.instance.check = false;

                    if (GameManager.instance.lap > 0)
                    { // 한 바퀴 돌 때마다 작동 
                        SE_Manager.instance.PlaySound(SE_Manager.instance.lap);
                        GameManager.instance.LapTime();
                    }
                    GameManager.instance.lap += 1;
                }
                
            }
            if(other.gameObject.tag == "CheckPoint")
            {
                GameManager.instance.check = true;
            }
        }
    }
}

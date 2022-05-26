using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Controller : MonoBehaviour, IPointerUpHandler,
    IPointerDownHandler, IDragHandler
{
    public RectTransform pad;
    public RectTransform stick;
    Vector3 playerRotate;
    Car player;
    Animator playerAni;
    bool onMove; // 이동중인지 확인
    float playerSpeed;

    private void Start()
    {
        player = GameManager.instance.player;
        playerAni = player.GetComponent<Animator>(); // 캐릭터에 애니매이션 되어있는거 받기
        StartCoroutine("PlayerMove");
    }

    public void OnDrag(PointerEventData eventData)
    {
        stick.position = eventData.position;
        stick.localPosition =
            Vector3.ClampMagnitude(eventData.position - (Vector2)pad.position, pad.rect.width * 0.5f);

        playerRotate = new Vector3(0, stick.localPosition.x, 0).normalized;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        stick.localPosition = Vector3.zero;
        playerRotate = Vector3.zero;
    }

    // Accel버튼 누르고 있을 떄
    public void OnMove()
    {
        StartCoroutine("Acceleration");
        onMove = true;
    }
    // 버튼에서 손 땔 때
    public void OffMove()
    {
        StartCoroutine("Braking");
    }

    IEnumerator PlayerMove()  // 스틱 조종, 변경바람
    {
        while(true)
        {
            if (onMove)
            {
                player.transform.Translate(Vector3.forward * playerSpeed
                    * Time.deltaTime);
                if (Mathf.Abs(stick.localPosition.x) > pad.rect.width * 0.2f) // 스틱이 움직였을 때
                    player.transform.Rotate(playerRotate * 30 * Time.deltaTime); // 플레이어 회전시킴

                // 애니매이션 재생
                if (Mathf.Abs(stick.localPosition.x) <= pad.rect.width * 0.2f)
                    playerAni.Play("Ani_Forward");
                if (stick.localPosition.x > pad.rect.width * 0.2f)
                    playerAni.Play("Ani_Right");
                if (stick.localPosition.x < pad.rect.width * -0.2f)
                    playerAni.Play("Ani_Left");

                player.transform.GetChild(3).gameObject.SetActive(true);
                player.transform.GetChild(4).gameObject.SetActive(false);
            }
            if (!onMove)
            {
                playerAni.Play("Ani_Idle");
                player.transform.GetChild(3).gameObject.SetActive(false);
                player.transform.GetChild(4).gameObject.SetActive(true);
            }
            yield return null;
        }
    }
    IEnumerator Acceleration()
    {
        StopCoroutine("Braking"); // 엑셀기능 실행될 때는 감속기능 멈춰야 함

        while (true)
        {
            playerSpeed += 7 * Time.deltaTime;
            
            //플레이어 스피드가 점점 증가하다가 카트의 속도보다 커지면 플레이어 스피드 낮춤
            // 카트의 속도가 최고속도에 도달하면 그 언저리 속도 유지
            if (playerSpeed >= player.carSpeed)
                playerSpeed -= 10 * Time.deltaTime;

            yield return null;
        }
    }
    // 엑셀에서 손 땠을 때 감속
    IEnumerator Braking()
    {
        StopCoroutine("Acceleration");

        while(true)
        {
            playerSpeed -= 7 * Time.deltaTime;

            if(playerSpeed <= 0)
            {
                playerSpeed = 0;
                onMove = false;
                StopCoroutine("Braking");
            }

            yield return null;
        }
    }

    

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    private void Update() // Accel, leftshift키
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            OnMove();
        if (Input.GetKeyUp(KeyCode.LeftShift))
            OffMove();
    }
    
}

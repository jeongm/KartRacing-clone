using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectMenu : MonoBehaviour, IPointerDownHandler
{
    public Camera cam;
    public GameObject finalCheckMenu;
    RaycastHit hit;
    bool checking;
    public void OnPointerDown(PointerEventData eventData)
    {
        if(!checking)
        {
            Ray ray = cam.ScreenPointToRay(eventData.position);
            Physics.Raycast(ray, out hit);

            if(hit.transform.gameObject.tag == "Car")
            {
                checking = true;
                cam.transform.SetParent(hit.transform);
                StopCoroutine("Cam_ZoomOut");
                StartCoroutine("Cam_ZoomIn");
                finalCheckMenu.SetActive(true);
            }
        }
    }

    IEnumerator Cam_ZoomIn()
    {
        while(true)
        {
            cam.transform.localPosition =
                Vector3.Slerp(cam.transform.localPosition,
                new Vector3(0, 2, -3.5f), 20 * Time.deltaTime);

            if (cam.transform.localPosition.z >= -3.5f)
                StopCoroutine("Cam_ZoomIn");
            yield return null;
        }
    }

    public void CancleBtn()
    {
        StopCoroutine("Cam_ZoomIn");
        StartCoroutine("Cam_ZoomOut");
        finalCheckMenu.SetActive(false);
        checking = false;
        
    }

    IEnumerator Cam_ZoomOut()
    {
        while (true)
        {
            cam.transform.localPosition =
                Vector3.Slerp(cam.transform.localPosition,
                new Vector3(0, 3, -5f), 20 * Time.deltaTime);

            if (cam.transform.localPosition.z <= -5f)
            {
                StopCoroutine("Cam_ZoomOut");
            }
                
            yield return null;
        }
    }

}

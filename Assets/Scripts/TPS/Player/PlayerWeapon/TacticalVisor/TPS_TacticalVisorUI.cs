using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPS_TacticalVisorUI : MonoBehaviour
{

    float sizeX;
    bool isRun;
    bool isOff;
    private void Awake()
    {
        sizeX = transform.localScale.x;
    }

    private void OnEnable()
    {
        var size = transform.localScale;
        size.x = 0f;
        transform.localScale = size;

        isRun = true;

        isOff = false;
    }

    public void OffTacticalVisorAim()
    {
        isOff = true;
    }

    [SerializeField]
    float sizeUpSpeed;
    private void Update()
    {
        if(isRun)
        {
            var plusScale = new Vector3(Time.deltaTime * sizeUpSpeed, 0f, 0f);

            transform.localScale += plusScale;

            if (sizeX <= transform.localScale.x)
            {
                transform.localScale = new Vector3(sizeX, transform.localScale.y, transform.localScale.z);
                isRun = false;
            }
        }
        else if(isOff)
        {
            var plusScale = new Vector3(Time.deltaTime * sizeUpSpeed, 0f, 0f);

            transform.localScale -= plusScale;

            if (transform.localScale.x <= 0)
            {
                isOff = false;
                gameObject.SetActive(false);
            }
        }
    }
   
}

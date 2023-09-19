using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TPS_SoldierHealth : MonoBehaviour
{
    float hP;
    float maxHP = 100f;
    float hpUITimer = 0f;
    float hitedCoolTime = 0f;

    GameObject hpUiParent;
    [SerializeField] Image hpUi;
    TPS_SoldierController soldierController;
    [SerializeField] GameObject ItemBox;

    // Start is called before the first frame update
    void Start()
    {
        hP = maxHP;
        hpUi.fillAmount = hP * 0.01f;
        soldierController = GetComponent<TPS_SoldierController>();

        hpUiParent = hpUi.gameObject.transform.parent.gameObject;
        hpUiParent.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        RunCoolTime();
    }

    void RunCoolTime()
    {
        if (0f < hpUITimer)
        {
            hpUITimer -= Time.deltaTime;
            if (hpUITimer <= 0f)
                hpUiParent.gameObject.SetActive(false);
        }

        if (0f < hitedCoolTime)
            hitedCoolTime -= Time.deltaTime;
    }


    public void TakeDamage(GameObject target, float damage)
    {
        if (soldierController.myState.getCurrentState() == StateMachine.EnumState.DEATH)
            return;
        if (hP <= 0)
            return;

        hpUiParent.gameObject.SetActive(true);
        hpUITimer = 3f;

        hP -= damage;
        hpUi.fillAmount = hP * 0.01f;

        if (soldierController.target == null)
        {
            soldierController.target = target;
        }

        if(hitedCoolTime <= 0f )
        {
            hitedCoolTime = 1f;
            soldierController.Hited();
        }

        if (hP <= 0)
        {
            Invoke("ShowItemBox", 2f);
            soldierController.myState.changeState(StateMachine.EnumState.DEATH);
        }

    }
    
    void ShowItemBox()
    {
        ItemBox.SetActive(true);
        ItemBox.transform.parent = transform.parent;
    }
  
}

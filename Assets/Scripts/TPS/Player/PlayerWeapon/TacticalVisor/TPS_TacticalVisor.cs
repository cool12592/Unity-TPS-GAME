using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TPS_TacticalVisor : MonoBehaviour
{
    //TV = TactivalVisor

    public LayerMask targetMask;

    public bool isOn = false;
    float tvTimer;
    const float tvTimerOrigin = 8f;
    bool isTvRun = false;
    public float viewDistance;

    [SerializeField]
    TPS_TacticalVisorUI tvScope; 
   
    [SerializeField]
    Image tvTimerUI;
    [SerializeField]
    Text tvExpiredText;
    [SerializeField]
    GameObject visor;
    [SerializeField]
    GameObject invenCharacterVisor1, invenCharacterVisor2;

    [HideInInspector] public TPS_SoldierController minDistEnemy = null;
    float minDist = 100000f;

    [SerializeField] 
    float tvAimSpeed;

    [HideInInspector] public PlayerController2 pc;

    private void Awake()
    {
        pc = GetComponent<PlayerController2>();
    }

    public void SetTacticalVisor(bool b)
    {
        isOn = b;
        visor.SetActive(b);
        invenCharacterVisor1.SetActive(b);
        invenCharacterVisor2.SetActive(b);

        if (b)
            tvTimer = tvTimerOrigin;
    }

    public void SetTacticalVisorAim(bool b)
    {
        if (b)
        {
            StartTacticalVisorAim();
        }
        else
        {
            EndTacticalVisorAim();
        }
    }

    void StartTacticalVisorAim()
    {
        tvScope.gameObject.SetActive(true);
        tvTimerUI.transform.parent.gameObject.SetActive(true);
        isTvRun = true;
    }

    void EndTacticalVisorAim()
    {
        tvTimerUI.transform.parent.gameObject.SetActive(false);
        tvScope.OffTacticalVisorAim();
        isTvRun = false;

        minDistEnemy = null;
        Collider[] targets = Physics.OverlapSphere(transform.position, viewDistance, targetMask);

        for (int i = 0; i < targets.Length; i++)
        {
            Transform target = targets[i].transform;

            if (target.gameObject.tag != "Enemy")
                continue;

            target.GetComponent<TPS_SoldierController>().tactical_UI.transform.GetChild(2).gameObject.SetActive(false);
            target.GetComponent<TPS_SoldierController>().tactical_UI.SetActive(false);
        }
    }

    private void Update()
    {
        CheckTacticalVisor();
    }

    void CheckTacticalVisor()
    {

        if (isTvRun == false)
            return;

        TacticalVisorRun();
        TacticalVisorTimer();
        TacticalVisorAttack();
    }

    void TacticalVisorRun()
    {
        if (minDistEnemy != null)
            minDistEnemy.tacticalTarget_UI.SetActive(false);

        minDist = 100000f;
        //시야거리 내에 존재하는 모든 컬라이더 받아오기
        Collider[] targets = Physics.OverlapSphere(transform.position, viewDistance, targetMask);

        for (int i = 0; i < targets.Length; i++)
        {
            Transform target = targets[i].transform;

            if (target.gameObject.tag != "Enemy")
                continue;

            //뒤에있는애들은 해당x
            if (Vector3.Dot(target.transform.position - pc.myCamera.transform.position, pc.myCamera.transform.forward) <= 0f)
                continue;

            var targetPosInScreen = Camera.main.WorldToScreenPoint(target.transform.position);
            var dist = Vector2.Distance(targetPosInScreen, new Vector2(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2));

            if (dist < Camera.main.pixelWidth * 0.2f)
            {
                var distance = Vector3.Distance(pc.myCamera.transform.position, target.transform.position);
                var dir = (target.transform.position+Vector3.up*1.5f) - pc.myCamera.transform.position;
                RaycastHit hit;
                if (Physics.Raycast(pc.myCamera.transform.position, dir, out hit, distance))
                {
                    if (hit.collider.gameObject != target.gameObject)
                        continue;
                }

                target.GetComponent<TPS_SoldierController>().tactical_UI.SetActive(true);

                if (dist < minDist)
                {
                    minDist = dist;
                    minDistEnemy = target.GetComponent<TPS_SoldierController>();
                }
            }
            else
                target.GetComponent<TPS_SoldierController>().tactical_UI.SetActive(false);


        }

        if (minDistEnemy != null)
            minDistEnemy.tacticalTarget_UI.gameObject.SetActive(true);
    }

    void TacticalVisorAttack()
    {
        if (pc.isAttacking)
        {
            if (minDistEnemy != null)
            {
                pc.myCamera.autoMove = false;
                var lookDirection = minDistEnemy.transform.position - pc.myCamera.transform.position;
                lookDirection += Vector3.up;
                pc.myCamera.transform.rotation = Quaternion.RotateTowards(pc.myCamera.transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * tvAimSpeed);
            }
        }
    }

    void TacticalVisorTimer()
    {
        tvTimer -= Time.deltaTime;
        tvTimerUI.fillAmount = tvTimer / tvTimerOrigin;
        if (tvTimer <= 0f)
        {
            tvExpiredText.gameObject.SetActive(true);
            SetTacticalVisorAim(false);
            SetTacticalVisor(false);

            pc.playerAttack.weaponParts.UnEquipParts(Item.PartsType.AR_Scope);
            pc.inven.RemoveItem(14, 1);
            pc.playerAttack.weaponParts.CreateARSopeParts();

            if (pc.isActive)
                pc.myState.changeState(StateMachine.EnumState.IDLE);
        }
    }
}

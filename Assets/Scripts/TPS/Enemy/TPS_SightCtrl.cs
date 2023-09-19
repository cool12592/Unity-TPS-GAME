using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPS_SightCtrl : MonoBehaviour
{
    public float viewAngle;    //시야각
    public float viewDistance; //시야거리

    public LayerMask targetMask;    
    public LayerMask obstacleMask; 

    public float cosViewAngle;

    TPS_SoldierController soldierController;

    [SerializeField]
    float minDist;

    [SerializeField]
    float heightOffset;

    Vector3 upperOffest;


    void Awake()
    {
        soldierController = GetComponent<TPS_SoldierController>();
        cosViewAngle = Mathf.Cos((viewAngle / 2) * Mathf.Deg2Rad);
    }

    void Update()
    {
        DrawView();             
    }

    public Vector3 DirFromAngle(float angleInDegrees)
    {
        //탱크의 좌우 회전값 갱신
        angleInDegrees += transform.eulerAngles.y;
        //경계 벡터값 반환
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void DrawView()
    {
        Vector3 leftBoundary = DirFromAngle(-viewAngle / 2);
        Vector3 rightBoundary = DirFromAngle(viewAngle / 2);
        Debug.DrawLine(transform.position, transform.position + leftBoundary * viewDistance, Color.blue);
        Debug.DrawLine(transform.position, transform.position + rightBoundary * viewDistance, Color.blue);
    }

    public bool FindVisibleTargets()
    {
        //시야거리 내에 존재하는 모든 컬라이더 받아오기
        Collider[] targets = Physics.OverlapSphere(transform.position, viewDistance, targetMask);

        for (int i = 0; i < targets.Length; i++)
        {
            Transform target = targets[i].transform;

            if (target.gameObject.tag != "Player")
                continue;

            bool isMinDist = false;
            //최소거리보다 짧으면 시야각 생략
            if (Vector3.Distance(target.position, transform.position) <= minDist)
                isMinDist = true;

            if (isMinDist == false && CheckViewAngleToTarget(target)) //시야각 확인
                continue;

            if (RayCastToTarget(target)) //레이케스트 확인
                return true;
           
        }

        return false;
    }

    public bool CheckViewAngleToTarget(Transform target)
    {
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        if (Vector3.Dot(transform.forward, dirToTarget) < cosViewAngle) //시야각 확인
            return true;

        return false;
    }


    public bool RayCastToTarget(Transform target)
    {
        upperOffest = Vector3.up * heightOffset;
        Vector3 dirToTarget = ( (target.position + upperOffest) - (transform.position+ upperOffest) ).normalized;

        float distToTarget = Vector3.Distance(transform.position, target.position);

        //Raycast에 ObstacleMask 가 걸릴경우 true를 반환
        bool res = Physics.Raycast(transform.position + upperOffest, dirToTarget, distToTarget, obstacleMask);
        if (res == false)
        {
            SuccessFindTarget(target);
            return true;
        }

        return false;
    }

    void SuccessFindTarget(Transform target)
    {
        Debug.DrawLine(transform.position + upperOffest, target.position+ upperOffest, Color.red);
        soldierController.StartAttack(target.gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPS_SightCtrl : MonoBehaviour
{
    public float viewAngle;    //�þ߰�
    public float viewDistance; //�þ߰Ÿ�

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
        //��ũ�� �¿� ȸ���� ����
        angleInDegrees += transform.eulerAngles.y;
        //��� ���Ͱ� ��ȯ
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
        //�þ߰Ÿ� ���� �����ϴ� ��� �ö��̴� �޾ƿ���
        Collider[] targets = Physics.OverlapSphere(transform.position, viewDistance, targetMask);

        for (int i = 0; i < targets.Length; i++)
        {
            Transform target = targets[i].transform;

            if (target.gameObject.tag != "Player")
                continue;

            bool isMinDist = false;
            //�ּҰŸ����� ª���� �þ߰� ����
            if (Vector3.Distance(target.position, transform.position) <= minDist)
                isMinDist = true;

            if (isMinDist == false && CheckViewAngleToTarget(target)) //�þ߰� Ȯ��
                continue;

            if (RayCastToTarget(target)) //�����ɽ�Ʈ Ȯ��
                return true;
           
        }

        return false;
    }

    public bool CheckViewAngleToTarget(Transform target)
    {
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        if (Vector3.Dot(transform.forward, dirToTarget) < cosViewAngle) //�þ߰� Ȯ��
            return true;

        return false;
    }


    public bool RayCastToTarget(Transform target)
    {
        upperOffest = Vector3.up * heightOffset;
        Vector3 dirToTarget = ( (target.position + upperOffest) - (transform.position+ upperOffest) ).normalized;

        float distToTarget = Vector3.Distance(transform.position, target.position);

        //Raycast�� ObstacleMask �� �ɸ���� true�� ��ȯ
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

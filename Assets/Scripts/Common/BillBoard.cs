using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    Transform camTr;

    [SerializeField] Transform target;
    [SerializeField] float offset;
    [SerializeField] bool isMinimapBillBoard = false;
    [SerializeField] bool isSpineBillBoard = false;

    // Start is called before the first frame update
    void Start()
    {
        camTr = Camera.main.transform;

        if(isSpineBillBoard)
            target = target.gameObject.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Spine);

    }

    // Update is called once per frame
    void Update()
    {
        if (isMinimapBillBoard) 
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, (target.eulerAngles.y + offset), transform.eulerAngles.z);
            return;
        }

        transform.LookAt(camTr.position);
    }
}

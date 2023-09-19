using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class TPS_PlayerAttack : MonoBehaviour
{
    [HideInInspector] public PlayerController2 pc;
    TPS_CameraShake cameraShake;

    [HideInInspector]
    public TPS_WeaponParts weaponParts;
    [HideInInspector]
    public TPS_PlayerWeapon playerWeapon;


 
    [Header("카메라 에임 옵셋")]

    [SerializeField] float aimRightOffset;
    [SerializeField] float aimDistance;
    [SerializeField] float aimHeight;

    float originRightOffset, originDistance, originHeight;

    [HideInInspector]
    public TPS_TacticalVisor tacticalVisor;


    private void Awake()
    {
        pc = GetComponent<PlayerController2>();
        playerWeapon = GetComponent<TPS_PlayerWeapon>();
        weaponParts = GetComponent<TPS_WeaponParts>();
        tacticalVisor = GetComponent<TPS_TacticalVisor>();

    }

    private void Start()
    {
        cameraShake = pc.myCamera.GetComponent<TPS_CameraShake>();
        originRightOffset = pc.myCamera.rightOffset;
        originDistance = pc.myCamera.defaultDistance;
        originHeight = pc.myCamera.height;
    }
  
    public void StartAimMode()
    {
        pc.isAiming = true;
        pc.cc.input = Vector3.zero;
        pc.anim.SetBool("isAiming", true);
        SetCameraOffset(aimRightOffset, aimDistance, aimHeight);
    }

    public void EndAimMode()
    {
        EndAttackInput();
        pc.anim.SetBool("isAiming", false);

        pc.isAiming = false;
        SetCameraOffset(originRightOffset, originDistance, originHeight);
    }

    public void EndAttackInput()
    {
        pc.anim.SetBool("isAttackAR", false);
        pc.isAttacking = false;
        cameraShake.EndShake();

        EndAttack();
    }

    void SetCameraOffset(float rightOffset, float distance, float height)
    {
        pc.myCamera.rightOffset = rightOffset;
        pc.myCamera.defaultDistance = distance;
        pc.myCamera.height = height;
    }

    public void StartFpsMode()
    {
        playerWeapon.nowWeapon_.StartFpsMode();
    }

    public void EndFpsMode()
    {
        playerWeapon.nowWeapon_.EndFpsMode();
    }

    public void StartAttack()
    {
        playerWeapon.nowWeapon_.StartAttack();
    }

    public void Attack()
    {
        playerWeapon.nowWeapon_.Attack();
    }
    public void EndAttack()
    {
        playerWeapon.nowWeapon_.EndAttack();
    }


   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TPS_PlayerWeapon : MonoBehaviour
{
    [HideInInspector] public PlayerController2 pc;

    public enum WeaponType { AR, SR };
    public WeaponType nowWeaponType;

    public TPS_BasicWeapon nowWeapon_ { get; private set; }
    public TPS_BasicWeapon weaponAR;
    public TPS_BasicWeapon weaponSR;

    [SerializeField]
    Animator ARFpsAnim;

    private void Awake()
    {
        pc = GetComponent<PlayerController2>();
        nowWeaponType = WeaponType.AR;

        nowWeapon_ = weaponAR;
    }

    public void ChangeWeaponType()
    {
        pc.anim.SetTrigger("TDraw");
    }

    void LoadWeapon()
    {
        nowWeapon_.LoadWeapon();

        if (nowWeaponType == WeaponType.AR)
        {
            nowWeaponType = WeaponType.SR;
            nowWeapon_ = weaponSR;
        }
        else if (nowWeaponType == WeaponType.SR)
        {
            nowWeaponType = WeaponType.AR;
            nowWeapon_ = weaponAR;

        }  
    }

    void DrawWeapon()
    {
        nowWeapon_.DrawWeapon();
    }

   
    public void InitBullet()
    {
        weaponAR.InitBullet();
        weaponSR.InitBullet();

        nowWeapon_.InitBullet();
    }

    public void ReLoad()
    {
        if (nowWeapon_.allRemainBullet <= 0)
            return;

        pc.playerAttack.EndAttackInput();
        SoundManager.Instance.GetSoundEffect(SoundManager.soundEnum.m4_reload).Play();

        pc.anim.SetTrigger("TReload");
        ARFpsAnim.SetTrigger("TReload");

    }

    void ReloadImp()
    {
        nowWeapon_.Reload();
    }
}

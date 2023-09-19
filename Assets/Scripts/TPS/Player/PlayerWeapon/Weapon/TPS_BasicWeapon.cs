using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class TPS_BasicWeapon : MonoBehaviour
{
    [SerializeField]
    protected PlayerController2 pc;
    protected TPS_PlayerWeapon playerWeapon;
    protected TPS_PlayerAttack playerAttack;
    protected TPS_WeaponParts weaponParts;
    protected Camera cameraMain;

    [SerializeField] protected float myDamage;


    
    [SerializeField]
    GameObject invenChar;

    [Header("위치 관련")]
    [SerializeField] GameObject drawPos;
    [SerializeField] GameObject loadPos;

    protected Transform nowFirePos;
    [SerializeField]
    protected Transform firePosThirdAim;
    [SerializeField]
    protected Transform firePosFirstAim;

    [Header("이펙트")]
    [SerializeField]
    protected ObjectPool attackEffectPool;
    [SerializeField]
    protected ObjectPool muzzleEffectPool;


    [Header("총알 관련")]
    [SerializeField]
    Text remainBulletText;
    public Image remainBulletGunIcon;

    [HideInInspector] public int remainBullet;
    [HideInInspector] public int allRemainBullet;
    [SerializeField] protected int maxBullet;



    [Header("스코프관련")]

    [SerializeField]
    protected GameObject aimUI;
    [SerializeField]
    protected GameObject scopeUI;
    [SerializeField]
    protected GameObject specialAimBackGround;
    protected TPS_CameraShake cameraShake;

    [SerializeField]
    public int itemID;


    public abstract void StartFpsMode();
    public abstract void EndFpsMode();

    public abstract void StartAttack();
    public abstract void Attack();
    public abstract void EndAttack();

    public abstract int GetMaxBullet();

    private void Awake()
    {
        playerWeapon = pc.GetComponent<TPS_PlayerWeapon>();
        playerAttack = pc.GetComponent<TPS_PlayerAttack>();
        weaponParts = pc.GetComponent<TPS_WeaponParts>();

        cameraMain = Camera.main;
        cameraShake = cameraMain.GetComponent<TPS_CameraShake>();

        nowFirePos = firePosThirdAim;
    }


    public void LoadWeapon()
    {
        SoundManager.Instance.GetSoundEffect(SoundManager.soundEnum.weaponChange).time = 0.5f;
        SoundManager.Instance.GetSoundEffect(SoundManager.soundEnum.weaponChange).Play();

        
        invenChar.SetActive(false);
        remainBulletGunIcon.gameObject.SetActive(false);

        transform.parent = loadPos.transform.parent;
        transform.position = loadPos.transform.position;
        transform.rotation = loadPos.transform.rotation;
    }

    public void DrawWeapon()
    {
        invenChar.SetActive(true);
        remainBulletGunIcon.gameObject.SetActive(true);

        transform.parent = drawPos.transform.parent;
        transform.position = drawPos.transform.position;
        transform.rotation = drawPos.transform.rotation;

        ChangeRemainBullet();

    }


    protected bool CheckBullet()
    {
        if (remainBullet <= 0)
            return false;

        return true;
    }

    public bool ConsumeBullet()
    {
        if (remainBullet <= 0)
            return false;

        remainBullet--;
        remainBulletText.text = remainBullet + " / " + allRemainBullet;
        return true;
    }

    public void ChangeRemainBullet(int addBullet=0)
    {
        remainBullet += addBullet;
        allRemainBullet = pc.inven.GetInvenRemainBullet(itemID);
        remainBulletText.text = remainBullet + " / " + allRemainBullet;
    }

    public void Reload()
    {
        ChangeRemainBullet(pc.inven.ChargeBullet(itemID, remainBullet, GetMaxBullet()));
    }

    public void InitBullet()
    {
        remainBullet = GetMaxBullet();
        Reload();
    }


    

}

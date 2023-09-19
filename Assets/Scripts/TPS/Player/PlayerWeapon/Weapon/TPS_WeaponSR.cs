using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPS_WeaponSR : TPS_BasicWeapon
{

    public override void StartFpsMode()
    {
        if (weaponParts.isEquipScope_SR == false)
        {
            pc.myState.changeState(StateMachine.EnumState.IDLE);
            return;
        }

        if (weaponParts.isSpecialScope_SR)
        {
            specialAimBackGround.SetActive(true);
        }

        cameraMain.cullingMask = cameraMain.cullingMask & ~(1 << LayerMask.NameToLayer("Player"));
        cameraMain.fieldOfView = 20f;

        nowFirePos = firePosFirstAim;


        aimUI.SetActive(false);
        scopeUI.SetActive(true);

        playerAttack.StartAimMode();
        cameraShake.StartSmallShake();
    }

    public override void EndFpsMode()
    {
        if (weaponParts.isSpecialScope_SR)
        {
            specialAimBackGround.SetActive(false);
        }

        cameraMain.cullingMask |= 1 << LayerMask.NameToLayer("Player");
        cameraMain.fieldOfView = 60f;

        nowFirePos = firePosThirdAim;

        aimUI.SetActive(true);
        scopeUI.SetActive(false);
        cameraShake.StopSmallShake();
        playerAttack.EndAimMode();
    }


    public override void StartAttack()
    {
        if (CheckBullet() == false)
            return;

        SoundManager.Instance.GetSoundEffect(SoundManager.soundEnum.awm_attack).time = 0.05f;
        SoundManager.Instance.GetSoundEffect(SoundManager.soundEnum.awm_attack, 0.2f).Play();

        pc.isAttacking = true;
        Attack();
        cameraShake.StartShake_SR(weaponParts.isCompensator_SR);
    }

    public override void Attack()
    {
        if (pc.isAttacking == false)
            return;

        if (ConsumeBullet() == false)
        {
            playerAttack.EndAttackInput();
            return;
        }
        Transform pos = nowFirePos;
   
        var muzzle = muzzleEffectPool.GetObject();
        muzzle.transform.position = pos.position;
        muzzle.transform.rotation = pos.rotation;
        muzzle.transform.parent = pos.transform;

        var ray = cameraMain.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            var target = hit.collider.gameObject;

            var attackEffect = attackEffectPool.GetObject();
            attackEffect.transform.position = hit.point;
            attackEffect.transform.rotation = Quaternion.identity;

            if (target.tag == "EnemyWeakPoint")
            {
                Debug.Log("Çìµå¼¦");
                target.transform.parent.GetComponent<TPS_SoldierHealth>().TakeDamage(gameObject, 100f);
            }
            else if (target.tag == "Enemy")
            {
                target.GetComponent<TPS_SoldierHealth>().TakeDamage(gameObject, myDamage);
            }

        }
    }

    public override void EndAttack()
    {
    }



    public override int GetMaxBullet()
    {
        if(weaponParts.isLargetCapacity_SR)
            return (int)Mathf.Ceil(maxBullet * 1.5f);

        return maxBullet;
    }
}

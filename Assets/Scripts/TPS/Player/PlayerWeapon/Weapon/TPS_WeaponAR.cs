using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TPS_WeaponAR : TPS_BasicWeapon
{
    public override void StartFpsMode()
    {
        if (weaponParts.isEquipScope_AR == false)
        {
            pc.myState.changeState(StateMachine.EnumState.IDLE);
            return;
        }

        aimUI.SetActive(false);
        cameraMain.cullingMask = cameraMain.cullingMask & ~(1 << LayerMask.NameToLayer("Player"));
        cameraShake.StartSmallShake();
        SettingScopeUIPos();

        playerAttack.StartAimMode();

        nowFirePos = firePosFirstAim;

        if (weaponParts.isSpecialScope_AR)
            specialAimBackGround.SetActive(true);
    }

    void SettingScopeUIPos()
    {
        var aimObjectPos = scopeUI.transform.localPosition;
        if (playerAttack.tacticalVisor.isOn)
        {
            scopeUI.transform.localPosition = new Vector3(aimObjectPos.x, aimObjectPos.y, 0.44f);
            playerAttack.tacticalVisor.SetTacticalVisorAim(true);
        }
        else
            scopeUI.transform.localPosition = new Vector3(aimObjectPos.x, aimObjectPos.y, 0.35f);

        scopeUI.SetActive(true);
    }

    public override void EndFpsMode()
    {
        if (playerAttack.tacticalVisor.isOn)
        {
            playerAttack.tacticalVisor.SetTacticalVisorAim(false);
        }

        if (weaponParts.isSpecialScope_AR)
        {
            specialAimBackGround.SetActive(false);
        }

        cameraMain.cullingMask |= 1 << LayerMask.NameToLayer("Player");
        nowFirePos = firePosThirdAim;

        aimUI.SetActive(true);
        scopeUI.SetActive(false);
        cameraShake.StopSmallShake();
        playerAttack.EndAimMode();
    }


    public override void StartAttack()
    {
        if (CheckBullet() == false)
        {
            pc.anim.SetBool("isAttackAR", false);
            return;
        }
        pc.anim.SetBool("isAttackAR", true);
        pc.isAttacking = true;

        if(pc.myState.getCurrentState()==StateMachine.EnumState.FIRSTAIM)
            cameraShake.StartShake_FirstAR(weaponParts.isCompensator_AR);
        else
            cameraShake.StartShake_ThirdAR(weaponParts.isCompensator_AR);

        SoundManager.Instance.GetSoundEffect(SoundManager.soundEnum.m4_attack, 0.1f).Play();
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
        RaycastHit hit = Physics.RaycastAll(ray).OrderBy(h => h.distance).Where(h => h.transform.tag != "Player").FirstOrDefault();
        if (hit.transform != null)
        {
            var attackEffect = attackEffectPool.GetObject();
            attackEffect.transform.position = hit.point;
            attackEffect.transform.rotation = Quaternion.identity;

            var target = hit.collider.gameObject;
            if (target.tag == "EnemyWeakPoint")
            {
                Debug.Log("Çìµå¼¦");
                target.transform.parent.GetComponent<TPS_SoldierHealth>().TakeDamage(gameObject, myDamage * 2f);
            }
            else if (target.tag == "Enemy")
            {
                target.GetComponent<TPS_SoldierHealth>().TakeDamage(gameObject, myDamage);
            }
        }
    }

    public override void EndAttack()
    {
        SoundManager.Instance.AddFadeOutSound(SoundManager.soundEnum.m4_attack);

        if(pc.myState.getCurrentState() == StateMachine.EnumState.FIRSTAIM)
            RemoveAllMuzze(firePosFirstAim);
        else if(pc.myState.getCurrentState() == StateMachine.EnumState.THIRDAIM)
            RemoveAllMuzze(firePosThirdAim);

        if (pc.myCamera.autoMove == false)
        {
            pc.myCamera.SetMainTarget(pc.gameObject.transform);
            pc.myCamera.autoMove = true;
        }

    }

    void RemoveAllMuzze(Transform firePos)
    {
        Transform[]  allChildren = firePos.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.tag == "Effect" && child.gameObject.activeSelf)
                muzzleEffectPool.ReturnObject(child.gameObject);
        }
    }

    public override int GetMaxBullet()
    {
        if (weaponParts.isLargetCapacity_AR)
            return (int)Mathf.Ceil(maxBullet * 1.5f);

        return maxBullet;
    }
}

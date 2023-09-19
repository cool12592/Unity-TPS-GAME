using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class TPS_SoldierState : MonoBehaviour, IState
{
    protected  TPS_SoldierController controller;
    public TPS_SoldierState(TPS_SoldierController controller_)
    {
        controller = controller_;
    }
    public abstract void enter();
    public abstract void update();
    public abstract void lateUpdate();
    public abstract void exit();
}
namespace SoldierState
{
    public class Patroll : TPS_SoldierState
    {
        const float DestDist = 4.5f;
        public float targetDestDist = 100f;

        Vector3 goalDest;


        public Patroll(TPS_SoldierController controller) : base(controller)
        { }

        public override void enter()
        {
            mySetDestination();
        }

        public override void update()
        {
            if(controller.isAwakeLoading==false)
                controller.sight.FindVisibleTargets();
            

            var dist = (goalDest - controller.transform.position).magnitude;
            if (dist < DestDist)
            {
                mySetDestination();
            }   
        }

        public override void lateUpdate()
        {
        }

        public override void exit()
        {
        }

        void mySetDestination()
        {
            controller.StartCoroutine(GetRandomPositionOnNavMesh()); // NavMesh ���� ������ ��ġ�� �����ɴϴ�.
        }

        IEnumerator GetRandomPositionOnNavMesh()
        {
            while (true)
            {
                Vector3 randomDirection = Random.insideUnitSphere * targetDestDist;          
                randomDirection += controller.transform.position;                     
                randomDirection.y = 0f;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDirection, out hit, 1f, NavMesh.AllAreas)) // ���� ��ġ�� NavMesh ���� �ִ��� Ȯ���մϴ�.
                {
                    goalDest = hit.position; // NavMesh ���� ���� ��ġ�� ��ȯ�մϴ�.

                    controller.agent.SetDestination(goalDest);
                    break;
                }
                yield return null;
            }
        }
    }


    public class Tracking : TPS_SoldierState
    {
        const float runSpeed = 6f;

        public Tracking(TPS_SoldierController controller) : base(controller)
        { }

        public override void enter()
        {
            controller.AnimTrigger("TIdle");
            controller.goalSpeed = runSpeed;
            controller.agent.avoidancePriority = 50;
            controller.spine.eulerAngles = controller.orininalSpineRot;
            controller.visibleAvatarspine.rotation = controller.spine.rotation;

        }

        public override void update()
        {
            controller.sight.FindVisibleTargets();

            if (controller.target == null)
                return;

            var pos = controller.target.transform.position;
          //  pos.y = -2f;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(pos, out hit, 10f, NavMesh.AllAreas)) // ���� ��ġ�� NavMesh ���� �ִ��� Ȯ���մϴ�.
            {
                controller.agent.SetDestination(hit.position);
            }
            else
                controller.agent.SetDestination(controller.transform.position);
        }

        public override void lateUpdate()
        {
        }

        public override void exit()
        {
        }
    }

    public class Attack : TPS_SoldierState
    {
        Vector3 rotOffset;
        float rotationSpeed = 16f;

        public Attack(TPS_SoldierController controller) : base(controller)
        { }

        public override void enter()
        {
            rotOffset = new Vector3(-20f, 40f, 0f);
            controller.goalSpeed = 0f;
            controller.agent.speed = 0f;
            controller.agent.velocity = Vector2.zero;
            controller.agent.avoidancePriority = 49;

            controller.sight.viewDistance += 10f;

            controller.AnimSetBool("isFire",true);

            controller.myAttackSound.volume = 1f;
            controller.myAttackSound.Play();

        }

        public override void update()
        {


           
        }

        public override void lateUpdate()
        {
            controller.spine.LookAt(controller.target.transform.position); //�÷��̾��� ��ü�κи� Ÿ�� ��ġ �ٶ�
            controller.spine.rotation = controller.spine.rotation * Quaternion.Euler(rotOffset);

            controller.visibleAvatarspine.rotation = controller.spine.rotation;

            Transform targetTr = controller.target.transform;

            if((controller.sight.RayCastToTarget(targetTr) == false))
            {
                controller.myState.changeState(StateMachine.EnumState.TRACKING);
                return;
            }

       
            if (controller.sight.CheckViewAngleToTarget(targetTr)) // �þ߰��� ����� ��ü ȸ���Ѵ�.
            {
                TurnTowardsTarget(controller.target);
            }


           
        }

        public override void exit()
        {

            controller.RemoveAllMuzzleEffect();
            controller.AnimSetBool("isFire", false);
            controller.sight.viewDistance -= 10f;
            SoundManager.Instance.AddFadeOutSound(controller.myAttackSound);
        }

        void TurnTowardsTarget(GameObject target_)
        {


            var direction = target_.transform.position - controller.transform.position;
            direction.y = 0f;
            Vector3 desiredForward = Vector3.RotateTowards(controller.transform.forward, direction.normalized, rotationSpeed * Time.deltaTime, .1f);
            Quaternion _newRotation = Quaternion.LookRotation(desiredForward);

            //�����
            controller.transform.rotation = _newRotation;

            ////y���� �ڱⲨ�� �״�� ����. �� ���Ʒ��δ� ȸ������ �ʰ��Ѵ�.
            //Vector3 targetVec = new Vector3(target_.transform.position.x, controller.transform.position.y, target_.transform.position.z);
            //Vector3 dir = targetVec - controller.transform.position;
            //controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * lookSpeed);
        }
    }

    public class Death : TPS_SoldierState
    {
        public Death(TPS_SoldierController controller) : base(controller)
        { }

        public override void enter()
        {
            controller.Death();
        }

        public override void update()
        {

        }

        public override void lateUpdate()
        {
        }

        public override void exit()
        {
        }
    }
}
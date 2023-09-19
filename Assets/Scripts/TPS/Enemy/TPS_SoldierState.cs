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
            controller.StartCoroutine(GetRandomPositionOnNavMesh()); // NavMesh 위의 랜덤한 위치를 가져옵니다.
        }

        IEnumerator GetRandomPositionOnNavMesh()
        {
            while (true)
            {
                Vector3 randomDirection = Random.insideUnitSphere * targetDestDist;          
                randomDirection += controller.transform.position;                     
                randomDirection.y = 0f;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDirection, out hit, 1f, NavMesh.AllAreas)) // 랜덤 위치가 NavMesh 위에 있는지 확인합니다.
                {
                    goalDest = hit.position; // NavMesh 위의 랜덤 위치를 반환합니다.

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

            if (NavMesh.SamplePosition(pos, out hit, 10f, NavMesh.AllAreas)) // 랜덤 위치가 NavMesh 위에 있는지 확인합니다.
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
            controller.spine.LookAt(controller.target.transform.position); //플레이어의 상체부분만 타겟 위치 바라봄
            controller.spine.rotation = controller.spine.rotation * Quaternion.Euler(rotOffset);

            controller.visibleAvatarspine.rotation = controller.spine.rotation;

            Transform targetTr = controller.target.transform;

            if((controller.sight.RayCastToTarget(targetTr) == false))
            {
                controller.myState.changeState(StateMachine.EnumState.TRACKING);
                return;
            }

       
            if (controller.sight.CheckViewAngleToTarget(targetTr)) // 시야각을 벗어나면 전체 회전한다.
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

            //여기당
            controller.transform.rotation = _newRotation;

            ////y축은 자기꺼를 그대로 쓴다. 즉 위아래로는 회전하지 않게한다.
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
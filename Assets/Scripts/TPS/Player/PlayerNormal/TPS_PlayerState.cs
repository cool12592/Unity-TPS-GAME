using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TPS_PlayerState : IState
{
    protected PlayerController2 controller;
    public TPS_PlayerState(PlayerController2 controller_)
    {
        controller = controller_;
    }
    public abstract void enter();
    public abstract void update();
    public abstract void lateUpdate();
    public abstract void exit();
}

namespace PlayerState
{
    public class Idle : TPS_PlayerState
    {
        float mouseClikTime = 0f;
        const float standClickTime = 0.15f;

        public Idle(PlayerController2 controller) : base(controller)
        { }

        public override void enter()
        {
            mouseClikTime = 0f;
        }

        public override void update()
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                mouseClikTime += Time.deltaTime;

                if (standClickTime < mouseClikTime)
                {
                    controller.myState.changeState(StateMachine.EnumState.THIRDAIM);

                }
            }


            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                if (mouseClikTime <= standClickTime)
                {
                    controller.myState.changeState(StateMachine.EnumState.FIRSTAIM);
                }
                mouseClikTime = 0f;

            }

        }

        public override void lateUpdate()
        {
        }

        public override void exit()
        {
        }
    }

    public class ThirdAim : TPS_PlayerState
    {
        public ThirdAim(PlayerController2 controller) : base(controller)
        { }

        public override void enter()
        {
            controller.playerAttack.StartAimMode();

        }

        public override void update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                controller.playerAttack.StartAttack();
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                controller.playerAttack.EndAttackInput();

            }

            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                controller.myState.changeState(StateMachine.EnumState.IDLE);
            }

        }

        public override void lateUpdate()
        {
        }

        public override void exit()
        {
            controller.playerAttack.EndAimMode();

        }
    }

    public class FpsMode : TPS_PlayerState
    {
        public FpsMode(PlayerController2 controller) : base(controller)
        { }

        public override void enter()
        {
            controller.playerAttack.StartFpsMode();
        }

        public override void update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                controller.playerAttack.StartAttack();
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                controller.playerAttack.EndAttackInput();
            }

            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                controller.myState.changeState(StateMachine.EnumState.IDLE);
            }

        }

        public override void lateUpdate()
        {
        }

        public override void exit()
        {
            controller.playerAttack.EndFpsMode();
        }
    }

    public class Death : TPS_PlayerState
    {
        public Death(PlayerController2 controller) : base(controller)
        { }

        public override void enter()
        {
            controller.anim.SetBool("isAlive",false);

            controller.isActive = false;
            controller.cc.input = Vector3.zero;
            controller.anim.SetTrigger("TDeath");

        }

        public override void update()
        {
            
        }

        public override void lateUpdate()
        {
        }

        public override void exit()
        {
            controller.anim.SetBool("isAlive", true);

        }
    }
}

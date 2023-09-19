using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TPS_SoldierController : MonoBehaviour
{
    [Header("공격 관련")]
    [SerializeField]
    ObjectPool groundEffectPool;
    [SerializeField]
    ObjectPool muzzleEffectPool;

    public Transform firePos;
    [SerializeField]
    float myDamage = 1.5f;

    PlayerHealth targetHealth;
    [HideInInspector] public GameObject target;
    [HideInInspector] public TPS_SightCtrl sight;
    [HideInInspector] public AudioSource myAttackSound;


    [Header("아바타 관련")]
    [HideInInspector] public Vector3 orininalSpineRot;
    [HideInInspector] public Transform spine; // 아바타의 상체
    [HideInInspector] public Transform visibleAvatarspine; // 아바타의 상체

    public GameObject alwaysVisibleAvatar;
    Animator visibleAvartarAnim;
    Animator anim;

    [Header("이동 관련")]
    [HideInInspector] public NavMeshAgent agent; // NavMeshAgent를 저장할 변수
    [HideInInspector] const float walkSpeed = 2f;
    public bool isMove = true;
    public float goalSpeed;


    [Header("기타 등등")]
    [HideInInspector] public bool isAwakeLoading = true;
    [HideInInspector] public StateMachine myState;

    [SerializeField] GameObject hideObject;
    public GameObject tactical_UI;
    public GameObject tacticalTarget_UI;
    GameObject myHead;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        spine = anim.GetBoneTransform(HumanBodyBones.Spine);

        visibleAvartarAnim = alwaysVisibleAvatar.GetComponent<Animator>();
        visibleAvatarspine = visibleAvartarAnim.GetBoneTransform(HumanBodyBones.Spine);
        orininalSpineRot = spine.eulerAngles;
        sight = GetComponent<TPS_SightCtrl>();

        myAttackSound = GetComponent<AudioSource>();
        goalSpeed = walkSpeed;
        myHead = transform.Find("Robot_Soldier_Head").gameObject;
        Invoke("AwakeUp", 3f);
    }

    void AwakeUp()
    {
        isAwakeLoading = false;
    }

    void Start()
    {

        myState = GetComponent<StateMachine>();
        myState.insertState(StateMachine.EnumState.PATROL, new SoldierState.Patroll(this));
        myState.insertState(StateMachine.EnumState.TRACKING, new SoldierState.Tracking(this));
        myState.insertState(StateMachine.EnumState.ATTACKING, new SoldierState.Attack(this));
        myState.insertState(StateMachine.EnumState.DEATH, new SoldierState.Death(this));

        myState.initState(StateMachine.EnumState.PATROL);

        GameManager.Instance.tps_nowStage.BornEnemy();
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            Death();

        agent.speed = Mathf.Lerp(agent.speed, goalSpeed, Time.deltaTime*2f);
        AnimSetFloat(agent.speed);
    }


    public void StartAttack(GameObject target)
    {
        if (myState.getCurrentState() == StateMachine.EnumState.ATTACKING || myState.getCurrentState() == StateMachine.EnumState.DEATH)
            return;

        if (targetHealth == null && target.GetComponent<PlayerHealth>() == null)
            return;

        targetHealth = target.GetComponent<PlayerHealth>();
        this.target = target;
        myState.changeState(StateMachine.EnumState.ATTACKING);
    }

    public void StartTrack(GameObject target)
    {
        if (myState.getCurrentState() == StateMachine.EnumState.ATTACKING || myState.getCurrentState() == StateMachine.EnumState.DEATH)
            return;

        if (targetHealth == null && target.GetComponent<PlayerHealth>() == null)
            return;

        targetHealth = target.GetComponent<PlayerHealth>();

        this.target = target;

        myState.changeState(StateMachine.EnumState.TRACKING);
    }


    public void FireEffect()
    {
        if (myState.getCurrentState() != StateMachine.EnumState.ATTACKING)
            return;

        var muzzleEffect = muzzleEffectPool.GetObject();
        muzzleEffect.transform.position = firePos.position;
        muzzleEffect.transform.rotation = firePos.rotation;
        muzzleEffect.transform.parent = muzzleEffectPool.gameObject.transform;

        if (target)
        {
            var groundFireEffect = groundEffectPool.GetObject();
            groundFireEffect.transform.position = target.transform.position;
            groundFireEffect.transform.rotation = Quaternion.identity;

            targetHealth.TakeDamage(myDamage);
        }


    }

    public void RemoveAllMuzzleEffect()
    {
        foreach(Transform muzzle in muzzleEffectPool.transform)
        {
            if(muzzle.gameObject.activeSelf)
                muzzleEffectPool.ReturnObject(muzzle.gameObject);
        }
    }

    public void AnimSetBool(string str,bool b)
    {
        anim.SetBool(str,b);
        if (visibleAvartarAnim)
        {
            visibleAvartarAnim.SetBool(str, b);
        }
    }

    public void AnimTrigger(string str)
    {
        anim.SetTrigger(str);
        if (visibleAvartarAnim)
        {
            visibleAvartarAnim.SetTrigger(str);
        }
    }

    public void AnimSetFloat(float num)
    {
        anim.SetFloat("nowSpeed", num);
        if (visibleAvartarAnim)
        {
            visibleAvartarAnim.SetFloat("nowSpeed", num);
        }
    }

    public void Death()
    {
        tactical_UI.SetActive(false);
        hideObject.SetActive(false);
        GetComponent<BoxCollider>().isTrigger = true;
        myHead.GetComponent<BoxCollider>().isTrigger = true;
        goalSpeed = 0f;
        agent.enabled = false;
        AnimTrigger("TDeath");
        AnimSetBool("isDeath", true);
        if(GameManager.Instance.tps_nowStage!=null)
        GameManager.Instance.tps_nowStage.DeathEnemy();
        Destroy(gameObject,30f);
    }

   
    public void Hited()
    {
        AnimTrigger("THited");
        if (myState.getCurrentState() == StateMachine.EnumState.PATROL)
        {
            goalSpeed = 0f;
            agent.speed = 0f;
            agent.velocity = Vector3.zero;
        }
        Invoke("EndHited", 0.5f);
    }

    public void EndHited()
    {
        if (myState.getCurrentState() == StateMachine.EnumState.PATROL)
        {
            myState.changeState(StateMachine.EnumState.TRACKING);
        }

    }

}

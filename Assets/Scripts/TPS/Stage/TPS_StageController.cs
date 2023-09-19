using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TPS_StageController : MonoBehaviour
{

    protected GameObject player;

    [Header("�� ���� ����")]

    [HideInInspector] public GameObject spawnObjectsParent;
    public GameObject stageObjectsPrefab;
    public Transform respawnPos;

    bool reStarting = false;

    [SerializeField]
    TPS_EnemySpawner enemySpawner;

    int currentAliveEnemyNumber = 0;
    const int MaxAliveEnemyNumber = 8;

    int currentKillNum = 0;
    float killLogTimer = 0f;
    float enemyRespawnTimer = 0f;
    [SerializeField] float enemyRespawnTime;

    [Header("�������� UI��")]

    [SerializeField]
    Text enemyNumText;
    [SerializeField]
    Text remainGenTime;

    int MaximumScore=0;
    [SerializeField]
    Text MaximumScoreText;
    [SerializeField]
    Text killLog;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.player;

        respawnPos = transform.Find("RespawnPos");
        InitStage();
    }


    void InitStage()
    {
        GameManager.Instance.tps_nowStage = this;
    }

    public void BornEnemy()
    {
        currentAliveEnemyNumber++;
        enemyNumText.text = "��: " + currentAliveEnemyNumber + "��";

        if(currentAliveEnemyNumber == MaxAliveEnemyNumber)
            remainGenTime.text = "";

    }
    public void DeathEnemy()
    {
        currentAliveEnemyNumber--;
        enemyNumText.text = "��: " + currentAliveEnemyNumber + "��";


        killLogTimer = 4f;
        killLog.gameObject.SetActive(true);
        killLog.text = ++currentKillNum + "ų";

        if (MaximumScore < currentKillNum)
        {
            MaximumScore = currentKillNum;
            MaximumScoreText.text = "�ִ� óġ: " + MaximumScore + "ų";
        }
    }

    private void Update()
    {
        KillLogTimer();
        ReSpawnTimer();
    }

    void KillLogTimer()
    {
        if (0f < killLogTimer)
        {
            killLogTimer -= Time.deltaTime;
            if (killLogTimer <= 0f)
            {
                killLogTimer = 0f;
                killLog.gameObject.SetActive(false);
            }
        }
    }

    void ReSpawnTimer()
    {
        if (currentAliveEnemyNumber < MaxAliveEnemyNumber)
        {
            enemyRespawnTimer += Time.deltaTime;

            int time = (int)(enemyRespawnTime - enemyRespawnTimer);
            remainGenTime.text = "���� �ð�: " + time + "��";

            if (enemyRespawnTime < enemyRespawnTimer)
            {
                int needEnemyNum = MaxAliveEnemyNumber - currentAliveEnemyNumber;
                enemySpawner.EnemySpawn(needEnemyNum);
                enemyRespawnTimer = 0f;


            }
        }
    }

    public virtual void StartStage()
    {
        currentKillNum = 0;
        spawnObjectsParent = Instantiate(stageObjectsPrefab, Vector3.zero,Quaternion.identity);

    }

    public void RestartGame()
    {
        if (reStarting)
            return;
        reStarting = true;

        Invoke("ExecuteRestartGame", 5f);
    }

    public void ExecuteRestartGame()
    {
        DestroySpawnObjects();
        currentAliveEnemyNumber = 0;
        player.GetComponent<PlayerController2>().OffMyInput();
        player.transform.position = respawnPos.transform.position;
        player.GetComponent<PlayerController2>().RestartGame();
        reStarting = false;
        StartStage();
    }

    public void DestroySpawnObjects()
    {
        Destroy(spawnObjectsParent);
    }
}

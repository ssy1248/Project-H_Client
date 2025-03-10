using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;

public class MonsterManager : MonoBehaviour
{
    private static MonsterManager _instance = null;
    public static MonsterManager Instance => _instance;

    private readonly Dictionary<int, string> monsterDb = new Dictionary<int, string>();
    private readonly Dictionary<string, Monster> monsterDict = new Dictionary<string, Monster>();


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeMonsterDatabase();
    }



    // ������ �о���� 
    private void InitializeMonsterDatabase()
    {
        for (int i = 1; i < 30; i++)
        {
            var monsterCode = Constants.MonsterCodeFactor + i;
            var monsterPath = $"Monster/Monster{i}";
            monsterDb.Add(monsterCode, monsterPath);
        }
    }

    // ���� ����
    public void CreateMonsters(S_MonsterSpawn spawnPacket)
    {
        foreach (var monsterInfo in spawnPacket.MonsterInfo)
        {
            CreateMonster(monsterInfo); // ���� ���� ����
        }
    }

    public void CreateMonster(SyncMonsterTransformInfo monsterInfo)
    {
        
        string id = monsterInfo.MonsterId;

        // �̹� �����ϸ� ���� X
        if (monsterDict.ContainsKey(id))
        {
            //Debug.Log($"[���� ����] ID {id} ���Ͱ� �̹� �����մϴ�.");
            return;
        }

        int monsterCode = monsterInfo.MonsterStatus.MonsterModel;
        float hp = monsterInfo.MonsterStatus.MonsterHp;
        Vector3 spawnPosition = new Vector3(monsterInfo.Transform.PosX, monsterInfo.Transform.PosY, monsterInfo.Transform.PosZ);
        Quaternion rot = Quaternion.Euler(0, monsterInfo.Transform.Rot, 0);
        string name = monsterInfo.MonsterStatus.MonsterIdx.ToString();

        // ���� ������ ��� ã��
        string monsterPath = monsterDb.GetValueOrDefault(Constants.MonsterCodeFactor + monsterCode, "Monster/Monster1");
        Monster monsterPrefab = Resources.Load<Monster>(monsterPath);

        if (monsterPrefab == null)
        {
            Debug.LogError($"���� ��ΰ� ������ �ƴմϴ�. : {monsterPath}");
            return;
        }

        // ���� �ν��Ͻ� ����
        Monster newMonster = Instantiate(monsterPrefab, spawnPosition, rot);
        newMonster.Initialize(id, name, hp, spawnPosition, rot);

        // ���� �߰�
        monsterDict[id] = newMonster;

        //Debug.Log($"[���� ����] ID {id} ���Ͱ� ���� �Ǿ����ϴ�..");

    }

    // ���� �ִϸ��̼� ������Ʈ
    public void MonsterAttckAnimation(S_MonsterAttck monsterAttckPacket)
    {
        foreach (var mMonsterId in monsterAttckPacket.MonsterId)
        {

            if (monsterDict.ContainsKey(mMonsterId)){
                Monster findMonster = monsterDict.GetValueOrDefault(mMonsterId);
                if (findMonster != null)
                {
                    findMonster.switchAnimation(monsterAttckPacket.MonsterAinID);
                }
            }
                        
        }
    }

    public void MonsterHitAnimation(S_MonsterHit monsterHitPacket)
    {
        foreach (var mMonsterId in monsterHitPacket.MonsterId)
        {

            if (monsterDict.ContainsKey(mMonsterId))
            {
                Monster findMonster = monsterDict.GetValueOrDefault(mMonsterId);
                if (findMonster != null)
                {
                    findMonster.switchAnimation(monsterHitPacket.MonsterAinID);
                }
            }

        }
    }

    // ���� ����
    public void DeleteMonster(S_MonsterDie monsterDiePacket)
    {
        foreach (var mMonsterId in monsterDiePacket.MonsterId)
        {

            if (monsterDict.ContainsKey(mMonsterId))
            {
                Monster findMonster = monsterDict[mMonsterId];

                // ������Ʈ�� �����ϸ� ����
                if (findMonster != null)
                {
                    findMonster.switchAnimation(monsterDiePacket.MonsterAinID);

                    // GameObject.Destroy(findMonster.gameObject);

                    // Dictionary������ ����
                    monsterDict.Remove(mMonsterId);
                }

            }

        }
    }


    // ���� ������Ʈ 
    public void UpdateMonsters(S_MonsterMove monsterMovePacket)
    {
        // int id, float hp ,Vector3 position, Quaternion rot, float speed

        foreach (var monsterInfo in monsterMovePacket.TransformInfo)
        {
            UpdateMonster(monsterInfo); // ���� ���� ������Ʈ 
        }

    }

    public void UpdateMonster(SyncMonsterTransformInfo transformInfo)
    {
        string id = transformInfo.MonsterId;
        //Debug.Log($"[���� ������Ʈ] {transformInfo} ");
        // �̹� ���ٸ� ������Ʈ X
        if (!monsterDict.ContainsKey(id))
        {
            //Debug.Log($"[���� ������Ʈ] ID {id} ���Ͱ� �����ϴ�.");
            CreateMonster(transformInfo);
            return;
        } 


        // �����ϸ� ���� ������Ʈ ����.
        Monster findMonster = monsterDict.GetValueOrDefault(id);

        // ������Ʈ�� ����� ���� ����.
        float hp = transformInfo.MonsterStatus.MonsterHp;
        float speed = transformInfo.Speed;
        Vector3 targetPosition = new Vector3(transformInfo.Transform.PosX, transformInfo.Transform.PosY, transformInfo.Transform.PosZ);
        Quaternion rot = Quaternion.Euler(0, transformInfo.Transform.Rot, 0);


        findMonster.UpdateMonsterPosition(targetPosition, rot, speed, hp);

        //Debug.Log($"[���� ������Ʈ] ID {id} ���Ͱ� ������Ʈ �Ǿ����ϴ�..");

    }


    // ���� ����. 
    public void RemoveMonster(string id)
    {
        if (monsterDict.ContainsKey(id))
        {
            Monster foundMonster = monsterDict.GetValueOrDefault(id);
            monsterDict.Remove(id); // �ش� Ű�� ��ųʸ����� ����

            // ���� ������Ʈ ����
            Destroy(foundMonster.gameObject); // ���� ������Ʈ ����
        } else
        {
            Debug.LogError($"������ ���� {id}�� �������� �ʽ��ϴ�.");
        }
        
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

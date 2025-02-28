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



    // 프리팹 읽어오기 
    private void InitializeMonsterDatabase()
    {
        for (int i = 1; i < 30; i++)
        {
            var monsterCode = Constants.MonsterCodeFactor + i;
            var monsterPath = $"Monster/Monster{i}";
            monsterDb.Add(monsterCode, monsterPath);
        }
    }

    // 몬스터 생성
    public void CreateMonsters(S_MonsterSpawn spawnPacket)
    {
        foreach (var monsterInfo in spawnPacket.MonsterInfo)
        {
            CreateMonster(monsterInfo); // 개별 몬스터 생성
        }
    }

    public void CreateMonster(SyncMonsterTransformInfo monsterInfo)
    {
        
        string id = monsterInfo.MonsterId;

        // 이미 존재하면 생성 X
        if (monsterDict.ContainsKey(id))
        {
            Debug.Log($"[몬스터 생성] ID {id} 몬스터가 이미 존재합니다.");
            return;
        }

        int monsterCode = monsterInfo.MonsterStatus.MonsterModel;
        float hp = monsterInfo.MonsterStatus.MonsterHp;
        Vector3 spawnPosition = new Vector3(monsterInfo.Transform.PosX, monsterInfo.Transform.PosY, monsterInfo.Transform.PosZ);
        Quaternion rot = Quaternion.Euler(0, monsterInfo.Transform.Rot, 0);
        string name = monsterInfo.MonsterStatus.MonsterIdx.ToString();

        // 몬스터 프리팹 경로 찾기
        string monsterPath = monsterDb.GetValueOrDefault(Constants.MonsterCodeFactor + monsterCode, "Monster/Monster1");
        Monster monsterPrefab = Resources.Load<Monster>(monsterPath);

        if (monsterPrefab == null)
        {
            Debug.LogError($"몬스터 경로가 정상이 아닙니다. : {monsterPath}");
            return;
        }

        // 몬스터 인스턴스 생성
        Monster newMonster = Instantiate(monsterPrefab, spawnPosition, rot);
        newMonster.Initialize(id, name, hp, spawnPosition, rot);

        // 몬스터 추가
        monsterDict[id] = newMonster;

        Debug.Log($"[몬스터 생성] ID {id} 몬스터가 생성 되었습니다..");

    }

    // 몬스터 애니메이션 업데이트
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

    // 몬스터 삭제
    public void DeleteMonster(S_MonsterDie monsterDiePacket)
    {
        foreach (var mMonsterId in monsterDiePacket.MonsterId)
        {

            if (monsterDict.ContainsKey(mMonsterId))
            {
                Monster findMonster = monsterDict[mMonsterId];

                // 오브젝트가 존재하면 삭제
                if (findMonster != null)
                {
                    findMonster.switchAnimation(monsterDiePacket.MonsterAinID);

                    // GameObject.Destroy(findMonster.gameObject);

                    // Dictionary에서도 제거
                    monsterDict.Remove(mMonsterId);
                }

            }

        }
    }


    // 몬스터 업데이트 
    public void UpdateMonsters(S_MonsterMove monsterMovePacket)
    {
        // int id, float hp ,Vector3 position, Quaternion rot, float speed

        foreach (var monsterInfo in monsterMovePacket.TransformInfo)
        {
            UpdateMonster(monsterInfo); // 개별 몬스터 업데이트 
        }

    }

    public void UpdateMonster(SyncMonsterTransformInfo transformInfo)
    {
        string id = transformInfo.MonsterId;
        //Debug.Log($"[몬스터 업데이트] {transformInfo} ");
        // 이미 없다면 업데이트 X
        if (!monsterDict.ContainsKey(id))
        {
            Debug.Log($"[몬스터 업데이트] ID {id} 몬스터가 없습니다.");
            CreateMonster(transformInfo);
            return;
        } 


        // 존재하면 몬스터 업데이트 진행.
        Monster findMonster = monsterDict.GetValueOrDefault(id);

        // 업데이트에 사용할 변수 생성.
        float hp = transformInfo.MonsterStatus.MonsterHp;
        float speed = transformInfo.Speed;
        Vector3 targetPosition = new Vector3(transformInfo.Transform.PosX, transformInfo.Transform.PosY, transformInfo.Transform.PosZ);
        Quaternion rot = Quaternion.Euler(0, transformInfo.Transform.Rot, 0);


        findMonster.UpdateMonsterPosition(targetPosition, rot, speed, hp);

        Debug.Log($"[몬스터 업데이트] ID {id} 몬스터가 업데이트 되었습니다..");

    }


    // 몬스터 삭제. 
    public void RemoveMonster(string id)
    {
        if (monsterDict.ContainsKey(id))
        {
            Monster foundMonster = monsterDict.GetValueOrDefault(id);
            monsterDict.Remove(id); // 해당 키를 딕셔너리에서 제거

            // 몬스터 오브젝트 삭제
            Destroy(foundMonster.gameObject); // 게임 오브젝트 삭제
        } else
        {
            Debug.LogError($"삭제할 몬스터 {id}가 존재하지 않습니다.");
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

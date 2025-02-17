// 파티 클래스
using UnityEngine;

[System.Serializable]
public class Party
{
    public string dungeonName;
    public string partyName;
    public int memberCount;

    public Party(string dungeonName, string partyName, int memberCount)
    {
        this.dungeonName = dungeonName;
        this.partyName = partyName;
        this.memberCount = memberCount;
    }
}
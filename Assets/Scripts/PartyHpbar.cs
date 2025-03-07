using UnityEngine;
using UnityEngine.UI;

public class PartyHealthBar : MonoBehaviour
{
    public Slider[] partyHealthBars; // 4명의 파티원 HP 바 (0~3번 인덱스)
    private PlayerController playerController;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    public Slider GetMyHealthBar()
    {
        if (playerController == null)
            return null;

        // partyIndex를 활용하여 해당 위치의 HP 바를 가져옴
        if (playerController.partyIndex >= 0 && playerController.partyIndex < partyHealthBars.Length)
        {
            return partyHealthBars[playerController.partyIndex];
        }

        return null;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class PartyHealthBar : MonoBehaviour
{
    public Slider[] partyHealthBars; // 4���� ��Ƽ�� HP �� (0~3�� �ε���)
    private PlayerController playerController;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    public Slider GetMyHealthBar()
    {
        if (playerController == null)
            return null;

        // partyIndex�� Ȱ���Ͽ� �ش� ��ġ�� HP �ٸ� ������
        if (playerController.partyIndex >= 0 && playerController.partyIndex < partyHealthBars.Length)
        {
            return partyHealthBars[playerController.partyIndex];
        }

        return null;
    }
}

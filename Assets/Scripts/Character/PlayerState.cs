using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] TextMeshProUGUI playerLevelText;
    [SerializeField] TextMeshProUGUI playerExpText;
    [SerializeField] TextMeshProUGUI playerHpText;
    [SerializeField] TextMeshProUGUI playerMpText;
    [SerializeField] TextMeshProUGUI playerSpeedText;
    [SerializeField] TextMeshProUGUI playerAtKText;
    [SerializeField] TextMeshProUGUI playerDefText;
    [SerializeField] Player player;

    public void SetState()
    {
        gameObject.SetActive(true);
        player = TownManager.Instance.MyPlayer;

        Debug.Log(player.playerData);
        playerNameText.text = player.nickname;
        playerLevelText.text = player.playerData.Level.ToString();
        playerExpText.text = "현재 경험치 : \n" + player.exp;
        playerHpText.text = player.playerData.MaxHp.ToString();
        playerMpText.text = player.playerData.MaxMp.ToString();
        playerAtKText.text = player.playerData.Atk.ToString();
        playerDefText.text = player.playerData.Def.ToString();
        playerSpeedText.text = player.playerData.Speed.ToString();
    }
    public void GetState()
    {
        TownManager.Instance.GetUserState();
    }
    public void Togle()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        else
        {
            GetState();
        }
    }
}

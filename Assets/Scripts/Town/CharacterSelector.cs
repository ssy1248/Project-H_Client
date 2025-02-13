using UnityEngine;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    [SerializeField] private int id;  // 캐릭터 id

    private void OnEnable()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(Selector);
    }

    void Selector()
    {
        // id가 1부터 5 사이여야만 요청하도록 처리 
        if (id >= 1 && id <= 5)
        {
            Debug.Log($"Character selected with id: {id}");  // 선택된 캐릭터의 id 로그 출력

            // UIRegister를 통해 선택된 캐릭터 반영
            UIRegister.Instance.SetSelectedCharacter(id);
        }
        else
        {
            Debug.LogError($"Invalid character id: {id}");
        }
    }
}

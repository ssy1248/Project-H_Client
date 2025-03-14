using UnityEngine;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    [SerializeField] private int id;  // ĳ���� id

    private void OnEnable()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(Selector);
    }

    void Selector()
    {
        // id�� 1���� 5 ���̿��߸� ��û�ϵ��� ó�� 
        if (id >= 1 && id <= 5)
        {
            //Debug.Log($"Character selected with id: {id}");  // ���õ� ĳ������ id �α� ���

            // UIRegister�� ���� ���õ� ĳ���� �ݿ�
            UIRegister.Instance.SetSelectedCharacter(id);
        }
        else
        {
            Debug.LogError($"Invalid character id: {id}");
        }
    }
}

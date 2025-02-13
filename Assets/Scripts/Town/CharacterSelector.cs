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
        // id�� 0���� 4 ���̿��߸� ��û�ϵ��� ó�� 
        if (id >= 0 && id <= 5)
        {
            Debug.Log($"Character selected with id: {id}");  // ���õ� ĳ������ id �α� ���

            // UIRegister�� ���� ���õ� ĳ���� �ݿ�
            UIRegister.Instance.SetSelectedCharacter(id);   
        }
        else
        {
            Debug.LogError($"Invalid character id: {id}");
        }
    }
}
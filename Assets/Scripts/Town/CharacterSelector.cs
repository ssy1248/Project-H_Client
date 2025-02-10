using UnityEngine;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    [SerializeField] int id;

    private void OnEnable()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(Selector);
    }

    void Selector()
    {
        TownManager.Instance.SelectCharacterRequest(id);
    }
}

using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Button leaveButton;

    private bool isShowing = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!DungeonManager.Instance) return;
        leaveButton.onClick.AddListener(DungeonManager.Instance.DungeonExit);
    }

    public void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        isShowing = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        isShowing = false;
    }

    public void Toggle()
    {
        if (isShowing)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }
}

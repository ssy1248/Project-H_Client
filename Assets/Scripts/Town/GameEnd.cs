using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnd : MonoBehaviour
{
    public void TheEnd()
    {
        Application.Quit();

        // ������ ȯ�濡���� ���� (Unity ������ �÷��� ����)
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}

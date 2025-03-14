using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnd : MonoBehaviour
{
    public void TheEnd()
    {
        Application.Quit();

        // 에디터 환경에서만 동작 (Unity 에디터 플레이 중지)
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundPlay : MonoBehaviour
{
    bool isStage1 = false;
    bool isIntro = false;
    bool isStage2 = false;
    bool isCredit = false;

    bool stage1 = false;
    bool stage2 = false;
    bool credit = false;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        SEManager.instance.LoopPlaySE("Intro");
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "UIScene" && !isIntro)
        {
            isIntro = true;
            isStage1 = false;
            isStage2 = false;
            isCredit = false;

            if (stage1)
            {
                SEManager.instance.StopSE("Stage1");
                SEManager.instance.LoopPlaySE("Intro");               
            }
            if(stage2)
            {
                SEManager.instance.StopSE("Stage2");
                SEManager.instance.LoopPlaySE("Intro");
            }
            if(credit)
            {
                SEManager.instance.StopSE("Credit");
                SEManager.instance.LoopPlaySE("Intro");
                credit = false;
            }
        }

        if (SceneManager.GetActiveScene().name == "MainScene" && !isStage1)
        {
            isIntro = false;
            isStage1 = true;
            isStage2 = false;
            isCredit = false;

            stage1 = true;
            SEManager.instance.StopSE("Intro");
            SEManager.instance.LoopPlaySE("Stage1");
        }

        if(SceneManager.GetActiveScene().name == "Stage2" && !isStage2)
        {
            isIntro = false;
            isStage1 = false;
            isStage2 = true;
            isCredit = false;

            stage2 = true;
            SEManager.instance.StopSE("Intro");
            SEManager.instance.LoopPlaySE("Stage2");
        }

        if(SceneManager.GetActiveScene().name == "EndingScene" && !isCredit)
        {
            credit = true;

            if (stage1)
            {
                SEManager.instance.StopSE("Stage1");
                SEManager.instance.LoopPlaySE("Credit");
                stage1 = false;
            }

            if(stage2)
            {
                SEManager.instance.StopSE("Stage2");
                SEManager.instance.LoopPlaySE("Credit");
                stage2 = false;
            }
        }
    } 
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SEPlay : MonoBehaviour
{
    public string soundKey;
    public bool hasDelay;
    public float delay;

    public enum PlayType { OnEnable, OnClick, OnDisable, OnDestroy, Custom }
    [SerializeField]
    public PlayType playType;

    private void Awake()
    {
        if(playType == PlayType.OnClick)
        {
            var b = GetComponent<Button>();
            if(b == null)
            {
                Debug.LogWarning(gameObject.name + "의 SEPlay가 OnClick시 발생하게 설정했으나 Button 컴포넌트가 존재하지 않습니다.");
            }
            else
            {
                b.onClick.AddListener(Play);
            }
        }
    }

    private void OnEnable()
    {
        if(playType == PlayType.OnEnable)
        {
            if(hasDelay)
            {
                Invoke("Play", delay);
            }
            else
            {
                Play();
            }
        }
    }

    private void OnDisable()
    {
        if (playType == PlayType.OnDisable)
        {
            if (hasDelay)
            {
                Invoke("Play", delay);
            }
            else
            {
                Play();
            }

        }
    }

    private void OnDestroy()
    {
        if (playType == PlayType.OnDestroy)
        {
            if (hasDelay)
            {
                Invoke("Play", delay);
            }
            else
            {
                Play();
            }

        }
    }
    public void Play()
    {
        SEManager.instance.PlaySE(soundKey);
    }
}

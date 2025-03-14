using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SoundEffect
{
    public string key;
    public AudioClip clip;
}

public class SEManager : MonoBehaviour
{
    [SerializeField]
    List<SoundEffect> effect;
    List<AudioSource> audioPool;

    public static SEManager instance;

    Dictionary<string, AudioClip> se_map;

    // **마스터 볼륨**
    public float masterVolume = 1f;
    // Use this for initialization
    void Awake ()
    {
        if(instance ==null)
        {
            instance = this;
            audioPool = new List<AudioSource>();
           
            se_map = new Dictionary<string, AudioClip>();
            foreach(var e in effect)
            {
                se_map[e.key] = e.clip;
            }
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DestroySEManager()
    {
        Destroy(gameObject);
    }
	
    public void PlaySE(string key)
    {
        if (se_map.ContainsKey(key) == false)
        {
            Debug.LogError(key + " SE가 SEManager에 없습니다.");
        }
        else
        {
            PlaySound(se_map[key]);
        }
    }

    public void LoopPlaySE(string key)
    {
        if (se_map.ContainsKey(key) == false)
        {
            Debug.LogError(key + " SE가 SEManager에 없습니다.");
        }
        else
        {
            LoopPlaySound(se_map[key]);
        }
    }

    public void StopSE(string key)
    {
        if(se_map.ContainsKey(key) == false)
        {
            Debug.LogError(key + " SE가 SEManager에 없습니다.");
        }
        else
        {
            StopSound(se_map[key]);
        }
    }

    void PlaySound(AudioClip clip)
    {
        AudioSource availableAS = null;
        foreach(var AS in audioPool)
        {
            if(AS.isPlaying == false)
            {
                availableAS = AS;
                break;
            }
        }

        if(availableAS == null)
        {
            availableAS = gameObject.AddComponent<AudioSource>();
            audioPool.Add(availableAS);
            availableAS.playOnAwake = false;
            availableAS.loop = false;
        }
        availableAS.clip = clip;
        availableAS.Play();
    }

    // 이쪽 문제 있는거 확인하기
    void LoopPlaySound(AudioClip clip)
    {
        AudioSource availableAS = null;
        foreach (var AS in audioPool)
        {
            if (AS.isPlaying == false)
            {
                availableAS = AS;
                break;
            }
        }

        if (availableAS == null)
        {
            availableAS = gameObject.AddComponent<AudioSource>();
            audioPool.Add(availableAS);
            availableAS.playOnAwake = false;
            availableAS.loop = true;
        }
        availableAS.clip = clip;
        availableAS.Play();
    }

    void StopSound(AudioClip clip)
    {
        AudioSource avilableAS = null;
        foreach(var AS in audioPool)
        {
            if(AS.isPlaying == true)
            {
                avilableAS = AS;
                break;
            }
        }

        avilableAS.loop = false;
        avilableAS.clip = clip;
        avilableAS.Stop();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        // 이미 재생 중인 AudioSource의 볼륨도 변경
        foreach (var AS in audioPool)
        {
            if (AS.isPlaying)
            {
                AS.volume = masterVolume;
            }
        }
    }
}

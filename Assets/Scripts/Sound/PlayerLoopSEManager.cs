using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlayerLoopSoundEffect
{
    public string key;
    public AudioClip clip;
}

public class PlayerLoopSEManager : MonoBehaviour
{
    [SerializeField]
    List<SoundEffect> effect;
    List<AudioSource> audioPool;

    public static PlayerLoopSEManager instance;

    Dictionary<string, AudioClip> se_map;
	// Use this for initialization
	void Awake ()
    {
        if(instance ==null)
        {
            audioPool = new List<AudioSource>();
            instance = this;
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

        avilableAS.clip = clip;
        avilableAS.Stop();
    }
}

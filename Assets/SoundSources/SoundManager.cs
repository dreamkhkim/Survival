using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;
    public float initLength;
    public float volume = 1;

    public GameObject soundSourcePrefab;

    public AudioClip curBgm;

    public AudioClip pistolSound;
    public AudioClip pistolReload;
    public AudioClip handlingPistol;

    public AudioClip handlingLifle;
    public AudioClip rifleSound;
    public AudioClip rifleReload;

    public AudioClip shotGunSound;
    public AudioClip handlingShotGun;
    public AudioClip shotGunReload;


    public AudioClip rangedMonster;
    public AudioClip meleeMonster;

    Queue<GameObject> soundQueue = new Queue<GameObject>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < initLength; i++)
        {
            AddPool();
        }
    }

    public void AddPool()
    {
        GameObject obj = Instantiate(soundSourcePrefab);
        obj.transform.SetParent(this.transform);
        obj.SetActive(false);
        soundQueue.Enqueue(obj);
    }

    public AudioSource PopObj()
    {
        if (soundQueue.Count == 0)
            AddPool();

        GameObject returnObj = soundQueue.Dequeue();
        returnObj.SetActive(true);
        returnObj.transform.SetParent(null);
        return returnObj.GetComponent<AudioSource>();
        
    }

    public void ReturnPool(GameObject returnObj)
    {
        soundQueue.Enqueue(returnObj.gameObject);
        returnObj.SetActive(false);
        returnObj.transform.parent = this.transform;
    }


    public void PlayAudio(AudioClip clip, bool isLoop, float value)
    {
        volume = value;
        AudioSource audio = PopObj();
        audio.clip = clip;
        audio.loop = isLoop;
        audio.volume = volume;
        audio.PlayOneShot(clip);
    }

    // 해당 몬스터 위치에서 사운드 재생하는 함수
    public void PlayAudio(AudioClip clip, bool isLoop, Vector3 pos, float value)
    {
        volume = value;
        AudioSource audio = PopObj();
        audio.transform.position = pos;
        audio.clip = clip;
        audio.loop = isLoop;
        audio.volume = volume;
        audio.PlayOneShot(clip);
    }

}

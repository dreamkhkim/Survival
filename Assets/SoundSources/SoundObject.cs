using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject : MonoBehaviour
{

    AudioSource source;
    float time;

    private void OnEnable()
    {
        source = GetComponent<AudioSource>();
        time = 0;
    }


    // Update is called once per frame
    void Update()
    {
        if (source.clip != null)
        {
            if (source.isPlaying != true)
                SoundManager.instance.ReturnPool(this.gameObject);
        }
    }
}

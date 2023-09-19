using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum soundEnum {m4_attack, awm_attack ,m4_reload,awm_reload,weaponChange};
    private static SoundManager instance = null;

    [SerializeField] AudioSource m4_attackSound, awm_attackSound, m4_reloadSound,awm_reloadSound,weaponChangeSound;

    Dictionary<soundEnum, AudioSource> soundDict = new Dictionary<soundEnum, AudioSource>();
    
    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        soundDict.Add(soundEnum.m4_attack, m4_attackSound);
        soundDict.Add(soundEnum.awm_attack, awm_attackSound);
        soundDict.Add(soundEnum.m4_reload, m4_reloadSound);
        soundDict.Add(soundEnum.awm_reload, awm_reloadSound);
        soundDict.Add(soundEnum.weaponChange, weaponChangeSound);


    }

    public static SoundManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    public AudioSource GetSoundEffect(soundEnum sound, float volume = 1f)
    {
        soundDict[sound].volume = volume;
        return soundDict[sound];
    }

    LinkedList<AudioSource> fadeOutList = new LinkedList<AudioSource>();
    [SerializeField]
    float fadeOutSpeed = 2f;
    private void Update()
    {
        if (fadeOutList.Count == 0)
            return;

        var sound = fadeOutList.First;
        while (sound != null)
        {
            var next = sound.Next;

            sound.Value.volume -= Time.deltaTime * fadeOutSpeed;
            if (sound.Value.volume <= 0f)
            {
                sound.Value.Stop();
                fadeOutList.Remove(sound);
            }
            sound = next;
        }
       
    }
    public void AddFadeOutSound(soundEnum enumSound)
    {
        fadeOutList.AddLast(soundDict[enumSound]);
    }

    public void AddFadeOutSound(AudioSource sound)
    {
        fadeOutList.AddLast(sound);
    }

}

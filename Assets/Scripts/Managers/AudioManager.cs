using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    public GameObject audioSourcePrefab;

    public int poolSize = 20;

    private List<AudioSource> audioPool;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializePool();
    }
    
   

    private void InitializePool()
    {
        audioPool = new List<AudioSource>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(audioSourcePrefab, transform);
            AudioSource source = obj.GetComponent<AudioSource>();

            source.playOnAwake = false;
            source.spatialBlend = 1f;

            audioPool.Add(source);
        }
    }

    public void PlaySoundAtLocation(AudioClip clip, Vector3 position, AudioMixerGroup mixerGroup = null, float volume = 1f)
    {
        AudioSource availableSource = GetAvailableSource();

        if (availableSource != null)
        {
            availableSource.transform.position = position;

            availableSource.clip = clip;
            availableSource.outputAudioMixerGroup = mixerGroup;
            availableSource.volume = volume;
            
            availableSource.Play();
        }
        else
        {
            Debug.LogWarning("No audio available in pool");
        }
    }

    private AudioSource GetAvailableSource()
    {
        for (int i = 0; i < audioPool.Count; i++)
        {
            if (!audioPool[i].isPlaying)
            {
                return audioPool[i];
            }
        }

        return null;
    }
}
using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioSource audioSourcePrefab;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(AudioClip clip)
    {
        AudioSource source = Instantiate(audioSourcePrefab, transform.position, Quaternion.identity);
        source.clip = clip;
        source.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
        source.Play();
        Destroy(source.gameObject, clip.length);
    }

    public void PlaySingleSFX(AudioClip clip, string name)
    {
        if(transform.Find(name) != null)
        {
            return;
        }
        Debug.Log($"[SFXMANAGER] Playing single SFX: {name}");
        AudioSource source = Instantiate(audioSourcePrefab, transform.position, Quaternion.identity, transform);
        source.gameObject.name = name;
        source.clip = clip;
        source.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
        source.Play();
        Destroy(source.gameObject, clip.length);
    }

    public void StopSingleSFX(string name)
    {
        Transform sfxTransform = transform.Find(name);
        if (sfxTransform != null)
        {
            AudioSource source = sfxTransform.GetComponent<AudioSource>();
            if (source != null)
            {
                source.Stop();
                Destroy(source.gameObject);
            }
        }
    }
}

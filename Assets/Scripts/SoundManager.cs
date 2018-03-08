using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    private bool musicIsOn = false;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (PlayerPrefs.HasKey(ConstantNames.Music))
        {
            if (PlayerPrefs.GetInt(ConstantNames.Music) == 1)
            {
                musicIsOn = true;
                audioSource.Play();
            }
        }
        else
        {
            musicIsOn = true;
            audioSource.Play();
        }
    }

    public void SwitchAudio()
    {
        if (musicIsOn)
        {
            musicIsOn = false;
            audioSource.Stop();
        }
        else
        {
            musicIsOn = true;
            audioSource.Play();
        }
        PlayerPrefs.SetInt(ConstantNames.Music, musicIsOn ? 1 : 0);
    }
}

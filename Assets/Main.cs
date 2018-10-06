using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Main : MonoBehaviour
{

    private AudioSource audioSource;
    private SoundPlayer soundPlayer;

    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        soundPlayer = new SoundPlayer(audioSource);
        soundPlayer.PlayStartSound();

        soundPlayer.PlayBackgroundMusic();
    }
}

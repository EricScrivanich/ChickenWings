using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip[] cluckSounds;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip downJump;
    [SerializeField] private AudioClip bounce;
    [SerializeField] private AudioClip dash;
    [SerializeField] private AudioClip crack;
    [SerializeField] private AudioClip start;
    [SerializeField] private AudioClip scoreSound;
    [SerializeField] private AudioClip frozenSound;
    


    [SerializeField] private float cluckVolume = 0.7f;
    [SerializeField] private float deathSoundVolume = 1f;
    [SerializeField] private float damageSoundVolume = 1f;
    [SerializeField] private float downJumpVolume = 0.7f;
    [SerializeField] private float bounceVolume = 0.7f;
    [SerializeField] private float dashVolume = 0.7f;
    [SerializeField] private float crackVolume = 0.7f;
    [SerializeField] private float startVolume = 0.7f;
    [SerializeField] private float scoreVolume = 0.7f;
    [SerializeField] private float frozenVolume = 0.7f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        // Don't destroy AudioManager on scene change.
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        PlayMusic();
    }

    public void PlayMusic()
    {
        musicSource.Play(); // Play the music audio source
    }

    public void StopMusic()
    {
        musicSource.Stop(); // Stop the music audio source
    }

    public void PlayCluck()
    {
        // Choose a random cluck sound from the array
        int randomIndex = Random.Range(0, cluckSounds.Length);
        audioSource.PlayOneShot(cluckSounds[randomIndex], cluckVolume);
    }

    public void PlayDeathSound()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound, deathSoundVolume);
    }
    public void PlayDamageSound()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(damageSound, damageSoundVolume);
    }

    public void PlayDownJumpSound()
    {
        audioSource.PlayOneShot(downJump, downJumpVolume);
    }
   
    public void PlayBounceSound()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(bounce, bounceVolume);
    }

    public void PlayDashSound()
    {
        audioSource.PlayOneShot(dash, dashVolume);
    }
    public void PlayCrackSound()
    {
        audioSource.PlayOneShot(crack, crackVolume);
    }
     public void PlayStartSound()
    {
        audioSource.PlayOneShot(start, startVolume);
    }

    public void PlayScoreSound()
    {
        audioSource.PlayOneShot(scoreSound, scoreVolume);
    }
    public void PlayFrozenSound()
    {
        audioSource.PlayOneShot(frozenSound, frozenVolume);
    }
}

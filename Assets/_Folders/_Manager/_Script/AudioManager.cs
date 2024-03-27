using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public float musicPitch;
    public float sfxPitch;
    public static AudioManager instance;
    private float globalAudioSourcePitch;


    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ringPassSource;
    [SerializeField] private AudioClip[] cluckSounds;
    [SerializeField] private AudioClip[] flipSounds;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip downJump;
    [SerializeField] private AudioClip bounce;
    [SerializeField] private AudioClip dash;
    [SerializeField] private AudioClip crack;
    [SerializeField] private AudioClip start;
    [SerializeField] private AudioClip scoreSound;
    [SerializeField] private AudioClip frozenSound;
    [SerializeField] private AudioClip planeExplosionSound;
    [SerializeField] private AudioClip bombExplosionSound;
    [SerializeField] private AudioClip swordSlashSound;

    



    [SerializeField] private float cluckVolume = 0.7f;
    [SerializeField] private float flipVolume = 0.7f;
    [SerializeField] private float deathSoundVolume = 1f;
    [SerializeField] private float damageSoundVolume = 1f;
    [SerializeField] private float downJumpVolume = 0.7f;
    [SerializeField] private float bounceVolume = 0.7f;
    [SerializeField] private float dashVolume = 0.7f;
    [SerializeField] private float crackVolume = 0.7f;
    [SerializeField] private float startVolume = 0.7f;
    [SerializeField] private float scoreVolume = 0.7f;
    [SerializeField] private float frozenVolume = 0.7f;
    [SerializeField] private float planeExplosionVolume = 0.7f;
    [SerializeField] private float bombExplosionVolume = 0.7f;
    [SerializeField] private float swordSlashVolume = 0.7f;

    [SerializeField] private AudioClip ringSuccess;
    [SerializeField] private AudioClip bucketBurstSound;

    [SerializeField] private float ringSuccessVolume = 0.7f;
    [SerializeField] private AudioClip ringPassSound; // Assign in Unity Editor
    [SerializeField] private float ringPassVolume = 0.7f;
    [SerializeField] private float bucketBurstSoundVolume = 0.7f;

    float semitoneRatio = Mathf.Pow(2, 1f / 12f);
    private float ringPassPitch = 1f; // Default pitch

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
    public void SlowMotionPitch(bool isSlow)
    {
        if (isSlow)
        {
            musicSource.pitch = musicPitch;
            audioSource.pitch = sfxPitch;
            audioSource.bypassEffects = false;

        }
        else 
        {
            musicSource.pitch = 1;
            audioSource.pitch = 1;
            audioSource.bypassEffects = true;

        }
       
    }


    public void PlayRingPassSound()
    {
        ringPassSource.PlayOneShot(ringPassSound, ringPassVolume);
        ringPassSource.pitch *= semitoneRatio;
    }

    public void PlayBucketBurstSound()
    {
        audioSource.PlayOneShot(bucketBurstSound, bucketBurstSoundVolume);
    }

    public void ResetRingPassPitch()
    {
        ringPassSource.pitch = 1f; // Reset to default pitch
    }
    public void PlayRingSuccessSound()
    {
        audioSource.PlayOneShot(ringSuccess, ringSuccessVolume);
    }
    public void PlayPlaneExplosionSound()
    {
        audioSource.PlayOneShot(planeExplosionSound, planeExplosionVolume);
    }
    public void PlayBombExplosionSound()
    {
        
        audioSource.PlayOneShot(bombExplosionSound, bombExplosionVolume);
    }

    public void PlaySwordSlashSound()
    {
        audioSource.PlayOneShot(swordSlashSound, swordSlashVolume);
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
    public void PlayFlipSound()
    {
        // Choose a random cluck sound from the array
        int randomIndex = Random.Range(0, flipSounds.Length);
        

        audioSource.PlayOneShot(flipSounds[randomIndex], flipVolume);
    }

    public void PlayDeathSound()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound, deathSoundVolume);
    }
    public void PlayDamageSound()
    {
        
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

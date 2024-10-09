using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{


    public float musicPitch;
    public float sfxPitch;
    private float currentWindmillPitch;
    public float newPitchSlow = 1;
    public static AudioManager instance;
    private float globalAudioSourcePitch;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource nonSlowSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ringPassSource;
    [SerializeField] private AudioSource pigAudioSource;
    [SerializeField] private AudioSource windMillAudioSource;

    [Header("Chicken")]
    [SerializeField] private AudioClip[] cluckSounds;
    [SerializeField] private AudioClip[] flipSounds;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip downJump;
    [SerializeField] private AudioClip bounce;
    [SerializeField] private AudioClip dash;
    [SerializeField] private AudioClip start;
    [SerializeField] private AudioClip swordSlashSound;
    [SerializeField] private AudioClip frozenSound;
    [SerializeField] private AudioClip slowMotionEnter;
    [SerializeField] private AudioClip slowMotionExit;
    [SerializeField] private AudioClip shotgunReload;
    [SerializeField] private AudioClip shotgunBlast;
    [SerializeField] private AudioClip shotgunShell;
    [SerializeField] private AudioClip shotgunFutureBlast;
    [SerializeField] private AudioClip[] shotgunShells;


    [Header("Eggs")]
    [SerializeField] private AudioClip eggDropPop;
    [SerializeField] private AudioClip crack;
    [SerializeField] private AudioClip scoreSound;


    [Header("Rings")]
    [SerializeField] private AudioClip ringSuccess;
    [SerializeField] private AudioClip bucketBurstSound;
    [SerializeField] private AudioClip ringPassSound; // Assign in Unity Editor
    [SerializeField] private AudioClip bucketSuccessSound; // Assign in Unity Editor


    [Header("Pigs")]
    [SerializeField] private AudioClip[] pigDeath;
    [SerializeField] private AudioClip pigHammerSwing;
    [SerializeField] private AudioClip pigJetPack;
    [SerializeField] private AudioClip missileLaunch;
    [SerializeField] private AudioClip[] farts;
    [SerializeField] private AudioClip[] flappyPigCackles;

    // 0 ,1 are good, we are using 0
    // [SerializeField] private AudioClip[] longFarts;
    [SerializeField] private AudioClip longFart;



    [Header("Explosions")]
    [SerializeField] private AudioClip planeExplosionSound;
    [SerializeField] private AudioClip bombExplosionSound;
    [SerializeField] private AudioClip bombDropped;
    [SerializeField] private AudioClip airRaid;
    [SerializeField] private AudioClip flyOver;
    [SerializeField] private AudioClip bombLaunch;
    [SerializeField] private AudioClip windMill;

    [Header("UI")]
    [SerializeField] private AudioClip chamberClick;
    [SerializeField] private AudioClip chamberCock;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip levelFinishSound;
    [SerializeField] private AudioClip starHitSound;




    [Header("Volumes")]
    [SerializeField] private float longFartVolume;
    [SerializeField] private float levelFinishVolume;
    [SerializeField] private float bucketSuccessVolume;
    [SerializeField] private float starHitVolume;
    [SerializeField] private float fartVolume;
    [SerializeField] private float flappyPigCackleVolume;
    [SerializeField] private float bombDroppedVolume;
    [SerializeField] private float bombLaunchVolume;
    [SerializeField] private float windMillVolume;
    [SerializeField] private float airRaidVolume;
    [SerializeField] private float flyOverVolume;
    [SerializeField] private float errorVolume = 0.7f;
    [SerializeField] private float chamberClickVolume = 0.7f;
    [SerializeField] private float chamberCockVolume = 0.7f;
    [SerializeField] private float eggDropPopVolume = 0.7f;
    [SerializeField] private float shotGunShellVolume = 0.7f;

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
    [SerializeField] private float ringSuccessVolume = 0.7f;
    [SerializeField] private float ringPassVolume = 0.7f;
    [SerializeField] private float bucketBurstSoundVolume = 0.7f;

    [SerializeField] private float[] pigDeathVolumes;
    [SerializeField] private float pigHammerSwingVolume;
    [SerializeField] private float pigJetPackVolume;
    [SerializeField] private float missileLaunchVolume;


    [SerializeField] private float slowMoVolume;
    [SerializeField] private float shotgunBlastVolume;
    [SerializeField] private float shotgunReloadVolume;


    private bool canPlayJetPackNoise = true;
    private float jetPackNoiseTime;
    private readonly float minJetPackNoiseDelay = .15f;





    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

        BoundariesManager.isDay = false;

        // Don't destroy AudioManager on scene change.


    }

    void Start()
    {
        PlayMusic();
    }


    void Update()
    {

        if (!canPlayJetPackNoise)
        {
            jetPackNoiseTime += Time.deltaTime;

            if (jetPackNoiseTime > minJetPackNoiseDelay)
            {
                canPlayJetPackNoise = true;
                jetPackNoiseTime = 0;

            }

        }

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

    public void PlayLevelFinishSounds(int type)
    {
        if (type == 0)
            nonSlowSource.PlayOneShot(levelFinishSound, levelFinishVolume);

        else if (type == 1)
            nonSlowSource.PlayOneShot(starHitSound, starHitVolume);
    }

    public void PlayLongFarts()
    {
        // int rand = Random.Range(0, 3);

        // audioSource.PlayOneShot(longFarts[rand], .4f);
        audioSource.PlayOneShot(longFart, longFartVolume);

    }

    public void ChangeWindMillPitch(float change)
    {
        currentWindmillPitch = change;
        windMillAudioSource.pitch = change;
    }
    public void PlayWindMillSound()
    {
        windMillAudioSource.PlayOneShot(windMill, windMillVolume);
    }

    public void PlayFartSound()
    {
        int rand = Random.Range(0, 4);
        audioSource.PlayOneShot(farts[rand], fartVolume);
    }

    public void PlayFlappyPigCackleSound()
    {
        int rand = Random.Range(0, 4);
        audioSource.PlayOneShot(flappyPigCackles[rand], flappyPigCackleVolume);
    }

    public void PlayBombDroppedSound()
    {
        audioSource.PlayOneShot(bombDropped, bombDroppedVolume);
    }

    public void PlayBombLaunchSound()
    {
        audioSource.PlayOneShot(bombLaunch, bombLaunchVolume);
    }

    public void PlayAirRaidSiren()
    {
        audioSource.PlayOneShot(airRaid, airRaidVolume);

    }

    public void PlayFlyOver()
    {
        audioSource.PlayOneShot(flyOver, flyOverVolume);
    }

    public void PlayErrorSound()
    {
        nonSlowSource.PlayOneShot(errorSound, errorVolume);
    }
    public void SlowAudioPitch(float newPitch)
    {
        pigAudioSource.pitch = newPitch;
        audioSource.pitch = newPitch;
        newPitchSlow = newPitch;

        windMillAudioSource.pitch = currentWindmillPitch * newPitch;
    }


    public void PlayRingPassSound(int order)
    {
        ringPassSource.pitch = Mathf.Pow(2, (order - 1) / 12.0f) * newPitchSlow;
        ringPassSource.PlayOneShot(ringPassSound, ringPassVolume);

    }

    public void PlayEggDrop()
    {
        audioSource.PlayOneShot(eggDropPop, eggDropPopVolume);

    }

    public void PlayShotgunShell(int ind)
    {
        if (ind == 1)
        {
            ind = Random.Range(1, 3);
        }

        audioSource.PlayOneShot(shotgunShells[ind], shotGunShellVolume);
    }

    public void PlayChamberClick()
    {
        // int ran = Random.Range(0, 4);
        audioSource.PlayOneShot(chamberClick, chamberClickVolume);
    }
    public void PlayChamberCock()
    {

        audioSource.PlayOneShot(chamberCock, chamberCockVolume);
    }


    public void PlayShoutgunNoise(int type)
    {
        switch (type)
        {
            case (0):
                audioSource.PlayOneShot(shotgunBlast, shotgunBlastVolume);
                break;
            case (1):
                audioSource.PlayOneShot(shotgunReload, shotgunReloadVolume);


                break;
            case (2):
                audioSource.PlayOneShot(shotgunShell);


                break;
            case (3):
                audioSource.PlayOneShot(shotgunFutureBlast);

                break;
        }
    }

    public void PlaySlowMotionSound(bool enter)
    {
        if (enter) nonSlowSource.PlayOneShot(slowMotionEnter, slowMoVolume);
        else nonSlowSource.PlayOneShot(slowMotionExit, slowMoVolume);
    }

    public void PlayPigDeathSound(int type)
    {
        // 0 for small, 1 for big, 2 for hammer

        if (type >= 0 && type < pigDeath.Length)
        {
            pigAudioSource.PlayOneShot(pigDeath[type], pigDeathVolumes[type]);
        }
    }
    public void PlayPigHammerSwingSound()
    {
        pigAudioSource.PlayOneShot(pigHammerSwing, pigHammerSwingVolume);

    }
    public void PlayPigJetPackSound()
    {
        if (!canPlayJetPackNoise) return;

        pigAudioSource.PlayOneShot(pigJetPack, pigJetPackVolume);
        canPlayJetPackNoise = false;

    }

    public void PlayBucketBurstSound()
    {
        audioSource.PlayOneShot(bucketBurstSound, bucketBurstSoundVolume);
    }
    public void PlayBucketSuccessSound()
    {
        audioSource.PlayOneShot(bucketSuccessSound, bucketSuccessVolume);

    }

    public void ResetRingPassPitch()
    {
        ringPassSource.pitch = 1f; // Reset to default pitch
    }

    public void PlayMissileLaucnh()
    {
        pigAudioSource.PlayOneShot(missileLaunch, missileLaunchVolume);
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
    public void PlayCrackSound(int type = 0)
    {
        if (type == 0)
            audioSource.PlayOneShot(crack, crackVolume);
        else if (type == 1)
            audioSource.PlayOneShot(crack, crackVolume * 1.4f);
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

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {

            musicSource.Pause();
            ringPassSource.Pause();
            audioSource.Pause();
        }
        else
        {
            musicSource.UnPause();
            ringPassSource.UnPause();
            audioSource.UnPause();

        }
    }
}




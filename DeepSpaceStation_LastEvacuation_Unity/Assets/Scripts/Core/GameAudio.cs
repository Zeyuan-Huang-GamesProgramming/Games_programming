using System;
using UnityEngine;

[DefaultExecutionOrder(-500)]
public class GameAudio : MonoBehaviour
{
    private const int SampleRate = 44100;
    private const string BackgroundMusicResourcePath = "Audio/Music/TenseFutureLoop";

    public static GameAudio Instance { get; private set; }

    [SerializeField, Range(0f, 1f)] private float masterVolume = 0.8f;
    [SerializeField, Range(0f, 1f)] private float ambienceVolume = 0.18f;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.75f;

    private AudioSource ambienceSource;
    private AudioSource musicSource;
    private AudioSource effectsSource;
    private AudioSource scannerSource;
    private AudioClip ambienceLoop;
    private AudioClip musicLoop;
    private AudioClip scannerLoop;
    private AudioClip doorOpenClip;
    private AudioClip deniedClip;
    private AudioClip terminalStartClip;
    private AudioClip calibrationStepClip;
    private AudioClip calibrationErrorClip;
    private AudioClip repairCompleteClip;
    private AudioClip pickupClip;
    private AudioClip refillClip;
    private AudioClip healClip;
    private AudioClip rechargeClip;
    private AudioClip robotAlertClip;
    private AudioClip damageClip;
    private AudioClip warningPulseClip;
    private AudioClip missionStartClip;
    private AudioClip victoryClip;
    private AudioClip failureClip;
    private float nextOxygenWarningTime;
    private bool musicLoadLogged;
    private bool musicMissingLogged;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (FindObjectOfType<GameAudio>() != null)
        {
            return;
        }

        GameObject audioDirector = new GameObject("Audio Director");
        DontDestroyOnLoad(audioDirector);
        audioDirector.AddComponent<GameAudio>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        ambienceSource = AddSource(true, ambienceVolume);
        musicSource = AddSource(true, musicVolume);
        effectsSource = AddSource(false, 1f);
        scannerSource = AddSource(true, 0.11f);
        BuildProceduralClips();

        ambienceSource.clip = ambienceLoop;
        ambienceSource.Play();
        EnsureMusicPlaying();

        scannerSource.clip = scannerLoop;
    }

    private void Start()
    {
        EnsureMusicPlaying();
    }

    private void Update()
    {
        masterVolume = GameSettings.MasterVolume;
        ApplySourceVolumes();
        EnsureMusicPlaying();
    }

    public void SetScannerActive(bool active)
    {
        if (scannerSource == null)
        {
            return;
        }

        if (active && !scannerSource.isPlaying)
        {
            scannerSource.Play();
        }
        else if (!active && scannerSource.isPlaying)
        {
            scannerSource.Stop();
        }
    }

    public void UpdateOxygenWarning(float normalizedOxygen)
    {
        if (normalizedOxygen > 0.28f || Time.unscaledTime < nextOxygenWarningTime)
        {
            return;
        }

        float urgency = Mathf.InverseLerp(0.28f, 0.04f, normalizedOxygen);
        nextOxygenWarningTime = Time.unscaledTime + Mathf.Lerp(2.6f, 0.8f, urgency);
        Play(warningPulseClip, Mathf.Lerp(0.34f, 0.62f, urgency));
    }

    public void PlayDoorOpen() => Play(doorOpenClip, 0.7f);
    public void PlayDenied() => Play(deniedClip, 0.48f);
    public void PlayTerminalStart() => Play(terminalStartClip, 0.48f);
    public void PlayCalibrationStep() => Play(calibrationStepClip, 0.34f);
    public void PlayCalibrationError() => Play(calibrationErrorClip, 0.58f);
    public void PlayRepairComplete() => Play(repairCompleteClip, 0.68f);
    public void PlayPickup() => Play(pickupClip, 0.48f);
    public void PlayRefill() => Play(refillClip, 0.52f);
    public void PlayHeal() => Play(healClip, 0.52f);
    public void PlayRecharge() => Play(rechargeClip, 0.52f);
    public void PlayRobotAlert() => Play(robotAlertClip, 0.7f);
    public void PlayDamage() => Play(damageClip, 0.64f);
    public void PlayWarningPulse() => Play(warningPulseClip, 0.52f);
    public void PlayMissionStart() => Play(missionStartClip, 0.58f);
    public void PlayVictory() => Play(victoryClip, 0.82f);
    public void PlayFailure() => Play(failureClip, 0.72f);

    private AudioSource AddSource(bool loop, float volume)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = loop;
        source.spatialBlend = 0f;
        source.ignoreListenerPause = true;
        source.volume = volume * masterVolume;
        return source;
    }

    private void ApplySourceVolumes()
    {
        if (ambienceSource != null)
        {
            ambienceSource.volume = ambienceVolume * masterVolume;
        }

        if (musicSource != null)
        {
            musicSource.volume = musicVolume * masterVolume;
        }

        if (scannerSource != null)
        {
            scannerSource.volume = 0.11f * masterVolume;
        }
    }

    private void EnsureMusicPlaying()
    {
        if (musicSource == null)
        {
            return;
        }

        if (musicLoop == null)
        {
            musicLoop = LoadBackgroundMusicClip();
            if (musicLoop == null)
            {
                if (!musicMissingLogged)
                {
                    Debug.LogWarning("Deep Space Station BGM could not be loaded from Resources/" + BackgroundMusicResourcePath + ".");
                    musicMissingLogged = true;
                }

                return;
            }
        }

        if (musicSource.clip != musicLoop)
        {
            musicSource.clip = musicLoop;
        }

        musicSource.loop = true;
        musicSource.volume = musicVolume * masterVolume;
        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }

        if (!musicLoadLogged)
        {
            Debug.Log("Deep Space Station BGM playing: " + musicLoop.name + " | volume " + musicSource.volume.ToString("0.00"));
            musicLoadLogged = true;
            musicMissingLogged = false;
        }
    }

    private void Play(AudioClip clip, float volume)
    {
        if (effectsSource != null && clip != null)
        {
            effectsSource.PlayOneShot(clip, volume * masterVolume);
        }
    }

    private void BuildProceduralClips()
    {
        ambienceLoop = CreateLoop("Station Atmosphere", 8f, t =>
            Mathf.Sin(2f * Mathf.PI * 37f * t) * 0.19f
            + Mathf.Sin(2f * Mathf.PI * 55f * t) * 0.1f
            + Mathf.Sin(2f * Mathf.PI * 110f * t) * 0.025f
            + Mathf.Sin(2f * Mathf.PI * 83f * t) * 0.012f);

        musicLoop = LoadBackgroundMusicClip();

        scannerLoop = CreateLoop("Scanner Sweep", 1f, t =>
        {
            float pulse = 0.35f + Mathf.Pow(Mathf.Sin(2f * Mathf.PI * 2f * t), 2f) * 0.65f;
            return (Mathf.Sin(2f * Mathf.PI * 440f * t) * 0.25f
                + Mathf.Sin(2f * Mathf.PI * 880f * t) * 0.09f) * pulse;
        });

        doorOpenClip = CreateOneShot("Door Slide", 0.82f, t =>
        {
            float motor = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(98f, 46f, t / 0.82f) * t) * 0.34f;
            float air = Noise(t * 97f) * 0.18f;
            return motor + air;
        });
        deniedClip = CreateOneShot("Access Denied", 0.3f, t =>
            Mathf.Sin(2f * Mathf.PI * (220f - t * 280f) * t) * 0.56f);
        terminalStartClip = CreateOneShot("Terminal Wake", 0.32f, t =>
            Mathf.Sin(2f * Mathf.PI * (260f + t * 880f) * t) * 0.38f);
        calibrationStepClip = CreateOneShot("Calibration Tick", 0.1f, t =>
            Mathf.Sin(2f * Mathf.PI * 880f * t) * 0.46f);
        calibrationErrorClip = CreateOneShot("Calibration Error", 0.42f, t =>
            (Mathf.Sin(2f * Mathf.PI * 156f * t) + Mathf.Sin(2f * Mathf.PI * 131f * t)) * 0.31f);
        repairCompleteClip = CreateOneShot("Repair Complete", 0.86f, t =>
        {
            float note = t < 0.24f ? 392f : t < 0.48f ? 523.25f : 783.99f;
            return Mathf.Sin(2f * Mathf.PI * note * t) * 0.42f;
        });
        pickupClip = CreateOneShot("Item Collected", 0.28f, t =>
            Mathf.Sin(2f * Mathf.PI * (420f + t * 1500f) * t) * 0.44f);
        refillClip = CreateOneShot("Oxygen Refill", 0.72f, t =>
            (Mathf.Sin(2f * Mathf.PI * (260f + t * 540f) * t) * 0.3f) + (Noise(t * 53f) * 0.08f));
        healClip = CreateOneShot("Medkit Used", 0.62f, t =>
            Mathf.Sin(2f * Mathf.PI * (330f + t * 520f) * t) * 0.36f);
        rechargeClip = CreateOneShot("Battery Recharged", 0.58f, t =>
            Mathf.Sin(2f * Mathf.PI * (510f + Mathf.PingPong(t * 900f, 420f)) * t) * 0.36f);
        robotAlertClip = CreateOneShot("Security Alert", 1.12f, t =>
        {
            float alarm = Mathf.Sin(2f * Mathf.PI * (Mathf.PingPong(t * 760f, 180f) + 520f) * t);
            return alarm * 0.45f;
        });
        damageClip = CreateOneShot("Suit Impact", 0.28f, t =>
            Noise(t * 131f) * 0.43f + Mathf.Sin(2f * Mathf.PI * 72f * t) * 0.32f);
        warningPulseClip = CreateOneShot("Oxygen Warning", 0.48f, t =>
        {
            float first = t < 0.16f ? Mathf.Sin(2f * Mathf.PI * 620f * t) : 0f;
            float second = t > 0.25f ? Mathf.Sin(2f * Mathf.PI * 620f * (t - 0.25f)) : 0f;
            return (first + second) * 0.46f;
        });
        missionStartClip = CreateOneShot("Protocol Start", 1f, t =>
            Mathf.Sin(2f * Mathf.PI * (120f + t * 540f) * t) * 0.4f);
        victoryClip = CreateOneShot("Evacuation Success", 2f, t =>
        {
            float chord = Mathf.Sin(2f * Mathf.PI * 261.63f * t)
                + Mathf.Sin(2f * Mathf.PI * 329.63f * t)
                + Mathf.Sin(2f * Mathf.PI * 392f * t);
            return chord * 0.18f;
        });
        failureClip = CreateOneShot("Mission Failed", 1.7f, t =>
        {
            float frequency = Mathf.Lerp(196f, 73f, t / 1.7f);
            return (Mathf.Sin(2f * Mathf.PI * frequency * t) + Mathf.Sin(2f * Mathf.PI * frequency * 0.5f * t)) * 0.25f;
        });
    }

    private static AudioClip LoadBackgroundMusicClip()
    {
        AudioClip clip = Resources.Load<AudioClip>(BackgroundMusicResourcePath);
        if (clip != null)
        {
            return clip;
        }

        AudioClip[] musicClips = Resources.LoadAll<AudioClip>("Audio/Music");
        if (musicClips != null && musicClips.Length > 0)
        {
            return musicClips[0];
        }

        return null;
    }

    private static AudioClip CreateLoop(string clipName, float duration, Func<float, float> wave)
    {
        return CreateClip(clipName, duration, wave, false);
    }

    private static AudioClip CreateOneShot(string clipName, float duration, Func<float, float> wave)
    {
        return CreateClip(clipName, duration, wave, true);
    }

    private static AudioClip CreateClip(string clipName, float duration, Func<float, float> wave, bool useEnvelope)
    {
        int sampleCount = Mathf.CeilToInt(duration * SampleRate);
        float[] samples = new float[sampleCount];
        for (int index = 0; index < sampleCount; index++)
        {
            float t = index / (float)SampleRate;
            float sample = wave(t);
            if (useEnvelope)
            {
                float attack = Mathf.Clamp01(t / 0.012f);
                float release = Mathf.Clamp01((duration - t) / Mathf.Min(0.16f, duration * 0.32f));
                sample *= Mathf.Min(attack, release);
            }

            samples[index] = Mathf.Clamp(sample, -0.95f, 0.95f);
        }

        AudioClip clip = AudioClip.Create(clipName, sampleCount, 1, SampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private static float Noise(float seed)
    {
        return Mathf.PerlinNoise(seed, 0.381f) * 2f - 1f;
    }
}

public static class GameSettings
{
    private const string MasterVolumeKey = "DSS.Settings.MasterVolume";
    private const string MouseSensitivityKey = "DSS.Settings.MouseSensitivity";

    public const float DefaultMasterVolume = 0.8f;
    public const float MinMouseSensitivity = 0.4f;
    public const float MaxMouseSensitivity = 5f;

    public static float MasterVolume => Mathf.Clamp01(PlayerPrefs.GetFloat(MasterVolumeKey, DefaultMasterVolume));

    public static float GetMouseSensitivity(float defaultValue)
    {
        return Mathf.Clamp(PlayerPrefs.GetFloat(MouseSensitivityKey, defaultValue), MinMouseSensitivity, MaxMouseSensitivity);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ApplySavedAudioSettings()
    {
        AudioListener.volume = MasterVolume;
    }

    public static float SetMasterVolume(float volume)
    {
        float value = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(MasterVolumeKey, value);
        PlayerPrefs.Save();
        AudioListener.volume = value;
        return value;
    }

    public static float SetMouseSensitivity(float sensitivity)
    {
        float value = Mathf.Clamp(sensitivity, MinMouseSensitivity, MaxMouseSensitivity);
        PlayerPrefs.SetFloat(MouseSensitivityKey, value);
        PlayerPrefs.Save();
        return value;
    }
}

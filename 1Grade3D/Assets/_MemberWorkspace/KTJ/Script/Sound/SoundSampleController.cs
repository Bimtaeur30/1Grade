using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public sealed class SoundSampleController : MonoBehaviour
{
    private const string MasterKey = "MASTER_VOLUME";
    private const string SfxKey = "SFX_VOLUME";
    private const string BgmKey = "BGM_VOLUME";

    [Header("Audio")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioClip testSfx;

    [Header("UI")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Text masterValueText;
    [SerializeField] private Text sfxValueText;
    [SerializeField] private Text bgmValueText;

    private AudioSource _testSource;

    private void Awake()
    {
        _testSource = GetComponent<AudioSource>();
        _testSource.playOnAwake = false;
        _testSource.outputAudioMixerGroup = sfxGroup;

        InitializeSlider(masterSlider, PlayerPrefs.GetFloat(MasterKey, 100f), SetMasterVolume);
        InitializeSlider(sfxSlider, PlayerPrefs.GetFloat(SfxKey, 100f), SetSfxVolume);
        InitializeSlider(bgmSlider, PlayerPrefs.GetFloat(BgmKey, 100f), SetBgmVolume);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
            PlayTestSfx();
    }

    public void PlayTestSfx()
    {
        if (testSfx == null)
        {
            Debug.LogWarning("Test SFX is not assigned.", this);
            return;
        }

        _testSource.PlayOneShot(testSfx);
    }

    public void SetMasterVolume(float value) => SetVolume("MasterVolume", MasterKey, value, masterValueText);
    public void SetSfxVolume(float value) => SetVolume("SFXVolume", SfxKey, value, sfxValueText);
    public void SetBgmVolume(float value) => SetVolume("BGMVolume", BgmKey, value, bgmValueText);

    private static void InitializeSlider(Slider slider, float value, UnityEngine.Events.UnityAction<float> callback)
    {
        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.wholeNumbers = true;
        slider.SetValueWithoutNotify(Mathf.Clamp(value, 0f, 100f));
        slider.onValueChanged.AddListener(callback);
        callback(slider.value);
    }

    private void SetVolume(string parameter, string preferenceKey, float value, Text valueText)
    {
        value = Mathf.Clamp(value, 0f, 100f);
        mixer.SetFloat(parameter, value <= 0f ? -80f : Mathf.Log10(value / 100f) * 20f);
        valueText.text = $"{value:0}";
        PlayerPrefs.SetFloat(preferenceKey, value);
    }

    private void OnDestroy()
    {
        PlayerPrefs.Save();
    }
}

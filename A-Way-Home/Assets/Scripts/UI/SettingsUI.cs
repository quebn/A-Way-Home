using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle muteToggle;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider ambienceSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI masterValue;
    [SerializeField] private TextMeshProUGUI bgmValue;
    [SerializeField] private TextMeshProUGUI ambienceValue;
    [SerializeField] private TextMeshProUGUI sfxValue;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Toggle leaderboardsToggle;
    [SerializeField] private Toggle levelsToggle;

    private void OnEnable()
    {
        InitValue();
    }

    private void OnDisable()
    {
        audioMixer.SetFloat("master", GameData.Instance.isMuted ? -80 :  Mathf.Log10(GameData.Instance.master * .01f) * 20);
    }

    private void InitValue()
    {
        this.fullscreenToggle.isOn = GameData.Instance.isFullscreen;
        this.muteToggle.isOn = GameData.Instance.isMuted;
        this.masterSlider.value = GameData.Instance.master;
        this.masterValue.text = this.masterSlider.value.ToString();
        this.bgmSlider.value = GameData.Instance.bgm;
        this.bgmValue.text = this.bgmSlider.value.ToString();
        this.ambienceSlider.value = GameData.Instance.ambience;
        this.ambienceValue.text = this.ambienceSlider.value.ToString();
        this.sfxSlider.value = GameData.Instance.sfx;
        this.sfxValue.text = this.sfxSlider.value.ToString();
        this.leaderboardsToggle.isOn = false;
        this.levelsToggle.isOn = false;
    }

    public void ChangeMaster()
    {
        masterValue.text = masterSlider.value.ToString();

        if(!muteToggle.isOn)
            audioMixer.SetFloat("master", Mathf.Log10(masterSlider.value * .01f) * 20);
    }

    public void ChangeBGM()
    {
        bgmValue.text = bgmSlider.value.ToString();
        audioMixer.SetFloat("bgm", Mathf.Log10(bgmSlider.value * .01f) * 20);
    }


    public void ChangeAmbience()
    {
        ambienceValue.text = ambienceSlider.value.ToString();
        audioMixer.SetFloat("ambience", Mathf.Log10(ambienceSlider.value * .01f) * 20);
    }

    public void ChangeSFX()
    {
        sfxValue.text = sfxSlider.value.ToString();
        audioMixer.SetFloat("sfx", Mathf.Log10(sfxSlider.value * .01f) * 20);
    }

    public void ToggleAudio()
    {
        audioMixer.SetFloat("master", muteToggle.isOn ? -80 :  Mathf.Log10(masterSlider.value * .01f) * 20);
    }

    public void SaveSettings()
    {
        GameData.Instance.isFullscreen = this.fullscreenToggle.isOn;
        GameData.Instance.isMuted = this.muteToggle.isOn;
        GameData.Instance.master = (uint)this.masterSlider.value;
        GameData.Instance.bgm = (uint)this.bgmSlider.value;
        GameData.Instance.ambience = (uint)this.ambienceSlider.value;
        GameData.Instance.sfx = (uint)this.sfxSlider.value;
        if(levelsToggle.isOn)
            GameData.Instance.unlockedLevels = new List<string>{"Stage1Level1"};
        if(leaderboardsToggle.isOn)
            GameData.Instance.leaderboards = new List<PlayerScoreData>();
        Screen.fullScreen = GameData.Instance.isFullscreen;
        SaveSystem.SaveGameData();
        MainMenuUI.Instance.CloseSettingsWindow();
    }

    public void RevertValues()
    {
        this.fullscreenToggle.isOn = true;
        this.muteToggle.isOn = false;
        this.masterSlider.value = 100;
        this.masterValue.text = this.masterSlider.value.ToString();
        this.bgmSlider.value = 100;
        this.bgmValue.text = this.bgmSlider.value.ToString();
        this.ambienceSlider.value = 100;
        this.ambienceValue.text = this.ambienceSlider.value.ToString();
        this.sfxSlider.value = 100;
        this.sfxValue.text = this.sfxSlider.value.ToString();
        this.leaderboardsToggle.isOn = false;
        this.levelsToggle.isOn = false;
        foreach (InputActionMap map in inputActions.actionMaps)
            map.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey("rebinds");
    }
}

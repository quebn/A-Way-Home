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

    private void Start()
    {
        InitValue();
    }

    private void InitValue()
    {
        GameData data = GameData.Instance;
        this.fullscreenToggle.isOn = data.isFullscreen;
        this.muteToggle.isOn = data.isMuted;
        this.masterSlider.value = data.master;
        this.masterValue.text = this.masterSlider.value.ToString();
        this.bgmSlider.value = data.bgm;
        this.bgmValue.text = this.bgmSlider.value.ToString();
        this.ambienceSlider.value = data.ambience;
        this.ambienceValue.text = this.ambienceSlider.value.ToString();
        this.sfxSlider.value = data.sfx;
        this.sfxValue.text = this.sfxSlider.value.ToString();
    }

    public void ChangeMaster()
    {
        GameData.Instance.master = (uint)masterSlider.value;
        masterValue.text = GameData.Instance.master.ToString();
        audioMixer.SetFloat("master", Mathf.Log10(GameData.Instance.master * .01f) * 20);
    }

    public void ChangeBGM()
    {
        GameData.Instance.bgm = (uint)bgmSlider.value;
        bgmValue.text = GameData.Instance.bgm.ToString();
        audioMixer.SetFloat("bgm", Mathf.Log10(GameData.Instance.bgm * .01f) * 20);
        // Debug.Log(GameData.Instance.bgm);
    }


    public void ChangeAmbience()
    {
        GameData.Instance.ambience = (uint)ambienceSlider.value;
        ambienceValue.text = GameData.Instance.ambience.ToString();
        audioMixer.SetFloat("ambience", Mathf.Log10(GameData.Instance.ambience * .01f) * 20);
    }

    public void ChangeSFX()
    {
        GameData.Instance.sfx = (uint)sfxSlider.value;
        sfxValue.text = GameData.Instance.sfx.ToString();
        audioMixer.SetFloat("sfx", Mathf.Log10(GameData.Instance.sfx * .01f) * 20);
    }

    public void ToggleAudio()
    {
        GameData.Instance.isMuted = muteToggle.isOn;
        Debug.Log(GameData.Instance.isMuted);
        audioMixer.SetFloat("master", GameData.Instance.isMuted? -80 :  Mathf.Log10(GameData.Instance.master * .01f) * 20);
    }

    public void SaveSettings()
    {
        GameData data = GameData.Instance;
        data.isFullscreen = this.fullscreenToggle.isOn;
        data.isMuted = this.muteToggle.isOn;
        data.master = (uint)this.masterSlider.value;
        data.bgm = (uint)this.bgmSlider.value;
        data.ambience = (uint)this.ambienceSlider.value;
        data.sfx = (uint)this.sfxSlider.value;
        Debug.Log("Settings Saved!");
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

        foreach (InputActionMap map in inputActions.actionMaps)
            map.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey("rebinds");
    }
}

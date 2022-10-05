using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle animationToggle;
    [SerializeField] private Slider audioSlider;
    [SerializeField] private Slider gameSpeedSlider;
    [SerializeField] private TextMeshProUGUI audioValue;
    [SerializeField] private TextMeshProUGUI gameSpeedValue;
    // TODO: Add Keybinds group

    private void Start()
    {
        InitValue();        
    }

    private void InitValue()
    {
        GameData data = GameData.Instance;
        this.fullscreenToggle.isOn = data.isFullscreen;
        this.animationToggle.isOn = data.hasAnimations;
        this.audioSlider.value = data.audio;
        this.audioValue.text = this.audioSlider.value.ToString();
        this.gameSpeedSlider.value = data.gameSpeed;
        this.gameSpeedValue.text = this.gameSpeedSlider.value.ToString();
    }

    public void ChangeAudio()
    {
        GameData.Instance.audio = (uint)audioSlider.value;
        audioValue.text = audioSlider.value.ToString();
    }

    public void ChangeGameSpeed()
    {
        gameSpeedValue.text = this.gameSpeedSlider.value.ToString();
    }
    public void SaveSettings()
    {
        GameData data = GameData.Instance;
        data.isFullscreen = this.fullscreenToggle.isOn;
        data.hasAnimations = this.animationToggle.isOn;
        data.gameSpeed = (uint)this.gameSpeedSlider.value;
        Debug.Log("Settings Saved!");
        SaveSystem.SaveGameData();
        MainMenuUI.Instance.CloseSettingsWindow();
    }
    public void RevertValues()
    {
        this.fullscreenToggle.isOn = true;
        this.animationToggle.isOn = true;
        this.audioSlider.value = 100;
        this.audioValue.text = this.audioSlider.value.ToString();
        this.gameSpeedSlider.value = 20;
        this.gameSpeedValue.text = this.gameSpeedSlider.value.ToString();
    }
}

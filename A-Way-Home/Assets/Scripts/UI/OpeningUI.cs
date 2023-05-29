using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class OpeningUI : MonoBehaviour
{
    [SerializeField] private GameObject[] texts;
    [SerializeField] private GameObject buttonSkip;
    [SerializeField] private GameObject continueText;
    [SerializeField] private AudioMixer audioMixer;

    private uint count;
    private Queue<GameObject> storyQueue;
    private GameObject currentText;
    private bool endEarly = false;

    private void Awake()
    {
        if (GameData.Instance == null)
            GameData.InitGameDataInstance();
    }

    private void LoadAudioSettings()
    {
        if(GameData.Instance.isMuted)
            audioMixer.SetFloat("master", -80);
        else
        {
            audioMixer.SetFloat("master", Mathf.Log10(GameData.Instance.master * .01f) * 20);
            audioMixer.SetFloat("bgm", Mathf.Log10(GameData.Instance.bgm * .01f) * 20);
            audioMixer.SetFloat("ambience", Mathf.Log10(GameData.Instance.ambience * .01f) * 20);
            audioMixer.SetFloat("sfx", Mathf.Log10(GameData.Instance.sfx * .01f) * 20);
        }
    }

    private void Start()
    {
        Screen.fullScreen = GameData.Instance.isFullscreen;
        LoadAudioSettings();
        storyQueue = new Queue<GameObject>();
        for(int i = 0; i < texts.Length; i++)
            storyQueue.Enqueue(texts[i]);
        StartCoroutine(PlayStory());
    }

    private void Update()
    {
        if(Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
            Continue();
    }

    private IEnumerator PlayStory()
    {
        count = 0;
        while(true)
        {
            count++;
            endEarly = false;
            currentText = storyQueue.Dequeue();
            if(!storyQueue.Contains(currentText))
                storyQueue.Enqueue(currentText);
            Debug.Log($"Count: {count}");
            Animator animator = currentText.GetComponent<Animator>();
            currentText.SetActive(true);
            UpdateSkipButton();
            yield return new WaitUntil(() => endEarly || animator.GetCurrentAnimatorStateInfo(0).IsName("End"));
            currentText.SetActive(false);
        }
    }

    private void UpdateSkipButton()
    {
        if(count < 6)
            return;
        Debug.Log($"Updating Messages");
        buttonSkip.SetActive(false);
        continueText.SetActive(true);
    }

    private void Continue()
    {
        if(count >= 6)
            SceneManager.LoadScene("MainMenu");
        endEarly = Keyboard.current.anyKey.wasPressedThisFrame ;
    }

    public void SkipStory()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

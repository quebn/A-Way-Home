using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class OpeningUI : MonoBehaviour
{
    [SerializeField] private GameObject[] texts;
    [SerializeField] private GameObject buttonSkip;
    [SerializeField] private GameObject continueText;

    private uint count;
    private Queue<GameObject> storyQueue;
    private GameObject currentText;
    private bool endEarly = false;

    private void Start()
    {
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

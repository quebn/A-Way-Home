using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class EndingUI : MonoBehaviour
{
    [SerializeField] private GameObject[] texts;
    [SerializeField] private GameObject buttonSkip;
    [SerializeField] private TextMeshProUGUI[] messages;
 
    private uint count;
    private Queue<GameObject> storyQueue;
    private GameObject currentText;
    private bool endEarly = false;

    private void Awake()
    {
        if (GameData.Instance == null)
            GameData.InitGameDataInstance();
    }

    private void Start()
    {
        SetNameText();
        storyQueue = new Queue<GameObject>();
        for(int i = 0; i < texts.Length; i++)
            storyQueue.Enqueue(texts[i]);
        StartCoroutine(PlayStory());
    }

    private void SetNameText()
    {
        messages[0].text = $"	Having Collected enough essences {GameData.selectedCharacter} was able to create a portal powerful enough to go back home to the AstraWorld. his friends and family who was looking for him was relieved seeing {GameData.selectedCharacter} was alive and well.";
        messages[1].text = $"	{GameData.selectedCharacter} told everyone the journey and the scenery of the unfamiliar world. Everyone was shocked because despite the world {GameData.selectedCharacter} described, he came back safe and sound almost as if there was a guardian angel protecting {GameData.selectedCharacter} along the way home.";
    }

    private void Update()
    {
        if(Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
            Continue();
    }

    private IEnumerator PlayStory()
    {
        count = 0;
        while(storyQueue.Count != 0)
        {
            count++;
            endEarly = false;
            currentText = storyQueue.Dequeue();
            Debug.Log($"Count: {count}");
            Animator animator = currentText.GetComponent<Animator>();
            currentText.SetActive(true);
            yield return new WaitUntil(() => endEarly || animator.GetCurrentAnimatorStateInfo(0).IsName("End"));
            currentText.SetActive(false);
        }
        SceneManager.LoadScene("EndScene");
    }

    private void Continue()
    {
        endEarly = true;
    }

    public void SkipStory()
    {
        SceneManager.LoadScene("EndScene");
    }
}

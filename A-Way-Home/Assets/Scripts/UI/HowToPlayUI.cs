using UnityEngine;

public class HowToPlayUI : MonoBehaviour
{
    [SerializeField] private GameObject prev;
    [SerializeField] private GameObject next;
    [SerializeField] private GameObject[] slides;
    
    private int index = 0;
    private GameObject currentSlide;

    private void Start()
    {
        index = 0;
        UpdateSlide();
    }

    public void Next()
    {
        index++;
        if(index >= slides.Length)
            index = 0;
        UpdateSlide();
    }
    public void Prev()
    {
        if(index > 0)
            index--;
        UpdateSlide();
    }

    private void UpdateSlide()
    {
        Debug.LogWarning(index);
        if(currentSlide != null)
            currentSlide.SetActive(false);
        currentSlide = slides[index];
        currentSlide.SetActive(true);
    }
}
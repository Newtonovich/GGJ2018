using UnityEngine;

public class OpeningPanel : MonoBehaviour
{
    [SerializeField] private GameObject firstParagraph;
    [SerializeField] private GameObject secondParagraph;
    [SerializeField] private GameObject explenation;
    
    void OnEnable()
    {
        firstParagraph.SetActive(true);
        secondParagraph.SetActive(false);
    }

    public void Next()
    {
        if(secondParagraph.activeInHierarchy)
        {
            explenation.SetActive(true);
            GameManager.Instance.SetGameWindows();
        }
        else
        {
            firstParagraph.SetActive(false);
            secondParagraph.SetActive(true);
        }

    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.IO;

public class DeckSlot : MonoBehaviour,IPointerClickHandler
{
    [SerializeField, Range(1, 9)]private int deckIndex = 1;
    [SerializeField] private GameObject finalDeckImage;
    [SerializeField] private bool canOpenDeckEditor = true;
    private void Start()
    {
        UpdateFinalDeckImage();
    }
    private void UpdateFinalDeckImage()
    {
        if (finalDeckImage == null)
            return;
        string filePath = Path.Combine(Application.persistentDataPath,$"deck_{deckIndex}.json");
        bool deckIsSaved = File.Exists(filePath);
        finalDeckImage.SetActive(deckIsSaved);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canOpenDeckEditor)
            return;
        DeckSelection.SelectDeck(deckIndex);
        Debug.Log($"{deckIndex}번 덱을 선택했습니다.");
        SceneManager.LoadScene("DeckEditorScene");
    }
}
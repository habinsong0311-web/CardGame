using System.IO;
using UnityEngine;
using TMPro;

public class PlayerSaveManager : MonoBehaviour
{
    [SerializeField] private TMP_Text selectionText;
    [SerializeField] private GameObject selectionDeckImage;
    private string filePath;
    private PlayerSaveData playerData;
    public int MainDeckIndex => playerData.mainDeckIndex;
    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath,"player_data.json");
        LoadPlayerData();
        UpdateSelectionText();
    }
    public void SelectMainDeck(int deckIndex)
    {
        playerData.mainDeckIndex = deckIndex;
        SavePlayerData();
        UpdateSelectionText();
        Debug.Log($"{deckIndex}번 덱을 메인 덱으로 설정했습니다.");
    }
    private void SavePlayerData()
    {
        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(filePath, json);
    }
    private void LoadPlayerData()
    {
        if (!File.Exists(filePath))
        {
            playerData = new PlayerSaveData();
            return;
        }
        string json = File.ReadAllText(filePath);
        playerData = JsonUtility.FromJson<PlayerSaveData>(json);
        if (playerData == null)
        {
            playerData = new PlayerSaveData();
        }
    }
    private void UpdateSelectionText()
    {
        bool hasSelectedDeck = playerData.mainDeckIndex != -1;
        if (selectionDeckImage != null)
        {
            selectionDeckImage.SetActive(hasSelectedDeck);
        }
        if (selectionText == null)
            return;
        if (!hasSelectedDeck)
        {
            selectionText.text = "선택된 덱 없음";
            return;
        }
        selectionText.text = $"현재 {playerData.mainDeckIndex}번 덱 선택";
    }
}
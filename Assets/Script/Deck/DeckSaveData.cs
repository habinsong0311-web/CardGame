using System.Collections.Generic;

[System.Serializable]
public class DeckCardSaveData
{
    public string cardId;
    public int count;
}

[System.Serializable]
public class DeckSaveData
{
    public int deckIndex;
    public string deckName;
    public List<DeckCardSaveData> cards = new List<DeckCardSaveData>();
}
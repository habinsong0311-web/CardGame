

public class DeckSelection
{
    public static int SelectedDeckIndex {get; private set;} = 1;

    public static void SelectDeck(int deckIndex)
    {
        SelectedDeckIndex = deckIndex;
    }
}

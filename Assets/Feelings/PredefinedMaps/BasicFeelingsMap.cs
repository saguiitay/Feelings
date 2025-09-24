public class BasicFeelingsMap : FeelingsMap
{
    public const string Mad = "Mad";
    public const string Scared = "Scared";
    public const string Joyful = "Joyful";
    public const string Powerful = "Powerful";
    public const string Peaceful = "Peaceful";
    public const string Sad = "Sad";

    public BasicFeelingsMap()
    {
        SetEffect(Mad, Powerful, -1);
        SetEffect(Powerful, Mad, -1);
        SetEffect(Scared, Peaceful, -1);
        SetEffect(Peaceful, Scared, -1);
        SetEffect(Joyful, Sad, -1);
        SetEffect(Sad, Joyful, -1);

        SetEffect(Mad, Sad, 0.1f);
        SetEffect(Mad, Scared, 0.1f);
        SetEffect(Scared, Mad, 0.1f);
        SetEffect(Sad, Mad, 0.1f);

        SetEffect(Powerful, Peaceful, 0.1f);
        SetEffect(Powerful, Joyful, 0.1f);
        SetEffect(Joyful, Powerful, 0.1f);
        SetEffect(Peaceful, Powerful, 0.1f);
    }
}
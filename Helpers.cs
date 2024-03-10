namespace AudioPlayerBlazor;

public static class Helpers
{

    public static string ToTimespanDisplay(this TimeSpan ts)
    {
        return ts.Hours >= 1 ? ts.ToString("hh':'mm':'ss") : ts.ToString("mm':'ss");
    }
}
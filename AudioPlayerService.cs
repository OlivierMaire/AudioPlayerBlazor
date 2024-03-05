namespace AudioPlayerBlazor;

public class AudioPlayerService {

    public bool IsPlaying {get;set;} = false;

    public event Action<bool>? OnPlaying;
    
    public void SetPlaying(bool value)
    {
        Console.WriteLine($"Set Playing to {value}");
        IsPlaying = value;
        OnPlaying?.Invoke(IsPlaying);
    }

}
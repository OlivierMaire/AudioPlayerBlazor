
namespace AudioPlayerBlazor;

public class QueuePlayerService 
{
public List<AudioMetadata> Queue { get; } = [];

    public event Action? OnQueueChanged;
    public void AddToQueue(AudioMetadata metadata)
    {
        Console.WriteLine($"AudioPlayer received {metadata.AudioSourceUrl}");
        Queue.Add(metadata);
        OnQueueChanged?.Invoke();
    }

}

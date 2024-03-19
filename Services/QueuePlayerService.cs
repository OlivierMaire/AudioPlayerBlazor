
namespace AudioPlayerBlazor;

public class QueuePlayerService
{
    public List<QueueRecord> Queue { get; } = [];

    public event Action? OnQueueChanged;
    public void AddToQueue(AudioMetadata metadata, string BookId, uint timestamp)
    {
        Console.WriteLine($"AudioPlayer received {metadata.AudioSourceUrl}");
        Queue.Add(new QueueRecord(metadata, BookId, timestamp));
        OnQueueChanged?.Invoke();
    }

    public void Clear()
    {
        Queue.Clear();
    }


}

    public record QueueRecord(AudioMetadata Data, string BookId, uint LastTimestamp);

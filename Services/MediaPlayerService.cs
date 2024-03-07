
using Microsoft.JSInterop;

namespace AudioPlayerBlazor.Services;

internal class MediaPlayerService
{
    private readonly QueuePlayerService queueService;
    private readonly PlayerInterop interop ;

    public uint _currentTime = 0;
    public TimeSpan CurrentTime => TimeSpan.FromMilliseconds(_currentTime);

    public Action? OnTimestampUpdated;

    public bool IsPlaying { get; set; } = false;

    // public event Action<bool>? OnPlaying;

    // public async Task SetPlaying(bool value, )
    // {
    //     Console.WriteLine($"Set Playing to {value}");
    //     IsPlaying = value;

    //     if (value)
    //         await interop.Play();

    //     OnPlaying?.Invoke(IsPlaying);
    // }

    public MediaPlayerService(QueuePlayerService queueService, PlayerInterop interop)
    {
       this.queueService = queueService;
         this.interop = interop;

    this.interop.OnTimestampUpdated += TimeUpdated;
    }

    public void TimeUpdated(float time)
    {
        _currentTime = (uint)(time * 1000);
        OnTimestampUpdated?.Invoke();
    }

        public async Task Init( )
    {
        await interop.Init();
    }

    public async Task<AudioMetadata> getCurrentFileInQueue()
    {
        var meta = queueService.Queue.First();

        var audiosource = meta.AudioSourceUrl;
        var audiotype = meta.MimeType;
        await interop.Load(audiosource, audiotype);

        return meta;
    }

    public async Task Play(AudioMetadata meta)
    {
        await interop.Play();
    }
    public async Task Pause()
    {
        await interop.Pause();

    }

    public async Task GoToTimestamp(uint time)
    {
        await interop.GoToTimestamp(time);
    }
    public async Task Next30s()
    {
        var time = await interop.GetTimestamp();
        await interop.GoToTimestamp(time + 30000);
    }

    public async Task Prev30s()
    {
        var time = await interop.GetTimestamp();
        time = time <= 30000 ? 0 : time - 30000;
        await interop.GoToTimestamp(time);
    }


}


public enum PlayerMode
{
    Mini = 0,
    Bottom = 1,

    Full = 2
}
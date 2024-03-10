
using Microsoft.JSInterop;

namespace AudioPlayerBlazor.Services;

internal class MediaPlayerService
{
    private readonly QueuePlayerService queueService;
    private readonly PlayerInterop interop;

    public uint _currentTime = 0;
    public uint CurrentTimeAsMilliseconds => _currentTime;
    public TimeSpan CurrentTime => TimeSpan.FromMilliseconds(_currentTime);

    public Action? OnTimestampUpdated;
    public Action<float>? OnVolumeChange;

    private AudioMetadata? CurrentFile { get; set; } = null;
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
        this.interop.OnVolumeChange += (v) => this.OnVolumeChange?.Invoke(v);
    }

    public void TimeUpdated(float time)
    {
        _currentTime = (uint)(time * 1000);
        OnTimestampUpdated?.Invoke();
    }

    public async Task Init()
    {
        await interop.Init();
    }

    public async Task<AudioMetadata> getCurrentFileInQueue()
    {
        if (CurrentFile != queueService.Queue.First())
        {
            CurrentFile = queueService.Queue.First();
        }
        return CurrentFile;
    }

    public async Task Load()
    {

        var file = await this.getCurrentFileInQueue();
        var audiosource = file.AudioSourceUrl;
        var audiotype = file.AudioFormat.MimeList.FirstOrDefault() ?? string.Empty;
        await interop.Load(audiosource, audiotype);
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
    public async Task Next30s(uint sec)
    {
        var time = await interop.GetTimestamp();
        await interop.GoToTimestamp(time + (sec * 1000));
    }

    public async Task Prev30s(uint sec)
    {
        var skipTime = (sec * 1000);
        var time = await interop.GetTimestamp();
        time = time <= skipTime ? 0 : time - skipTime;
        await interop.GoToTimestamp(time);
    }

    public ChapterInfoDto? CurrentChapter()
    {
        if (CurrentFile == null || CurrentFile?.Chapters == null || CurrentFile?.Chapters?.Count == 0)
            return null;

        var currentChapter = CurrentFile.Chapters
            .FirstOrDefault(c => CurrentTimeAsMilliseconds >= c.StartTime
            && CurrentTimeAsMilliseconds < c.EndTime);

        return currentChapter;
    }

    public async Task  SetWaveRendering(bool shouldRender)
    {
       await interop.SetWaveRendering(shouldRender);
    }

}


public enum PlayerMode
{
    Mini = 0,
    Bottom = 1,

    Full = 2
}
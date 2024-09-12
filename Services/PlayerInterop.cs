using System.Diagnostics.CodeAnalysis;
using Microsoft.JSInterop;

namespace AudioPlayerBlazor.Services;

internal class PlayerInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    public Action<float>? OnTimestampUpdated;
    public Action<float>? OnVolumeChange;

    [DynamicDependency(nameof(UpdateCurrentTimestampFromJs))]
    public PlayerInterop(IJSRuntime jsRuntime)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/AudioPlayerBlazor/AudioPlayerBlazorInterop.js").AsTask());
    }
    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }

    public async ValueTask Init()
    {
        var module = await moduleTask.Value;
        var lDotNetReference = DotNetObjectReference.Create(this);
        await module.InvokeVoidAsync("player.setDotNetRef", lDotNetReference);
    }
    public async ValueTask Play()
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("player.play");
    }

    public async ValueTask Pause()
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("player.pause");
    }

    public async ValueTask Stop()
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("player.stop");
    }
    public async ValueTask Load(string src, string mime, uint position = 0)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("player.load", src, mime, position);
    }

    public async ValueTask SetMediaSession(string title, string artist, string cover)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("player.initMediaSession", title, artist, cover);
    }

    public async ValueTask GoToTimestamp(uint time)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("player.goToTimestamp", time / 1000f);
    }

    public async ValueTask<uint> GetTimestamp()
    {
        var module = await moduleTask.Value;
        return (uint)await module.InvokeAsync<float>("player.getTimestamp") * 1000;
    }

    [JSInvokableAttribute("UpdateCurrentTimestampFromJs")]
    public void UpdateCurrentTimestampFromJs(float time)
    {
        // Console.WriteLine($"Interop got a new time {time}");
        OnTimestampUpdated?.Invoke(time);
    }

    [JSInvokableAttribute("UpdateVolumeFromJs")]
    public void UpdateVolumeFromJs(float volume)
    {
        // Console.WriteLine($"Interop got a new time {time}");
        OnVolumeChange?.Invoke(volume);
    }

    public async Task SetWaveRendering(bool shouldRender)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("player.SetWaveRendering", shouldRender);
    }
}
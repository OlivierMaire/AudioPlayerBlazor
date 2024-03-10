
namespace AudioPlayerBlazor.Services;

internal class SettingService(Blazored.LocalStorage.ILocalStorageService localStorage)
{
    public PlayerMode LastMode { get => Data.LastMode; set { if (Data.LastMode == value) return; Data.LastMode = value; SaveSettings(); } }
    public bool WaveFrame { get => Data.WaveFrame; set { if (Data.WaveFrame == value) return; Data.WaveFrame = value; SaveSettings(); WaveFrameSettingChanged?.Invoke(value); } }
    public uint RewindTime
    {
        get => Data.RewindTime;
        set
        {
            if (Data.RewindTime == value) return;
            if (value > 99) value = 99;
            Data.RewindTime = value;
            SaveSettings();
            RewindTimeSettingChanged?.Invoke(value);
        }
    }
    public uint ForwardTime
    {
        get => Data.ForwardTime; set
        {
            if (Data.ForwardTime == value) return;
            if (value > 99) value = 99;
            Data.ForwardTime = value;
            SaveSettings();
            ForwardTimeSettingChanged?.Invoke(value);
        }
    }

    private SettingStorage Data = new();

    const string StorageKey = "AudioPlayerBlazor.Settings";

    public Action? OnCloseSettings;
    public Action<SettingStorage>? SettingsLoaded;
    public Action<bool>? WaveFrameSettingChanged;
    public Action<uint>? RewindTimeSettingChanged;
    public Action<uint>? ForwardTimeSettingChanged;
    public async Task LoadSettings()
    {
        Data = await localStorage.GetItemAsync<SettingStorage>(StorageKey) ?? new SettingStorage();
        SettingsLoaded?.Invoke(Data);
    }

    private void SaveSettings()
    {
        Task.Run(
        async () =>
        {
            await localStorage.SetItemAsync<SettingStorage>(StorageKey, Data);
        });
    }

    internal class SettingStorage
    {

        public PlayerMode LastMode { get; set; } = PlayerMode.Mini;
        public bool WaveFrame { get; set; } = true;
        public uint RewindTime { get; set; } = 30;
        public uint ForwardTime { get; set; } = 30;

    }

    public void CloseSettings()
    {
        OnCloseSettings?.Invoke();
    }

}

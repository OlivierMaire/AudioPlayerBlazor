namespace AudioPlayerBlazor.Services;

internal class DisplayService
{
  public PlayerMode PlayerMode { get; set; } = PlayerMode.Mini;

    public event Action<PlayerMode>? OnPlayerModeChanged;

    public void SetPlayerMode(PlayerMode value)
    {
        PlayerMode = value;
        OnPlayerModeChanged?.Invoke(PlayerMode);
    }

}
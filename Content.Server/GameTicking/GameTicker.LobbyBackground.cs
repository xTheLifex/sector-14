using Content.Server.GameTicking.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;
using System.Linq;

namespace Content.Server.GameTicking;

public sealed partial class GameTicker
{
    [ViewVariables]
    public string? LobbyBackground { get; private set; }

    [ViewVariables]
    private List<ResPath>? _lobbyBackgrounds;

    private static readonly string[] WhitelistedBackgroundExtensions = new string[] {"png", "jpg", "jpeg", "webp"};
    private static float lobbyImageTimer = 0.0f;
    private void InitializeLobbyBackground()
    {
        _lobbyBackgrounds = _prototypeManager.EnumeratePrototypes<LobbyBackgroundPrototype>()
            .Select(x => x.Background)
            .Where(x => WhitelistedBackgroundExtensions.Contains(x.Extension))
            .ToList();

        RandomizeLobbyBackground();
    }

    public void UpdateLobbyImage(float frameTime)
    {
        base.Update(frameTime);
        if (lobbyImageTimer <= 0.0f)
        {
            lobbyImageTimer = 30.0f + _robustRandom.NextFloat(0.0f, 5.0f);
            RandomizeLobbyBackground();
            SendStatusToAll();
        }

        lobbyImageTimer -= frameTime;
    }

    private void RandomizeLobbyBackground() {
        LobbyBackground = _lobbyBackgrounds!.Any() ? _robustRandom.Pick(_lobbyBackgrounds!).ToString() : null;
    }
}

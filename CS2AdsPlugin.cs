using System.Reflection;
using System.Text.RegularExpressions;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;

namespace CS2Ads;

public class CS2AdsPlugin : BasePlugin, IPluginConfig<CS2AdsConfig>
{
    public override string ModuleName => "CS2Ads";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Lonza";
    public override string ModuleDescription => "Rotates advertisement messages in chat/center at a fixed interval.";

    public CS2AdsConfig Config { get; set; } = new();

    private static readonly Dictionary<string, char> ColorTags = BuildColorTags();

    private static readonly Regex TagRegex = new(@"\{([a-zA-Z0-9_]+)\}", RegexOptions.Compiled);

    private Timer? _timer;
    private int _index = -1;
    private readonly Random _rng = new();

    public void OnConfigParsed(CS2AdsConfig config)
    {
        Config = config;
        _index = -1;
    }

    public override void Load(bool hotReload)
    {
        StartTimer();
        Logger.LogInformation("[CS2Ads] loaded. {Count} messages, every {Interval}s.",
            Config.Messages.Count, Config.IntervalSeconds);
    }

    public override void Unload(bool hotReload)
    {
        _timer?.Kill();
        _timer = null;
    }

    private void StartTimer()
    {
        _timer?.Kill();
        _timer = null;

        if (!Config.Enabled || Config.Messages.Count == 0)
        {
            return;
        }

        _timer = AddTimer(Config.IntervalSeconds, ShowNextAd, TimerFlags.REPEAT);
    }

    private void ShowNextAd()
    {
        if (!Config.Enabled || Config.Messages.Count == 0)
        {
            return;
        }

        var raw = PickNextMessage();
        var formatted = FormatMessage(raw);
        var mode = Config.Mode.Trim().ToLowerInvariant();

        foreach (var player in Utilities.GetPlayers().Where(p => p.IsValid && !p.IsBot))
        {
            if (mode is "chat" or "both")
            {
                player.PrintToChat(formatted);
            }

            if (mode is "center" or "both")
            {
                player.PrintToCenter(StripColorTags(formatted));
            }
        }
    }

    private string PickNextMessage()
    {
        if (Config.RandomOrder)
        {
            return Config.Messages[_rng.Next(Config.Messages.Count)];
        }

        _index = (_index + 1) % Config.Messages.Count;
        return Config.Messages[_index];
    }

    private string FormatMessage(string raw)
    {
        var withPlaceholders = ReplacePlaceholders(raw);
        return TagRegex.Replace(withPlaceholders, match =>
        {
            var tag = match.Groups[1].Value;
            return ColorTags.TryGetValue(tag, out var c) ? c.ToString() : match.Value;
        });
    }

    private string ReplacePlaceholders(string raw)
    {
        var players = Utilities.GetPlayers().Count(p => p.IsValid && !p.IsBot);

        return raw
            .Replace("{prefix}", Config.Prefix, StringComparison.OrdinalIgnoreCase)
            .Replace("{map}", Server.MapName, StringComparison.OrdinalIgnoreCase)
            .Replace("{players}", players.ToString(), StringComparison.OrdinalIgnoreCase)
            .Replace("{maxplayers}", Server.MaxPlayers.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    private string StripColorTags(string formatted)
    {
        // PrintToCenter doesn't support chat color chars; drop them.
        return string.Concat(formatted.Where(c => !ColorTags.ContainsValue(c)));
    }

    private static Dictionary<string, char> BuildColorTags()
    {
        var map = new Dictionary<string, char>(StringComparer.OrdinalIgnoreCase);
        var fields = typeof(ChatColors).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(char) && f.GetCustomAttribute<ObsoleteAttribute>() == null);

        foreach (var field in fields)
        {
            map[field.Name] = (char)field.GetValue(null)!;
        }

        return map;
    }

    [ConsoleCommand("css_ads_skip", "Immediately show the next advertisement")]
    [RequiresPermissions("@css/root")]
    public void OnAdsSkip(CCSPlayerController? player, CommandInfo info)
    {
        ShowNextAd();
        info.ReplyToCommand("[CS2Ads] Skipped to next ad.");
    }

    [ConsoleCommand("css_ads_toggle", "Enable/disable the advertisement rotation")]
    [RequiresPermissions("@css/root")]
    public void OnAdsToggle(CCSPlayerController? player, CommandInfo info)
    {
        Config.Enabled = !Config.Enabled;
        StartTimer();
        info.ReplyToCommand($"[CS2Ads] Rotation {(Config.Enabled ? "enabled" : "disabled")}.");
    }
}

using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace CS2Ads;

public class CS2AdsConfig : BasePluginConfig
{
    [JsonPropertyName("Enabled")]
    public bool Enabled { get; set; } = true;

    [JsonPropertyName("IntervalSeconds")]
    public float IntervalSeconds { get; set; } = 30.0f;

    [JsonPropertyName("RandomOrder")]
    public bool RandomOrder { get; set; } = false;

    /// <summary>Accepted values: "chat", "center", "both" (case-insensitive).</summary>
    [JsonPropertyName("Mode")]
    public string Mode { get; set; } = "chat";

    [JsonPropertyName("Prefix")]
    public string Prefix { get; set; } = "{green}[SERVER]{default}";

    [JsonPropertyName("Messages")]
    public List<string> Messages { get; set; } = new()
    {
        "{prefix} Bienvenido al servidor, escribe {gold}!discord{default} para unirte a la comunidad.",
        "{prefix} Mapa actual: {gold}{map}{default} - Jugadores: {gold}{players}/{maxplayers}{default}",
        "{prefix} Sigue las {gold}reglas del servidor{default} para evitar sanciones.",
        "{prefix} Visita nuestra web: {lightblue}www.example.com{default}"
    };
}

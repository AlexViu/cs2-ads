# CS2Ads

Plugin de anuncios rotativos para CS2 usando [CounterStrikeSharp](https://docs.cssharp.dev/).

Cada `IntervalSeconds` segundos (30 por defecto) el plugin muestra **un** anuncio de la lista
por chat y/o pantalla central, avanzando al siguiente en cada tick (o en orden aleatorio si se
activa `RandomOrder`).

## Requisitos

- Servidor CS2 con [Metamod:Source](https://www.sourcemm.net/) y [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp/releases) instalados.
- .NET 10 SDK para compilar (la versión actual del paquete `CounterStrikeSharp.API` en NuGet exige `net10.0`; si tu build de CounterStrikeSharp es más antigua y usa `net8.0`, fija `<TargetFramework>net8.0</TargetFramework>` en `CS2Ads.csproj` y `<PackageReference Include="CounterStrikeSharp.API" Version="1.0.3xx" />` con una versión acorde).

## Compilar

```bash
dotnet build -c Release
```

El `.dll` resultante queda en `bin/Release/net10.0/CS2Ads.dll`.

## Instalar

1. Copia `CS2Ads.dll` (y las demás dependencias generadas en esa carpeta) a:
   `game/csgo/addons/counterstrikesharp/plugins/CS2Ads/`
2. Inicia o reinicia el mapa. La primera vez se generará automáticamente el archivo de
   configuración en:
   `game/csgo/addons/counterstrikesharp/configs/plugins/CS2Ads/CS2Ads.json`
3. Edita ese JSON a tu gusto (ver ejemplo abajo) y recarga con `css_plugins reload CS2Ads`.

## Configuración (`CS2Ads.json`)

```json
{
  "Enabled": true,
  "IntervalSeconds": 30.0,
  "RandomOrder": false,
  "Mode": "chat",
  "Prefix": "{green}[SERVER]{default}",
  "Messages": [
    "{prefix} Bienvenido al servidor, escribe {gold}!discord{default} para unirte a la comunidad.",
    "{prefix} Mapa actual: {gold}{map}{default} - Jugadores: {gold}{players}/{maxplayers}{default}",
    "{prefix} Sigue las {gold}reglas del servidor{default} para evitar sanciones.",
    "{prefix} Visita nuestra web: {lightblue}www.example.com{default}"
  ],
  "ConfigVersion": 1
}
```

- `Mode`: `chat`, `center` o `both`.
- Placeholders disponibles en cada mensaje: `{prefix}`, `{map}`, `{players}`, `{maxplayers}`.
- Tags de color: cualquier nombre de `ChatColors` (p. ej. `{default}`, `{red}`, `{green}`,
  `{gold}`, `{lightblue}`, `{purple}`, `{yellow}`, `{orange}`, `{lime}`, `{grey}`...). Solo
  aplican al modo chat; en `center` se eliminan automáticamente.

## Comandos (requieren `@css/root`)

- `css_ads_skip`: fuerza el siguiente anuncio de inmediato.
- `css_ads_toggle`: activa/desactiva la rotación sin recargar el plugin.

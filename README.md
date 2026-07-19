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
2. Copia `configs/CS2Ads.json` (incluido en este repo) a:
   `game/csgo/addons/counterstrikesharp/configs/plugins/CS2Ads/CS2Ads.json`

   Si no lo copias, no pasa nada: CounterStrikeSharp genera ese mismo archivo
   automáticamente la primera vez que carga el plugin, usando los valores por
   defecto definidos en `CS2AdsConfig.cs`.
3. Inicia o reinicia el mapa.

## Añadir / editar mensajes

Los anuncios **no** están en el código — viven en el JSON de configuración del servidor
(`CS2Ads.json`), así que puedes cambiarlos en cualquier momento sin recompilar el plugin:

1. Abre `game/csgo/addons/counterstrikesharp/configs/plugins/CS2Ads/CS2Ads.json` en el servidor.
2. Añade, edita o borra strings dentro del array `"Messages"`.
3. Guarda y, desde la consola del servidor (o RCON), ejecuta `css_plugins reload CS2Ads`
   (o cambia de mapa) para que recargue la config sin reiniciar el servidor entero.

Si además quieres llevar control de versiones de tus anuncios, edita también
`configs/CS2Ads.json` en este repo y haz commit — es una copia idéntica a la que
usa el servidor, pensada para servir como plantilla/backup versionado.

## Configuración (`CS2Ads.json`)

```json
{
  "Enabled": true,
  "IntervalSeconds": 30.0,
  "RandomOrder": false,
  "Mode": "chat",
  "Prefix": "{default}[SERVER]{default}",
  "Messages": [
    "{prefix} Bienvenido al servidor, escribe !discord para unirte a la comunidad.",
    "{prefix} Mapa actual: {map} - Jugadores: {players}/{maxplayers}",
    "{prefix} Sigue las reglas del servidor para evitar sanciones.",
    "{prefix} Visita nuestra web: www.example.com"
  ],
  "ConfigVersion": 1
}
```

- `Mode`: `chat`, `center` o `both`.
- Placeholders disponibles en cada mensaje: `{prefix}`, `{map}`, `{players}`, `{maxplayers}`.
- Tags de color: cualquier nombre de `ChatColors` (p. ej. `{default}`, `{red}`, `{green}`,
  `{gold}`, `{lightblue}`, `{purple}`, `{yellow}`, `{orange}`, `{lime}`, `{grey}`...). Solo
  aplican al modo chat; en `center` se eliminan automáticamente.

### ⚠️ Límite de un color por línea (limitación del motor de CS2, no del plugin)

El chat de CS2 solo permite renderizar **un color "especial" distinto por línea**
(cualquiera que no sea `{default}`). Si combinas dos colores distintos como `{green}`
y `{gold}` en el mismo mensaje (por ejemplo uno en el `Prefix` y otro en el cuerpo),
solo se aplicará el último — el resto se muestra en blanco/color por defecto.

- `{default}` siempre es seguro de combinar con cualquier otro color (no cuenta para el límite).
- Repetir el **mismo** color varias veces en una línea funciona bien (no es "otro" color).
- Si quieres el prefijo de un color y el cuerpo del mensaje de otro, tendrás que elegir uno
  de los dos, o dejar el prefijo en `{default}` (como viene configurado por defecto).

**Por eso los mensajes por defecto ya no llevan color en el cuerpo** — solo el `{prefix}`
lleva color especial. Así, si cambias `Prefix` a por ejemplo
`"{lightblue}[Random Picks]{default}"`, se aplicará correctamente en todos los mensajes,
porque no compite con ningún otro color en la misma línea. Si quieres además resaltar
una palabra dentro del cuerpo (p. ej. `!discord` en dorado), usa el **mismo** color que
el `Prefix` (en este ejemplo `{lightblue}`, no `{gold}`), o el prefijo perderá su color.

## Comandos (requieren `@css/root`)

- `css_ads_skip`: fuerza el siguiente anuncio de inmediato.
- `css_ads_toggle`: activa/desactiva la rotación sin recargar el plugin.

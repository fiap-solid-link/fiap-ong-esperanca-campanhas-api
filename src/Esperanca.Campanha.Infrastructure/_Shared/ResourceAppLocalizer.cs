using System.Text.Json;
using Esperanca.Campanha.Application;
using Esperanca.Campanha.Application._Shared.Localization;

namespace Esperanca.Campanha.Infrastructure._Shared;

internal sealed class ResourceAppLocalizer : IAppLocalizer
{
    private readonly IReadOnlyDictionary<string, string> _messages;

    public ResourceAppLocalizer()
    {
        _messages = LoadMessages();
    }

    private static IReadOnlyDictionary<string, string> LoadMessages()
    {
        var assembly = typeof(CampanhaApplicationModule).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("pt-BR.json"));

        if (resourceName is null)
            return new Dictionary<string, string>();

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var doc = JsonDocument.Parse(stream);

        return doc.RootElement
            .GetProperty("texts")
            .EnumerateObject()
            .ToDictionary(p => p.Name, p => p.Value.GetString() ?? p.Name);
    }

    public string this[string code] =>
        _messages.TryGetValue(code, out var msg) ? msg : code;
}

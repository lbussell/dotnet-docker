using System.Text.Json;

namespace update_dependencies_2.Model;

internal record ReleaseConfig(
    string Channel,
    string MajorVersion,
    string Release,
    string Runtime,
    string Asp,
    List<string> Sdks,
    string Runtime_Build,
    string Asp_Build,
    List<string> Sdk_Builds,
    string Release_Date,
    bool Security,
    string Support_Phase,
    bool Internal,
    bool SdkOnly)
{
    public override string ToString()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }

    public static ReleaseConfig FromJson(string json)
    {
        return JsonSerializer.Deserialize<ReleaseConfig>(json)
            ?? throw new InvalidOperationException("Failed to deserialize release config: " + json);
    }
};

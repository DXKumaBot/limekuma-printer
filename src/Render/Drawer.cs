using Limekuma.Prober.Common;
using Limekuma.Render.ExpressionEngine;
using Limekuma.Render.Nodes;
using SixLabors.ImageSharp;
using System.Collections;
using System.Collections.Immutable;
using System.Text;

namespace Limekuma.Render;

public sealed class Drawer
{
    private static readonly AsyncNCalcEngine ExpressionEngine = CreateExpressionEngine();

    public async Task<Image> DrawBestsAsync(User user, IReadOnlyList<Record> ever,
        IReadOnlyList<Record> current, int everTotal, int currentTotal, string? condition,
        IEnumerable<string> tags) => await DrawBestsAsync(user, ever, current, everTotal, currentTotal, condition,
        tags, null, "./Resources/Layouts/bests.xml");

    public async Task<Image> DrawBestsAsync(User user, IReadOnlyList<Record> ever,
        IReadOnlyList<Record> current, int everTotal, int currentTotal, string? condition,
        IEnumerable<string> tags, User? player2) => await DrawBestsAsync(user, ever, current, everTotal,
        currentTotal, condition, tags, player2, "./Resources/Layouts/bests.xml");

    public async Task<Image> DrawBestsAsync(User player1, IReadOnlyList<Record> ever,
        IReadOnlyList<Record> current, int everTotal, int currentTotal, string? condition,
        IEnumerable<string> tags, User? player2, string xmlPath)
    {
        int everMax = ever.Count > 0 ? ever[0].DXRating : 0;
        int everMin = ever.Count > 0 ? ever[^1].DXRating : 0;
        int currentMax = current.Count > 0 ? current[0].DXRating : 0;
        int currentMin = current.Count > 0 ? current[^1].DXRating : 0;
        Version? version = typeof(Drawer).Assembly.GetName().Version;
        StringBuilder sb = new();
        for (int i = 0; i < version?.Build / 26; ++i)
        {
            sb.Append('Z');
        }

        sb.Append(Convert.ToChar('A' + (version?.Build % 26)));
        Dictionary<string, object?> scope = new(StringComparer.OrdinalIgnoreCase)
        {
            ["userInfo"] = player1,
            ["everRecords"] = ever,
            ["currentRecords"] = current,
            ["everRating"] = everTotal,
            ["currentRating"] = currentTotal,
            ["condition"] = condition,
            ["tags"] = tags,
            ["2pUserInfo"] = player2,
            ["everMax"] = everMax,
            ["everMin"] = everMin,
            ["currentMax"] = currentMax,
            ["currentMin"] = currentMin,
            ["now"] = DateTimeOffset.Now.ToString("yyy/M/d H:mmz"),
            ["version"] = $"Ver.LI{version?.Major ?? 0}.{version?.Minor ?? 0}-{sb}"
        };
        return await DrawAsync(scope, xmlPath);
    }

    public async Task<Image> DrawListAsync(User user, IReadOnlyList<Record> records, int page,
        ImmutableArray<int> counts, int totalCount, int startIndex, string condition,
        IEnumerable<string> tags) => await DrawListAsync(user, records, page, counts, totalCount, startIndex, condition,
        tags, "./Resources/Layouts/list.xml");

    public async Task<Image> DrawListAsync(User user, IReadOnlyList<Record> records, int page,
        ImmutableArray<int> counts, int totalCount, int startIndex, string condition,
        IEnumerable<string> tags, string xmlPath)
    {
        int totalPages = (int)Math.Ceiling(totalCount / 55m);
        Version? version = typeof(Drawer).Assembly.GetName().Version;
        StringBuilder sb = new();
        for (int i = 0; i < version?.Build / 26; ++i)
        {
            sb.Append('Z');
        }

        sb.Append(Convert.ToChar('A' + (version?.Build % 26)));
        Dictionary<string, object?> scope = new(StringComparer.OrdinalIgnoreCase)
        {
            ["userInfo"] = user,
            ["pageRecords"] = records,
            ["pageNumber"] = page,
            ["totalPages"] = totalPages,
            ["rankCounts"] = counts[..7],
            ["comboCounts"] = counts[7..],
            ["totalCount"] = totalCount,
            ["startIndex"] = startIndex,
            ["condition"] = condition,
            ["tags"] = tags,
            ["now"] = DateTimeOffset.Now.ToString("yyy/M/d H:mmz"),
            ["version"] = $"Ver.LI{version?.Major ?? 0}.{version?.Minor ?? 0}-{sb}"
        };
        return await DrawAsync(scope, xmlPath);
    }

    private static async Task<Image> DrawAsync(IDictionary<string, object?> scope, string xmlPath)
    {
        TemplateReader loader = new(ExpressionEngine);
        Node tree = await loader.LoadAsync(xmlPath, scope);
        AssetProvider assets = AssetProvider.Shared;
        return NodeRenderer.Render((CanvasNode)tree, assets, assets);
    }

    private static AsyncNCalcEngine CreateExpressionEngine()
    {
        AsyncNCalcEngine expr = new();
        expr.RegisterFunction("ToString", (object x) => Convert.ToString(x));
        expr.RegisterFunction("Count", (IList x) => x.Count);
        return expr;
    }
}

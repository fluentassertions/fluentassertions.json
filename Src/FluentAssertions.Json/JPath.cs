namespace FluentAssertions.Json;

internal sealed class JPath
{
    private readonly string[] nodes;

    public JPath()
    {
        nodes = ["$"];
    }

    private JPath(JPath existingPath, string extraNode)
    {
        nodes = [.. existingPath.nodes, extraNode];
    }

    public JPath AddProperty(string name)
    {
        return new JPath(this, $".{name}");
    }

    public JPath AddIndex(int index)
    {
        return new JPath(this, $"[{index}]");
    }

    public override string ToString()
    {
        return string.Concat(nodes);
    }
}

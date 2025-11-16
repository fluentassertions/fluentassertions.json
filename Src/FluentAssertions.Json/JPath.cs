using System.Collections.Generic;

namespace FluentAssertions.Json
{
    internal sealed class JPath
    {
        private readonly List<string> nodes = new();

        public JPath()
        {
            nodes.Add("$");
        }

        private JPath(JPath existingPath, string extraNode)
        {
            nodes.AddRange(existingPath.nodes);
            nodes.Add(extraNode);
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
}

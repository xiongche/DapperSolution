using System.Collections.Generic;

namespace System.Xml
{
    public sealed class XmlHelper
    {
        public static XmlNode FindNodeByName(string fileName, string nodeName)
        {
            var doc = GetDocument(fileName);

            return FindNodeByName(doc, nodeName);
        }

        public static XmlNode FindNodeByName(XmlDocument doc, string nodeName)
        {
            var pureNodeName = StringHelper.ToPureUpper(nodeName);

            foreach (XmlNode node in doc.ChildNodes)
            {
                if (!IsValidNode(node))
                {
                    continue;
                }

                var result = FindNodeByName(node, pureNodeName);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public static XmlNode FindNodeByPath(string fileName, string xpath)
        {
            var doc = GetDocument(fileName);
            var node = doc.SelectSingleNode(xpath);

            return IsValidNode(node) ? node : null;
        }

        public static IList<XmlNode> FindNodesByName(string fileName, string nodeName)
        {
            var pureNodeName = StringHelper.ToPureUpper(nodeName);
            var nodes = new List<XmlNode>();

            var doc = GetDocument(fileName);
            foreach (XmlNode node in doc.ChildNodes)
            {
                if (!IsValidNode(node))
                {
                    continue;
                }

                var result = FindNodeByName(node, pureNodeName);
                if (result != null)
                {
                    nodes.Add(result);
                }
            }

            return nodes;
        }

        public static XmlNodeList FindNodesByPath(string fileName, string xpath)
        {
            var doc = GetDocument(fileName);

            return doc.SelectNodes(xpath);
        }

        public static XmlDocument GetDocument(string fileName)
        {
            var doc = new XmlDocument();
            doc.Load(fileName);

            return doc;
        }

        private static bool IsValidNode(XmlNode node)
        {
            return node == null ? false : node.NodeType == XmlNodeType.Element;
        }

        private static XmlNode FindNodeByName(XmlNode node, string nodeName)
        {
            if (StringHelper.ToPureUpper(node.Name) == nodeName)
            {
                return node;
            }

            foreach (XmlNode child in node.ChildNodes)
            {
                if (!IsValidNode(child))
                {
                    continue;
                }

                var result = FindNodeByName(child, nodeName);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}

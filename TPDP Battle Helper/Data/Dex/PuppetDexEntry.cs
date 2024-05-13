
using System.Xml;

namespace TPDP_Battle_Helper.Data.Dex
{
    internal class PuppetDexEntry
    {

        public short InternalId { get; }
        public string PuppetName { get; }
        public short DexId { get; }
        public short Cost { get; }

        public readonly Dictionary<byte, StyleDexEntry> StyleList = [];

        public PuppetDexEntry(XmlNode baseNode)
        {
            foreach (XmlNode node in baseNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "id":
                        InternalId = Convert.ToInt16(node.InnerText, 16);
                        break;
                    case "name":
                        PuppetName = node.InnerText;
                        break;
                    case "dexId":
                        DexId = short.Parse(node.InnerText);
                        break;
                    case "cost":
                        Cost = short.Parse(node.InnerText);
                        break;
                    case "styles":
                        foreach (XmlNode styleNode in node.ChildNodes)
                        {
                            if (node.NodeType == XmlNodeType.Element)
                            {
                                byte styleId = byte.Parse(styleNode.Attributes["id"].Value);
                                StyleDexEntry entry = new StyleDexEntry(styleNode);
                                StyleList.Add(styleId, entry);
                            }
                        }
                        break;
                }
            }
        }

    }
}

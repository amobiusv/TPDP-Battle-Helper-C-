
using System.Xml;

namespace TPDP_Battle_Helper.Data.Dex
{
    internal class PuppetDex
    {

        public static readonly Dictionary<ushort, PuppetDexEntry> PuppetList = [];

        public static void Init()
        {

            XmlDocument doc = new XmlDocument();
            doc.Load(".\\Config\\Puppets.xml");

            XmlNode baseNode = doc.DocumentElement;

            foreach (XmlNode node in baseNode.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    ushort puppetId = (ushort) Convert.ToInt16(node.Attributes["id"].Value, 16);
                    PuppetDexEntry entry = new PuppetDexEntry(node);
                    PuppetList.Add(puppetId, entry);
                }
            }

        }

    }
}

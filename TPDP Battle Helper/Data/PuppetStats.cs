using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TPDP_Battle_Helper.Data
{

    internal class PuppetStats
    {

        public int HP { get; set; }
        public int FAtk { get; set; }
        public int FDef { get; set; }
        public int SAtk { get; set; }
        public int SDef { get; set; }
        public int Spd { get; set; }

        public PuppetStats(XmlNode baseNode)
        {
            foreach (XmlNode node in baseNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "HP":
                        HP = int.Parse(node.InnerText);
                        break;
                    case "FAtk":
                        FAtk = int.Parse(node.InnerText);
                        break;
                    case "FDef":
                        FDef = int.Parse(node.InnerText);
                        break;
                    case "SAtk":
                        SAtk = int.Parse(node.InnerText);
                        break;
                    case "SDef":
                        SDef = int.Parse(node.InnerText);
                        break;
                    case "Spd":
                        Spd = int.Parse(node.InnerText);
                        break;
                }
            }
        }
    }
}

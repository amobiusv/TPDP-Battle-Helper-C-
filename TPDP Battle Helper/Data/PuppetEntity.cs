using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPDP_Battle_Helper.Data.Dex;

namespace TPDP_Battle_Helper.Data
{
    internal class PuppetEntity
    {

        public PuppetDexEntry PuppetSpecies { get; }
        public StyleDexEntry PuppetStyle { get; }

        private PuppetEntity(PuppetDexEntry puppet, StyleDexEntry style)
        {
            PuppetSpecies = puppet;
            PuppetStyle = style;
        }

        public static PuppetEntity? FindPuppet(ushort puppetId, byte styleIdx)
        {
            if (!PuppetDex.PuppetList.ContainsKey(puppetId)) return null;
            PuppetDexEntry puppet = PuppetDex.PuppetList[puppetId];
            if (!puppet.StyleList.ContainsKey(styleIdx)) return null;
            StyleDexEntry style = puppet.StyleList[styleIdx];
            return new PuppetEntity(puppet, style);
        }

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ThunderRoad;
using UnityEngine;

namespace SpellUndying
{
    public class LoadModule : LevelModule
    {

        public string mod_version = "0.0";
        public string mod_name = "UnnamedMod";
        public string logger_level = "Basic";

        public bool dieOnHeadChop = false;

        public override IEnumerator OnLoadCoroutine()
        {
            Logger.init(mod_name, mod_version, logger_level);
            Logger.Basic("Loading " + mod_name);

            (Catalog.GetData(Catalog.Category.Spell, "Undying") as SpellUndying).dieOnHeadChop = dieOnHeadChop;

            return base.OnLoadCoroutine();
        }
    }
}

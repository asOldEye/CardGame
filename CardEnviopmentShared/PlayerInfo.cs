using System;
using System.Collections.Generic;
using AuxiliaryLibrary;
using CardSessionShared;
using Chat;

namespace CardEnvironmentShared
{
    [Serializable]
    public class PlayerInfo : IChatOwnerInfo, IControllerInfo
    {
        public string Name { get; set; }

        public List<Pair<InterpretedObject, Info>> Soliders { get; set; }
        public List<Pair<InterpretedObject, Info>> Spells { get; set; }
        public List<Pair<InterpretedObject, Info>> CurrentSoliders { get; set; }
        public List<Pair<InterpretedObject, Info>> CurrentSpells { get; set; }
    }
}
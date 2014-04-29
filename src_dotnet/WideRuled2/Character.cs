using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    [Serializable()]
    public class Character : StoryEntity
    {

        private static Character _noOneChar = new Character(
                StoryData.NullCharacterName,
                StoryData.NullCharacterId,
                StoryData.NullCharacterTypeId,
                null);

        public static Character getNoOneCharacter() {
            return _noOneChar;
            
        }

        public Character(string name, UInt64 charId, UInt64 charTypeId, StoryData world) :
            base(name, charId, charTypeId, world)
        {
            if (world != null) findTraitByName("Name").TypeId = world.CharNameTraitTypeId;
        }
        public Character(string name, UInt64 charTypeId, StoryData world):
             base(name, charTypeId, world)
        {
            if (world != null) findTraitByName("Name").TypeId = world.CharNameTraitTypeId;
        }


    }
}

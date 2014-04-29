using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    [Serializable()]
    public class Environment : StoryEntity
    {

        private static Environment _noWhereEnv = new Environment(
                StoryData.NullEnvironmentName, 
                StoryData.NullEnvironmentId, 
                StoryData.NullEnvironmentTypeId, 
                null);

        public static Environment getNoWhereEnvironment() {

            return _noWhereEnv;
        }

        public Environment(string name, UInt64 charId, UInt64 envTypeId, StoryData world) :
            base(name, charId, envTypeId, world)
        {
            if (world != null) findTraitByName("Name").TypeId = world.EnvNameTraitTypeId;
        }

        public Environment(string name, UInt64 envTypeId, StoryData world):
            base(name, envTypeId, world)
        {
            if (world != null) findTraitByName("Name").TypeId = world.EnvNameTraitTypeId;
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    // Utility class that allows for XAML elements to bind to dynamic story world
    // data easier.
    public class StoryWorldDataProvider
    {
        private static StoryData _storyWorld;
        

        public static void setStoryData(StoryData world)
        {
            _storyWorld = world;
        }

        public static StoryData getStoryData()
        {
            return _storyWorld;
        }

        public static List<String> getTraitDataTypes()
        {
            List<String> dataTypeStrings = new List<String>();

            dataTypeStrings.Add(Trait.TextString);
            dataTypeStrings.Add(Trait.NumberString);
            dataTypeStrings.Add(Trait.TrueFalseString);

            return dataTypeStrings;

        }

        public static AuthorGoal findAuthorGoalById(UInt64 id)
        {
            return _storyWorld.findAuthorGoalById(id);

        }

        public Character findCharacterById(UInt64 id)
        {
            return _storyWorld.findCharacterById(id);

        }

        public Environment findEnvironmentById(UInt64 id)
        {
            return _storyWorld.findEnvironmentById(id);

        }

        public PlotFragment findPlotFragmentById(UInt64 id)
        {
            return _storyWorld.findPlotFragmentById(id);

        }

        public static List<Character> getCharactersWithNoOne()
        {


            if (_storyWorld == null) 
            {
                return new List<Character>();
            }
            else 
            {
                Character NoOne = Character.getNoOneCharacter();
                List<Character> charList = _storyWorld.Characters;
                List<Character> fullCharList = new List<Character>();

                fullCharList.AddRange(charList);
                fullCharList.Add(NoOne);

                return fullCharList;
            }

        }

        public static List<Environment> getEnvironmentsWithNoWhere()
        {


            if (_storyWorld == null)
            {
                return new List<Environment>();
            }
            else
            {
                Environment NoWhere = Environment.getNoWhereEnvironment();
                List<Environment> envList = _storyWorld.Environments;
                List<Environment> fullEnvList = new List<Environment>();

                fullEnvList.AddRange(envList);
                fullEnvList.Add(NoWhere);

                return fullEnvList;
            }

        }
    }
}

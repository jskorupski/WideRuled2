using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    [Serializable()]
    public class StoryData
    {
        public const string NullCharacterName = "No One";
        public const string NullEnvironmentName = "Nowhere";
        public const UInt64 NullCharacterId = 0;
        public const UInt64 NullCharacterTypeId = 0;
        public const UInt64 NullEnvironmentId = 0;
        public const UInt64 NullEnvironmentTypeId = 0;


        private UInt64 _charTypeId;
        private UInt64 _envTypeId;
        private UInt64 _defaultCharNameTraitTypeId;
        private UInt64 _defaultEnvNameTraitTypeId;

        private UInt64 _relTypeId;

        private bool _showDebugMessages;

        private UInt64 _idCount;

        private List<Character> _characters;
        private List<Environment> _environments;
        private List<PlotPointType> _ppTypes;
        private List<AuthorGoal> _authorGoals;
        private List<Interaction> _interactions;

        private UInt64 _startGoalId;

        public StoryData()
        {

            _idCount = 0;
            _startGoalId = 0;
            _showDebugMessages = false;
            _characters = new List<Character>();
            _environments = new List<Environment>();
            _ppTypes = new List<PlotPointType>();
            _authorGoals = new List<AuthorGoal>();
            _interactions = new List<Interaction>();

            _charTypeId = getNewId();
            _envTypeId = getNewId();
            _defaultCharNameTraitTypeId = getNewId();
            _defaultEnvNameTraitTypeId = getNewId();
            _relTypeId = getNewId();
        }

        public UInt64 getNewId() 
        {
            _idCount++;
            return _idCount;
        }

        public bool ShowDebugMessages
        {
            get { return _showDebugMessages; }
            set { _showDebugMessages = value; }
        }
        public UInt64 CharTypeId
        {
            get { return _charTypeId; }
            set { _charTypeId = value; }
        }
        public UInt64 EnvTypeId
        {
            get { return _envTypeId; }
            set { _envTypeId = value; }
        }
        public UInt64 CharNameTraitTypeId
        {
            get 
            { 
                if(_characters.Count == 0)
                {
                    return _defaultCharNameTraitTypeId; 
                }
                else
                {
                    return _characters[0].findTraitByName("Name").TypeId;
                }
                
            }
        }

        public UInt64 EnvNameTraitTypeId
        {
            get 
            {
                if (_environments.Count == 0)
                {
                    return _defaultEnvNameTraitTypeId; 
                }
                else 
                {
                    return _environments[0].findTraitByName("Name").TypeId;

                }
                
                
            }
        }
        public UInt64 RelTypeId
        {
            get { return _relTypeId; }
            set { _relTypeId = value; }
        }
        public UInt64 StartGoalId
        {
            get { return _startGoalId; }
            set { _startGoalId = value; }
        }
        public List<Character> Characters
        {
            get { return _characters; }
            set { _characters = value; }
        }


        public List<Environment> Environments
        {
            get { return _environments; }
            set { _environments = value; }
        }


        public List<PlotPointType> PlotPointTypes
        {
            get { return _ppTypes; }
            set { _ppTypes = value; }
        }


        public List<AuthorGoal> AuthorGoals
        {
            get { return _authorGoals; }
            set { _authorGoals = value; }
        }


        public List<Interaction> Interactions
        {
            get { return _interactions; }
            set { _interactions = value; }
        }


        public Character findCharacterByName(string name)
        {
            
            foreach (Character item in _characters)
            {
                if ((string)item.Name == name)
                {
                    return item;
                }
            }
            return null;
        }

        public Environment findEnvironmentByName(string name)
        {

            foreach (Environment item in _environments)
            {
                if ((string)item.Name == name)
                {
                    return item;
                }
            }
            return null;
        }

        public Character findCharacterById(UInt64 id)
        {
            if (id == NullCharacterId) { return Character.getNoOneCharacter(); }
            foreach (Character item in _characters)
            {
                if (item.Id == id)
                {
                    return item;
                }
            }
            return null;
        }
        public Environment findEnvironmentById(UInt64 id)
        {
            if (id == NullEnvironmentId) { return Environment.getNoWhereEnvironment(); }
            foreach (Environment item in _environments)
            {
                if (item.Id == id)
                {
                    return item;
                }
            }
            return null;
        }
        public PlotPointType findPlotPointTypeById(UInt64 id)
        {

            foreach (PlotPointType item in _ppTypes)
            {
                if (item.Id == id)
                {
                    return item;
                }
            }
            return null;
        }

        public PlotFragment findPlotFragmentById(UInt64 id)
        {

            foreach (AuthorGoal agItem in _authorGoals)
            {
                foreach (PlotFragment fragItem in agItem.PlotFragments)
                {
                    if (fragItem.Id == id)
                    {
                        return fragItem;
                    }
                }

            }
            return null;
        }

        public AuthorGoal findAuthorGoalById(UInt64 id)
        {

            foreach (AuthorGoal item in _authorGoals)
            {
                if (item.Id == id)
                {
                    return item;
                }
            }
            return null;
        }

    

        

    }
}

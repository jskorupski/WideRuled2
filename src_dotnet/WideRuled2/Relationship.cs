using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    [Serializable()]
    public class Relationship : ICloneable
    {

        private UInt64 _relId;
        private UInt64 _relTypeId;

        private string _name;

        private UInt64 _fromId;
        private UInt64 _toId;
        private double _strength;


        public Relationship(string name, UInt64 fromId, UInt64 toId, double strength, UInt64 relTypeId, StoryData world) 
        {
            Constructor(name, fromId, toId, strength, relTypeId, world);

        }

        public Relationship (string name, UInt64 fromId, UInt64 relTypeId, StoryData world)
        {
            Constructor(name, fromId, 0, 0.0, relTypeId, world);
             
        }



        // Copy trait and keep trait type id to identify it as the same relationship elsewhere in the program
        public Relationship(Relationship anotherRel, StoryEntity originatingEntity, StoryData world)
        {
            UInt64 nullTargetId = 0;
            if (originatingEntity.GetType() == typeof(Character))
            {
                nullTargetId = Character.getNoOneCharacter().Id;
            }
            else if (originatingEntity.GetType() == typeof(Environment))
            {

                nullTargetId = Environment.getNoWhereEnvironment().Id;
            }

            Constructor(anotherRel.Name, originatingEntity.Id, nullTargetId, 0.0, anotherRel.TypeId, world);
        }

        private void Constructor(string name, UInt64 fromId, UInt64 toId, double strength, UInt64 relTypeId, StoryData world)
        {
            if(world == null)
            {
                _relId = 0;
            }
            else
            {
                _relId = world.getNewId();
            }
         
            _relTypeId = relTypeId;


            _name = name;
            _strength = strength;
            _fromId = fromId;
            _toId = toId;

        }
        
        public UInt64 Id
        {
            get { return _relId; }
            set { _relId = value; }
        }

        public UInt64 TypeId
        {
            get { return _relTypeId; }
            set { _relTypeId = value; }
        }
            
        public string Name 
        {
            get { return _name; }
            set { _name = value; }
        }

        public Character ToCharacter 
        {
            get 
            {
                return StoryWorldDataProvider.getStoryData().findCharacterById(_toId);
            }
            set 
            {

                _toId = value.Id;
            }
        }

        public Environment ToEnvironment
        {
            get
            {
                return StoryWorldDataProvider.getStoryData().findEnvironmentById(_toId);
            }
            set
            {

                _toId = value.Id;
            }
        }

        public UInt64 FromId
        {
            get { return _fromId; }
            set { _fromId = value; }
        }

        public UInt64 ToId
        {
            get { return _toId; }
            set { _toId = value; }
        }

        public double Strength
        {
            get { return _strength; }
            set { _strength = value; }
        }


      
        #region ICloneable Members

        virtual public Object Clone()
        {
            Relationship newClone = (Relationship)MemberwiseClone();
            newClone.Id = StoryWorldDataProvider.getStoryData().getNewId();
   
            return newClone;
        }

        #endregion

 

 
    }
}

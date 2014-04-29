using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;


namespace WideRuled2
{
    [Serializable()]
    public class StoryEntity : ICloneable
    {
        protected UInt64 _entityId;
        protected UInt64 _entityTypeId;

        protected List<Trait> _traits;
        protected List<Relationship> _relationships;


        public StoryEntity(string name, UInt64 entityId, UInt64 entityTypeId, StoryData world)
        {
            _entityId = entityId;
            _entityTypeId = entityTypeId;
            _traits = new List<Trait>();
            _relationships = new List<Relationship>();
      
            //TODO eventually: Make this automatically pull all traits and relationships from
            //the entity type associated with this entity
            _traits.Add(
                new Trait("Name", TraitDataType.Text, name, 0, world)
                );
         

        }
        public StoryEntity(string name, UInt64 entityTypeId, StoryData world)
        {
            if (world == null)
            {
                _entityId = 0;
            }
            else
            {
                _entityId = world.getNewId();
            }

            _entityTypeId = entityTypeId;
            _traits = new List<Trait>();
            _relationships = new List<Relationship>();

            //TODO eventually: Make this automatically pull all traits and relationships from
            //the entity type associated with this entity
            _traits.Add(
                new Trait("Name", TraitDataType.Text, name, 0, world)
                );
        }
      
        #region ICloneable Members

        virtual public Object Clone()
        {

            StoryEntity newClone = (StoryEntity)MemberwiseClone();
            newClone.Id = StoryWorldDataProvider.getStoryData().getNewId();

            newClone.Traits = new List<Trait>();
            newClone.Relationships = new List<Relationship>();

            foreach (Trait traitItem in _traits)
            {
                newClone.Traits.Add((Trait)traitItem.Clone());
            }

            foreach (Relationship relItem in _relationships)
            {
                Relationship newRel = (Relationship)relItem.Clone();
                //Hook up fromId properly
                newRel.FromId = newClone.Id;

                newClone.Relationships.Add(newRel);
            }

            //Give the clone a unique name, making sure the new name is actually unique.
            //If name is not unique, keep increment suffix number until it is
           // string newName = (string)newClone.Name;
           // int cloneCount = 1;
          //  while (StoryWorldDataProvider.getStoryData().findCharacterByName(newName) != null)
           // {
           //     newName = newClone.Name + "_clone" + cloneCount;
               // cloneCount++;
           // }

           // newClone.Name = newName;

            return newClone;
        }

        #endregion

        public Object Name
        {
  
            get 
            {
              Trait foundTrait = findTraitByName("Name");
              if (foundTrait != null)
              { 
                  return foundTrait.Value; 
              }
              return "";
            }
            set 
            { 
              Trait foundTrait = findTraitByName("Name");
              if (foundTrait != null)
              { 
                  foundTrait.Value =  value; 
              }
            }
        }

        public UInt64 Id 
        {
            get { return _entityId; }
            set { _entityId = value; }
        }

        public UInt64 TypeId
        {
            get { return _entityTypeId; }
            set { _entityTypeId = value; }
        }

        public List<Trait> Traits 
        {
            get { return _traits; }
            set { _traits = value; }
        }

        public List<Relationship> Relationships
        {
            get { return _relationships; }
            set { _relationships = value; }
        }

        public Trait findTraitByName(string name) {

            foreach (Trait currTrait in _traits)
            {
                if(currTrait.Name.Equals(name))
                {
                    return currTrait;
                }
            }
            return null;
        }

        public Relationship findRelationshipByName(string name)
        {

            foreach (Relationship currRel in _relationships)
            {
                if (currRel.Name.Equals(name))
                {
                    return currRel;
                }
            }
            return null;
        }

        public Relationship getRelationshipByTypeId(UInt64 typeId)
        {

            foreach (Relationship currRel in _relationships)
            {

                if (currRel.TypeId == typeId)
                {
                    return currRel;
                }
            }

            return null;
        }

        public Trait getTraitByTypeId(UInt64 typeId)
        {

            foreach (Trait currTrait in _traits)
            {

                if (currTrait.TypeId == typeId)
                {
                    return currTrait;
                }
            }

            return null;
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    [Serializable()]
    public class PreconditionStatementCharacter : PreconditionStatementEntity
    {

  

        public PreconditionStatementCharacter(UInt64 parentPlotFragmentId, StoryData world) :
            base(parentPlotFragmentId, world)
        {
     
        }

        override public string Description
        {

            get
            {

                string desc = "";
                if (_storyObjectExists)
                {
                    desc += "There exists a Character";
                }
                else
                {
                    desc += "There does not exist a Character";
                }

                if(SaveMatchedObject)
                {
                    desc += ", saved as \"" + SaveObjectVariableName + "\"";
                }
                if(_constraints.Count > 0)
                {
                    desc += ", where ";
                }
                int consCount = _constraints.Count;
                int counter = 0;

                foreach(Constraint cons in _constraints)
                {
                    counter++;
                    desc += cons.Description;
                    if(counter != consCount)
                    {
                        desc += ", and ";
                    }
                }
                return desc;

            }
        }

        public override void checkAndUpdateDependences(List<Trait> previouslyBoundVars, StoryData world)
        {
            PlotFragment parentFrag = world.findPlotFragmentById(_parentPlotFragmentId);
            if (parentFrag == null)
            {
                throw new Exception("Edit Object Action not have parent Plot Fragment");
            }



            //Check to make sure all trait or relationships exist on the object to be edited, for each
            //constraint
  

            List<Trait> traitListToCompare = world.Characters[0].Traits;
            List<Relationship> relListToCompare = world.Characters[0].Relationships;

            foreach(Constraint cons in _constraints)
            {
                if(cons is TraitConstraint)
                {
                    if (null == Utilities.findTraitByNameAndType(traitListToCompare, cons.ComparisonValue.Name, cons.ComparisonValue.Type))
                    {
                        throw new Exception("A Character-matching precondition statement in Plot Fragment \"" +
                          parentFrag.Name + "\" refers to trait variable \"" + cons.ComparisonValue.Name +
                          "\", \nwhich now has a different type or no longer exists.");
                
                    }
                }
                else if (cons is RelationshipConstraint)
                {
                    if (null == Utilities.findRelationshipByName(relListToCompare, cons.ComparisonValue.Name))
                    {
                        throw new Exception("A Character-matching precondition statement in Plot Fragment \"" +
                          parentFrag.Name + "\" refers to a relationship variable \"" + cons.ComparisonValue.Name +
                          "\", \nwhich now has a different type or no longer exists.");
                
                    }
                }
               
            }

            //Now do individual checks
            foreach (Constraint cons in _constraints)
            {
                cons.checkAndUpdateDependences(previouslyBoundVars, world);
            }
        }

    }
}

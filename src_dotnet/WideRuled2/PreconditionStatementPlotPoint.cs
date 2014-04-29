using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    [Serializable()]
    public class PreconditionStatementPlotPoint : PreconditionStatementEntity
    {
        private UInt64 _plotPointTypeId;



        public PreconditionStatementPlotPoint(UInt64 parentPlotFragmentId, PlotPointType type, StoryData world) :
            base(parentPlotFragmentId, world)
        {
            _plotPointTypeId = type.Id;
         
        }

        public UInt64 MatchTypeId
        {
            get { return _plotPointTypeId; }
            set { _plotPointTypeId = value; }
        }

        override public string Description
        {

            get
            {

                PlotPointType ppType = StoryWorldDataProvider.getStoryData().findPlotPointTypeById(_plotPointTypeId);
                string typeString = "*UNKNOWN PLOT POINT TYPE*";
                if(ppType != null)
                {
                    typeString = ppType.Description;
                }

                string desc = "";

                if (_storyObjectExists)
                {
                    desc += "There exists a " + typeString;
                }
                else
                {
                    desc += "There does not exist a " + typeString;
                }
                if (SaveMatchedObject)
                {
                    desc += ", saved as \"" + SaveObjectVariableName + "\"";
                }
                if (_constraints.Count > 0)
                {
                    desc += ", where ";
                }
                int consCount = _constraints.Count;
                int counter = 0;
                foreach (Constraint cons in _constraints)
                {
                    counter++;
                    desc += cons.Description;
                    if (counter != consCount)
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


            List<Trait> traitListToCompare = null;

            PlotPointType currType = world.findPlotPointTypeById(_plotPointTypeId);
            if (currType != null)
            {
                traitListToCompare = currType.Traits;
            }
            else
            {
                throw new Exception("A Plot Point-matching Precondition Plot Fragment \"" +
                    parentFrag.Name + "\" refers to a Plot Point type that no longer exists.");
            }

            

            foreach (Constraint cons in _constraints)
            {
                if (cons is TraitConstraint)
                {
                    if (null == Utilities.findTraitByNameAndType(traitListToCompare, cons.ComparisonValue.Name, cons.ComparisonValue.Type))
                    {
                        throw new Exception("A " + currType.Description + "-matching precondition statement in Plot Fragment \"" +
                          parentFrag.Name + "\" refers to trait variable \"" + cons.ComparisonValue.Name +
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

        #region ICloneable Members

        override public Object Clone()
        {
            //TODO: make cloning work properly for duplication of entire plot fragments and author goals
            //- add a new clone method or something to 
            //stop the creation of new unique names/variable names
            PreconditionStatementPlotPoint newClone = (PreconditionStatementPlotPoint)MemberwiseClone();
            newClone.Id = StoryWorldDataProvider.getStoryData().getNewId();

            newClone.Constraints = new List<Constraint>();
            foreach (Constraint cons in _constraints)
            {

                newClone.Constraints.Add((Constraint)cons.Clone());
            }


            //If name is not unique, keep increment suffix number until it is

            string newName = newClone.SaveObjectVariableName;
            int cloneCount = 1;
            PlotFragment parentFrag = StoryWorldDataProvider.getStoryData().findPlotFragmentById(_parentPlotFragmentId);


            Action nullAction = null;
            List<string> prevVars = parentFrag.getAllPreviouslyBoundVariableNames(nullAction, true);


            while (prevVars.Contains(newName))
            {
                newName = newClone.SaveObjectVariableName + cloneCount.ToString();
                cloneCount++;
            }

            newClone.SaveObjectVariableName = newName;


            return newClone;
        }

        #endregion



    }
}

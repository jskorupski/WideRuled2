using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    [Serializable()]
    public class RelationshipConstraint : Constraint
    {
        private bool _targetNameMode;


        public RelationshipConstraint(UInt64 parentPlotFragmentId, UInt64 parentPrecondStatementId, bool allowedToSave, bool targetNameMode, UInt64 matchingEntityTypeId, StoryData world) :
            base(parentPlotFragmentId, parentPrecondStatementId, allowedToSave, matchingEntityTypeId, world)
        {
            _targetNameMode = targetNameMode;
        }


        public RelationshipConstraint(TraitConstraint cons, StoryData world) :
            base(cons.ParentPlotFragmentId, cons.ParentPreconditionStatementId, cons.AllowedToSave, cons.MatchingEntityTypeId, world)
        {


            _constraintType = cons.ConstraintType;
            _comparisonValue = (Parameter)cons.ComparisonValue.Clone();
            _saveAttribute = cons.ContainsSavedVariable;
            _savedVariable = (Trait)cons.SavedVariable.Clone();

        }


        public bool TargetNameMode
        {
            get { return _targetNameMode; }
            set { _targetNameMode = value; }
        }
        public override string Description
        {
            get
            {
                string desc = "Relationship \"" + _comparisonValue.Name + "\"";

                if(_targetNameMode)
                {
                    desc += " target name";
                }
                else
                {
                    desc += " strength";
                }
                switch (_constraintType)
                {
                    case ConstraintComparisonType.Equals:
                        desc +=  " == " + _comparisonValue.LiteralValueOrBoundVarName;
                	    break;
                    case ConstraintComparisonType.NotEquals:
                        desc += " != " + _comparisonValue.LiteralValueOrBoundVarName;
                        break;
                    case ConstraintComparisonType.LessThan:
                        desc += " < " + _comparisonValue.LiteralValueOrBoundVarName;
                        break;
                    case ConstraintComparisonType.GreaterThan:
                        desc += " > " + _comparisonValue.LiteralValueOrBoundVarName;
                        break;
                    case ConstraintComparisonType.LessThanEqualTo:
                        desc += " <= " + _comparisonValue.LiteralValueOrBoundVarName;
                        break;
                    case ConstraintComparisonType.GreaterThanEqualTo:
                        desc += " >= " + _comparisonValue.LiteralValueOrBoundVarName;
                        break;
                }


                if (_constraintType == ConstraintComparisonType.None)
                {
                    desc += " is saved as variable \"" + _savedVariable.Name + "\"";
                }
                else if(_saveAttribute)
                {
                    desc += ", saved as variable \"" + _savedVariable.Name + "\"";
                }

                return desc;
            }
        }

        override public void checkAndUpdateDependences(List<Trait> previouslyBoundVars, StoryData world)
        {
            base.checkAndUpdateDependences(previouslyBoundVars, world);
            //Additionally, check that trait names exist in the class of object that this is constrained on
            //Check to make sure all trait or relationships exist on the object to be edited
            PlotFragment parentFrag = world.findPlotFragmentById(_parentPlotFragmentId);
            
            List<Relationship> relListToCompare = new List<Relationship>();

            if (_matchingEntityTypeId == world.CharTypeId)
            {
                if (world.Characters.Count > 0)
                {
                   
                    relListToCompare = world.Characters[0].Relationships;
                }

            }
            else if (_matchingEntityTypeId == world.EnvTypeId)
            {

                if (world.Environments.Count > 0)
                {
                    
                    relListToCompare = world.Environments[0].Relationships;
                }
            }
          

            //look for trait we are editing
            if (null == Utilities.findRelationshipByName(relListToCompare, _comparisonValue.Name))
            {
                throw new Exception("A Relationship constraint within a precondition statement within Plot Fragment \"" +
                    parentFrag.Name + "\" refers to a relationship with name \"" +
                    _comparisonValue.Name + 
                    "\" that no longer exists.");
            }


        }


        #region ICloneable Members

        override public Object Clone()
        {
            //TODO: make cloning work properly for duplication of entire plot fragments and author goals
            //- add a new clone method or something to 
            //stop the creation of new unique names/variable names
            RelationshipConstraint newClone = (RelationshipConstraint)MemberwiseClone();
            newClone.Id = StoryWorldDataProvider.getStoryData().getNewId();

            newClone.ComparisonValue = (Parameter)_comparisonValue.Clone();
            newClone.SavedVariable = (Trait)_savedVariable.Clone();


            //Make a new unique variable name
            if (newClone.ContainsSavedVariable)
            {
                string newName = newClone.SavedVariable.Name;
                int cloneCount = 1;

                Action nullAction = null;
                List<string> prevVars = StoryWorldDataProvider.getStoryData().findPlotFragmentById(_parentPlotFragmentId).getAllPreviouslyBoundVariableNames(nullAction, true);



                while (prevVars.Contains(newName))
                {
                    newName = newClone.SavedVariable.Name + cloneCount.ToString();
                    cloneCount++;
                }

                newClone.SavedVariable.Name = newName;
            }
           

            return newClone;
        }

        #endregion

    }
}

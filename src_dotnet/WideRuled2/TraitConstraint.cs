using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    [Serializable()]
    public class TraitConstraint : Constraint
    {
        public TraitConstraint(UInt64 parentPlotFragmentId, UInt64 parentPrecondStatementId, bool allowedToSave, UInt64 matchingEntityTypeId, StoryData world) :
            base(parentPlotFragmentId, parentPrecondStatementId, allowedToSave, matchingEntityTypeId, world)
        {

        }

        public TraitConstraint(RelationshipConstraint cons, StoryData world) :
            base(cons.ParentPlotFragmentId, cons.ParentPreconditionStatementId, cons.AllowedToSave, cons.MatchingEntityTypeId, world)
        {


            _constraintType = cons.ConstraintType;
            _comparisonValue = (Parameter)cons.ComparisonValue.Clone();
            _saveAttribute = cons.ContainsSavedVariable;
            _savedVariable = (Trait)cons.SavedVariable.Clone();

        }

        override public void checkAndUpdateDependences(List<Trait> previouslyBoundVars, StoryData world)
        {
            base.checkAndUpdateDependences(previouslyBoundVars, world);
            //Additionally, check that trait names exist in the class of object that this is constrained on
            //Check to make sure all trait or relationships exist on the object to be edited
            List<Trait> traitListToCompare = new List<Trait>();

            PlotFragment parentFrag = world.findPlotFragmentById(_parentPlotFragmentId);
            
            if (_matchingEntityTypeId == world.CharTypeId)
            {
                if (world.Characters.Count > 0)
                {
                    traitListToCompare = world.Characters[0].Traits;
                    
                }

            }
            else if (_matchingEntityTypeId == world.EnvTypeId)
            {

                if (world.Environments.Count > 0)
                {
                    traitListToCompare = world.Environments[0].Traits;
                    
                }
            }
            else
            {
                PlotPointType currType = world.findPlotPointTypeById(_matchingEntityTypeId);
                if (currType != null)
                {
                    traitListToCompare = currType.Traits;
                }
                else
                {
                    throw new Exception("A Trait constraint within a precondition statement within Plot Fragment \"" +
                        parentFrag.Name + "\" refers to a Plot Point type that no longer exists.");
                }

            }

            //look for trait we are editing
            if (null == Utilities.findTraitByNameAndType(traitListToCompare, _comparisonValue.Name, _comparisonValue.Type))
            {
                throw new Exception("A Trait constraint within a precondition statement within Plot Fragment \"" +
                    parentFrag.Name + "\" refers to a trait with name \"" +
                    _comparisonValue.Name + "\" and type \"" +
                    Trait.TraitDataTypeToString(_comparisonValue.Type) +
                    "\" that no longer exists.");
            }

           
        }

        public override string Description
        {
            get
            {
                string desc  = "";
    
                desc += "Trait \"" + _comparisonValue.Name + "\"";
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
                else if (_saveAttribute)
                {
                    desc += ", saved as variable \"" + _savedVariable.Name + "\"";

                }

                return desc;
            }
        }

     

      

    }
}

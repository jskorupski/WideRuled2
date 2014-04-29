using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    public enum ConstraintComparisonType {None, Equals, NotEquals, LessThan, GreaterThan, LessThanEqualTo, GreaterThanEqualTo};
    [Serializable()]
    public class Constraint : ICloneable, DependencyHolder
    {
        protected UInt64 _constraintId;
        protected UInt64 _parentPrecondStmtId;
        protected UInt64 _parentPlotFragmentId;
        protected ConstraintComparisonType _constraintType;
        protected Parameter _comparisonValue; //maps name to value
        protected bool _saveAttribute;
        protected Trait _savedVariable;
        protected bool _allowedToSave;
        protected UInt64 _matchingEntityTypeId;
        protected bool _mustAlwaysBeTrue;



        public Constraint(UInt64 parentPlotFragmentId, UInt64 parentPrecondStatementId, bool allowedToSave, UInt64 matchingEntityTypeId, StoryData world)
        {
            _matchingEntityTypeId = matchingEntityTypeId;
            _constraintId = world.getNewId();
            _parentPrecondStmtId = parentPrecondStatementId;
            _allowedToSave = allowedToSave;
            _parentPlotFragmentId = parentPlotFragmentId;
            _constraintType = ConstraintComparisonType.Equals;
            _comparisonValue = new Parameter("", TraitDataType.Text, false, world);
            _saveAttribute = false;
            _savedVariable = new Trait("", TraitDataType.Text, "", 0, world);
            _mustAlwaysBeTrue = false;

        }

        public UInt64 Id
        {
            get { return _constraintId; }
            set { _constraintId = value; }
        }

        public bool AllowedToSave
        {
            get { return _allowedToSave; }
            set 
            { 
                _allowedToSave = value; 
                if(!_allowedToSave)
                {
                    _saveAttribute = false;

                }
            }
        }

        public bool MustAlwaysBeTrue {
            get { return _mustAlwaysBeTrue; }
            set { _mustAlwaysBeTrue = value; }
        }
        public bool ContainsSavedVariable
        {
            get { return _saveAttribute; }
            set { _saveAttribute = value; }
        }
        public Trait SavedVariable
        {
            get { return _savedVariable; }
            set { _savedVariable = value; }
        }
        public Parameter ComparisonValue
        {
            get { return _comparisonValue; }
            set { _comparisonValue = value; }
        }
        public UInt64 MatchingEntityTypeId
        {
            get { return _matchingEntityTypeId; }
            set { _matchingEntityTypeId = value; }
        }
        public ConstraintComparisonType ConstraintType
        {
            get { return _constraintType; }
            set 
            { 
                _constraintType = value; 
                if(_constraintType == ConstraintComparisonType.None)
                {
                    ComparisonValue.ValueIsBoundToVariable = false;
                }
            }
        }
        public UInt64 ParentPreconditionStatementId
        {
            get { return _parentPrecondStmtId; }
            set { _parentPrecondStmtId = value; }
        }
        public UInt64 ParentPlotFragmentId
        {
            get { return _parentPlotFragmentId; }
            set { _parentPlotFragmentId = value; }
        }
        virtual public string Description
        {
            get { return "Unknown constraint";  }
        }

        //Return ordered list of bound variable names
        virtual public List<string> getAllBoundVariableNames()
        {

            if (_saveAttribute)
            {
                List<string> newVarList = new List<string>();
                newVarList.Add(_savedVariable.Name);
                return newVarList;
            }
            else
            {
                return new List<string>();
            }

        }

        //Return ordered list of bound variable names
        virtual public List<Trait> getBoundPrimitiveVariables(TraitDataType type, bool allTypes)
        {
            if(_saveAttribute && (allTypes || (_savedVariable.Type == type)))
            {
                List<Trait> newVarList = new List<Trait>();
                newVarList.Add(_savedVariable);
                return newVarList;
            }
            else
            {
                return new List<Trait>();
            }

        }

 

        virtual public void checkAndUpdateDependences(List<Trait> previouslyBoundVars, StoryData world)
        {

            PlotFragment parentFrag = world.findPlotFragmentById(_parentPlotFragmentId);
            if (parentFrag == null)
            {
                throw new Exception("Delete Object Action not have parent Plot Fragment");
            }


            

            //check if variable to compare to exists
            if (_comparisonValue.ValueIsBoundToVariable)
            {
                if (null == Utilities.findTraitByNameAndType(previouslyBoundVars, (string)_comparisonValue.LiteralValueOrBoundVarName, _comparisonValue.Type))
                {
                    throw new Exception("A precondition statement in Plot Fragment \"" +
                           parentFrag.Name + "\" refers to variable \"" + (string)_comparisonValue.LiteralValueOrBoundVarName +
                           "\", \nwhich has a different type or does not exist in any previous Author Goal parameters or precondition statements.");
                }

            }

            //Add new variable to running list of bound vars
            if (_saveAttribute)
            {
                previouslyBoundVars.Add(_savedVariable);
            }

        }
     
        public static string ConstraintComparisonTypeToString(ConstraintComparisonType cons)
        {
            switch(cons)
            {
                case ConstraintComparisonType.Equals:
                    return "==";
                  
                case ConstraintComparisonType.NotEquals:
                    return "!=";
                
                case ConstraintComparisonType.GreaterThan:
                    return ">";
                
                case ConstraintComparisonType.LessThan:
                    return "<";
               
                case ConstraintComparisonType.GreaterThanEqualTo:
                    return ">=";
                 
                case ConstraintComparisonType.LessThanEqualTo:
                    return "<=";
             
                case ConstraintComparisonType.None:
                    return "Save Only";
                  
            }
            return "";
        }

        #region ICloneable Members

        virtual public Object Clone()
        {
            //TODO: make cloning work properly for duplication of entire plot fragments and author goals
            //- add a new clone method or something to 
            //stop the creation of new unique names/variable names

            Constraint newClone = (Constraint)MemberwiseClone();
            newClone.Id = StoryWorldDataProvider.getStoryData().getNewId();

            newClone.ComparisonValue = (Parameter)_comparisonValue.Clone();
            newClone.SavedVariable = (Trait)_savedVariable.Clone();

            //Make a new unique variable name
            if(newClone.ContainsSavedVariable)
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

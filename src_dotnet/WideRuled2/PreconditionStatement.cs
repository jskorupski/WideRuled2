using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    [Serializable()]
    public class PreconditionStatement : ICloneable, DependencyHolder
    {
        protected UInt64 _precId;
        protected UInt64 _parentPlotFragmentId;
        protected bool _storyObjectExists;
        protected string _saveObjectVariableName;
        protected bool _saveMatch;

        protected List<Constraint> _constraints;


        public PreconditionStatement(UInt64 parentPlotFragmentId, StoryData world)
        {
            _precId = world.getNewId();
            _parentPlotFragmentId = parentPlotFragmentId;
            _storyObjectExists = true;
            _saveObjectVariableName = "";
            _saveMatch = false;
            _constraints = new List<Constraint>();
        }


        public List<Constraint> Constraints
        {
            get { return _constraints; }
            set { _constraints = value; }
        }

        public UInt64 Id
        {
            get { return _precId; }
            set { _precId = value; }
        }
        public UInt64 ParentPlotFragmentId
        {

            get { return _parentPlotFragmentId; }
            set { _parentPlotFragmentId = value; }
        }

        public bool SaveMatchedObject
        {
            get { return _saveMatch; }
            set { _saveMatch = value; }
        }
        public string SaveObjectVariableName
        {
            get { return _saveObjectVariableName; }
            set { _saveObjectVariableName = value; }
        }
        public bool ObjectExists
        {
            get { return _storyObjectExists; }
            set { _storyObjectExists = value; }
        }

        public List<Constraint> getAlwaysTrueConstraints() 
        {
            List<Constraint> newList = new List<Constraint>();
            foreach(Constraint cons in _constraints)
            {
                if (cons.MustAlwaysBeTrue)
                {
                    newList.Add(cons);
                }
            }
            return newList;


        }
        public List<Trait> getPreviouslyBoundPrimitiveVariables(TraitDataType variableType, bool allTypes, Constraint currentConstraint)
        {


            PlotFragment parentFrag = StoryWorldDataProvider.getStoryData().findPlotFragmentById(_parentPlotFragmentId);
            List<Trait> varsToReturn = parentFrag.getPreviouslyBoundPrimitiveVariables(variableType, allTypes, this);


            foreach (Constraint cons in Constraints)
            {
                if (cons == currentConstraint)
                {
                    //We have found where to stop looking, can now return variable list
                    return varsToReturn;
                }
                else if (cons.ContainsSavedVariable)
                {
                    if (allTypes || (cons.SavedVariable.Type == variableType))
                    {
                        //Add the variable names bound in this constraint to the running list
                        varsToReturn.Add(cons.SavedVariable);
                    }

                }
            }

            return varsToReturn;
        }


        public List<string> getAllPreviouslyBoundVariableNames(Constraint currentConstraint)
        {

            PlotFragment parentFrag = StoryWorldDataProvider.getStoryData().findPlotFragmentById(_parentPlotFragmentId);
            List<string> varsToReturn = parentFrag.getAllPreviouslyBoundVariableNames(this, true);




            foreach (Constraint cons in Constraints)
            {
                if (cons == currentConstraint)
                {
                    //We have found where to stop looking, can now return variable list
                    return varsToReturn;
                }
                else if (cons.ContainsSavedVariable)
                {
                   
                        //Add the variable names bound in this constraint to the running list
                        varsToReturn.Add(cons.SavedVariable.Name);
                    

                }
            }

            return varsToReturn;


        }



        virtual public List<string> getAllBoundVariableNames()
        {
            List<string> newVars = new List<string>();
            if(_saveMatch)
            {
                newVars.Add(_saveObjectVariableName);
            }
            
            foreach (Constraint cons in _constraints)
            {
                newVars.AddRange(cons.getAllBoundVariableNames());
            }

            return newVars;

        }

        
        //Return ordered list of bound variable names
        virtual public List<Trait> getBoundPrimitiveVariables(TraitDataType type, bool allTypes)
        {
            List<Trait> newVars = new List<Trait>();

            foreach(Constraint cons in _constraints)
            {
                newVars.AddRange(cons.getBoundPrimitiveVariables(type, allTypes));
            }

            return newVars;
        }
       

        virtual public void checkAndUpdateDependences(List<Trait> previouslyBoundVars, StoryData world)
        {
            throw new NotImplementedException();

        }

        virtual public string Description
        {

            get
            {

                return "Unknown precondition statement";
            }
        }

        
        #region ICloneable Members

        virtual public Object Clone()
        {
            //TODO: make cloning work properly for duplication of entire plot fragments and author goals
            //- add a new clone method or something to 
            //stop the creation of new unique names/variable names
            PreconditionStatement newClone = (PreconditionStatement)MemberwiseClone();
            newClone.Id = StoryWorldDataProvider.getStoryData().getNewId();

            newClone.Constraints = new List<Constraint>();

            foreach(Constraint cons in _constraints)
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

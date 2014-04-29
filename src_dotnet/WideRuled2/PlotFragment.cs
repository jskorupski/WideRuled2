using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    [Serializable()]
    public class PlotFragment : DependencyHolder, ICloneable
    {

        private UInt64 _parentAuthorGoalId;
        private UInt64 _plotFragId;
        private string _name;

        private List<PreconditionStatement> _precStatements;
        private List<Action> _actions;


        public PlotFragment(string name, UInt64 parentAuthorGoalId, StoryData world)
        {
            _name = name;
            _plotFragId = world.getNewId();
            _parentAuthorGoalId = parentAuthorGoalId;

            _precStatements = new List<PreconditionStatement>();
            _actions = new List<Action>();


                    

        }

        #region ICloneable Members

        virtual public Object Clone()
        {

            //TODO: make cloning work properly for duplication of entire plot fragments and author goals
            //- add a new clone method or something to 
            //stop the creation of new unique names/variable names
            PlotFragment newClone = (PlotFragment)MemberwiseClone();
            newClone.Id = StoryWorldDataProvider.getStoryData().getNewId();


            newClone.PrecStatements = new List<PreconditionStatement>();
            newClone.Actions = new List<Action>();

            foreach (PreconditionStatement prec in _precStatements)
            {

                //This clone operation will result in variable names with "1" appended,
                //because the clone operation checks the current parent plot fragment
                //and attempts to create unique variable names
                newClone.PrecStatements.Add((PreconditionStatement)prec.Clone());
            }

            foreach (Action act in _actions)
            {

                //This clone operation will result in variable names with "1" appended,
                //because the clone operation checks the current parent plot fragment
                //and attempts to create unique variable names
                newClone.Actions.Add((Action)act.Clone());
            }


            string newName = newClone.Name;
            int cloneCount = 1;
            
            List<string> prevVars = new List<string>();
            foreach(PlotFragment frag in StoryWorldDataProvider.getStoryData().findAuthorGoalById(_parentAuthorGoalId).PlotFragments)
            {
                prevVars.Add(frag.Name);
            }


            while (prevVars.Contains(newName))
            {
                newName = newClone.Name + cloneCount.ToString();
                cloneCount++;
            }

            newClone.Name = newName;


            
            return newClone;
        }

        #endregion


        public string Name {

            get { return _name; }
            set { _name = value; }
        }

        public UInt64 Id
        {

            get { return _plotFragId; }
            set { _plotFragId = value; }
        }

        public UInt64 ParentAuthorGoalId
        {
            get { return _parentAuthorGoalId; }
            set { _parentAuthorGoalId = value; }
        }

        public PreconditionStatement findPreconditionStatementById(UInt64 id)
        {
            foreach(PreconditionStatement statement in _precStatements)
            {
                if(statement.Id == id)
                {
                    return statement;
                }
            }

            return null;
        }

        public void checkAndUpdateDependences(List<Trait> previouslyBoundVars, StoryData world)
        {

          
            foreach (PreconditionStatement prec in _precStatements)
            {
                prec.checkAndUpdateDependences(previouslyBoundVars, world);
            }

            foreach (Action action in _actions)
            {
                action.checkAndUpdateDependences(previouslyBoundVars, world);
            }
        }

        public bool containsTextOutputAction()
        {
            foreach (Action act in _actions)
            {
                if (act is ActionTextOutput)
                {
                    return true;
                }
            }
            return false;
        }

        public List<string> getCalculationVariables()
        {
            List<string> varNames = new List<string>();
            foreach(Action act in _actions)
            {
                if(act is ActionCalculation)
                {
                    varNames.Add(((ActionCalculation)act).ResultVarName);
                }
            }
            return varNames;
        }

        public List<string> getNewCharacterActionVarNames(UInt64 charTypeId)
        {
            List<string> varNames = new List<string>();
            foreach (Action act in _actions)
            {
                if (act is ActionCreateCharacter)
                {
                    varNames.Add(((ActionCreateCharacter)act).VariableName);
                }
            }
            return varNames;
        }

        public List<string> getNewEnvironmentActionVarNames(UInt64 charTypeId)
        {
            List<string> varNames = new List<string>();
            foreach (Action act in _actions)
            {
                if (act is ActionCreateEnvironment)
                {
                    varNames.Add(((ActionCreateEnvironment)act).VariableName);
                }
            }
            return varNames;
        }

        public List<string> getNewPlotPointActionVarNames(PlotPointType type)
        {
            List<string> varNames = new List<string>();
            foreach (Action act in _actions)
            {
                if ((act is ActionCreatePlotPoint) && (((ActionCreatePlotPoint)act).NewPlotPoint.TypeId == type.Id))
                {
                    varNames.Add(((ActionCreatePlotPoint)act).VariableName);
                }
            }
            return varNames;
        }
 
        public List<string> getPreviouslyBoundCharacterVarNames(Action currentAction)
        {
            List<string> charVars = new List<string>();
            foreach (PreconditionStatement precStmt in _precStatements)
            {
                if ((precStmt is PreconditionStatementCharacter) && ((PreconditionStatementCharacter)precStmt).SaveMatchedObject)
                {
                    charVars.Add(((PreconditionStatementCharacter)precStmt).SaveObjectVariableName);
                }

            }

            foreach(Action actItem in _actions)
            {
                //We have found where to stop looking, can now return variable list
                if(actItem == currentAction)
                {
                    return charVars;
                }
                else if(actItem is ActionCreateCharacter)
                {
                    charVars.Add(((ActionCreateCharacter)actItem).VariableName);
                }
            }

            return charVars;
        }


        public List<string> getPreviouslyBoundEnvironmentVarNames(Action currentAction)
        {
            List<string> envVars = new List<string>();
            foreach (PreconditionStatement precStmt in _precStatements)
            {
                if ((precStmt is PreconditionStatementEnvironment) && ((PreconditionStatementEnvironment)precStmt).SaveMatchedObject)
                {
                    envVars.Add(((PreconditionStatementEnvironment)precStmt).SaveObjectVariableName);
                }

            }

            foreach (Action actItem in _actions)
            {
                //We have found where to stop looking, can now return variable list
                if (actItem == currentAction)
                {
                    return envVars;
                }
                else if (actItem is ActionCreateEnvironment)
                {
                    envVars.Add(((ActionCreateEnvironment)actItem).VariableName);
                }
            }

            return envVars;
        }


        public List<string> getPreviouslyBoundPlotPointTypeVarNames(PlotPointType type, Action currentAction)
        {
            List<string> ppVars = new List<string>();
            foreach (PreconditionStatement precStmt in _precStatements)
            {
                //Match the ID of the type now, since plot points have different types
                if (
                     (precStmt is PreconditionStatementPlotPoint) &&
                     (((PreconditionStatementPlotPoint)precStmt).MatchTypeId == type.Id) &&
                     ((PreconditionStatementPlotPoint)precStmt).SaveMatchedObject
                   )
                {
                    ppVars.Add(((PreconditionStatementPlotPoint)precStmt).SaveObjectVariableName);
                }

            }

            foreach (Action actItem in _actions)
            {
                //We have found where to stop looking, can now return variable list
                if (actItem == currentAction)
                {
                    return ppVars;
                }
                else if (
                          (actItem is ActionCreatePlotPoint) && 
                         (((ActionCreatePlotPoint)actItem).NewPlotPoint.TypeId == type.Id)
                         )
                {
                    ppVars.Add(((ActionCreatePlotPoint)actItem).VariableName);
                }
            }

            return ppVars;
        }

        public List<string> getAllPreviouslyBoundVariableNames(PreconditionStatement currentPrecStatement, bool includeAuthorGoalParameters)
        {
            List<string> varsToReturn = new List<string>();

            if (includeAuthorGoalParameters) 
            {
                AuthorGoal parentGoal = StoryWorldDataProvider.findAuthorGoalById(_parentAuthorGoalId);

             
                // Add parameter variable names
                foreach (Parameter param in parentGoal.Parameters)
                {

                    varsToReturn.Add(param.Name);

                }

            }


            foreach (PreconditionStatement precondStmt in _precStatements)
            {
                if (precondStmt == currentPrecStatement)
                {
                    //We have found where to stop looking, can now return variable list
                    return varsToReturn;
                }
                else
                {
                    //Add the variable names bound in this statement to the running list
                    varsToReturn.AddRange(precondStmt.getAllBoundVariableNames());
                }
            }

            return varsToReturn;


        }

        public List<string> getAllPreviouslyBoundVariableNames(Action currentAction, bool includeAuthorGoalParameters)
        {
            PreconditionStatement nullPrecStatement = null;
            List<string> varsToReturn = getAllPreviouslyBoundVariableNames(nullPrecStatement, includeAuthorGoalParameters);



            foreach (Action actItem in _actions)
            {
                //We have found where to stop looking, can now return variable list
                if (actItem == currentAction)
                {
                    return varsToReturn;
                }
                else if (actItem is ActionCreateObject)
                {
                    varsToReturn.Add(((ActionCreateObject)actItem).VariableName);
                }
                else if (actItem is ActionCalculation)
                {
                    varsToReturn.Add(((ActionCalculation)actItem).ResultVarName);
                }
            }
            return varsToReturn;

        }

        public List<Trait> getPreviouslyBoundPrimitiveVariables(TraitDataType variableType, bool allTypes, PreconditionStatement currentPrecStatement)
        {
            List<Trait> varsToReturn = new List<Trait>();
          

            AuthorGoal parentGoal = StoryWorldDataProvider.findAuthorGoalById(_parentAuthorGoalId);

            // Add parameter variable names
            foreach (Parameter param in parentGoal.Parameters)
            {
                if(allTypes || (param.Type == variableType))
                {
                    varsToReturn.Add(param);
                }
                
            }

            foreach (PreconditionStatement precondStmt in _precStatements)
            {
                if(precondStmt == currentPrecStatement)
                {
                    //We have found where to stop looking, can now return variable list
                    return varsToReturn;
                }
                else
                {
                    //Add the variable names bound in this statement to the running list
                    varsToReturn.AddRange(precondStmt.getBoundPrimitiveVariables(variableType, allTypes));
                }
            }

            return varsToReturn;
        }

  
        public List<Trait> getPreviouslyBoundPrimitiveVariables(TraitDataType variableType, bool allTypes, Action currentAction)
        {

            //Get bound variable names in parameters and preconditions by passing
            //a non-existent PreconditionStatement reference (null) to the other variable
            //name collection function

            PreconditionStatement nullPrecStatement = null;
            List<Trait> varsToReturn = getPreviouslyBoundPrimitiveVariables(variableType, allTypes, nullPrecStatement);



            foreach (Action action in _actions)
            {
                if (action == currentAction)
                {
                    //We have found where to stop looking, can now return variable list
                    return varsToReturn;
                }
                else
                {
                    //Add the variable names bound in this statement to the running list
                    varsToReturn.AddRange(action.getBoundPrimitiveVariables(variableType, allTypes));
                }
            }

            return varsToReturn;

        }

      


        public List<PreconditionStatement> PrecStatements
        {
            get { return _precStatements; }
            set { _precStatements = value; }
        }


        public List<Action> Actions
        {
            get { return _actions; }
            set { _actions = value; }
        }
        public List<ActionSubgoal> SubGoals
        {
           
            get
            {
                List<ActionSubgoal> newList = new List<ActionSubgoal>();

                foreach (Action act in _actions)
                {
                    if(act.GetType() == typeof(ActionSubgoal))
                    {
                        newList.Add((ActionSubgoal)act);
                    }
                }
                return newList;
            }
 
        }


        public void verifyAllTraitAndRelationshipReferences()
        {
            throw new NotImplementedException();

        }

        internal void removeAllPPReferences()
        {
            throw new NotImplementedException();
        }
    }
}

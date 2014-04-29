using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    [Serializable()]
    public class ActionSubgoal : Action
    {


        private List<Parameter> _parametersToPass;
        private UInt64 _subGoalId;



        public ActionSubgoal(UInt64 parentPlotFragmentId, UInt64 subgoalId, StoryData world) :
            base(parentPlotFragmentId, world)
        {

            _subGoalId = subgoalId;
            _parametersToPass = new List<Parameter>();


            AuthorGoal subGoal = world.findAuthorGoalById(subgoalId);
            if (subGoal != null)
            {
                List<Parameter> parameters = subGoal.Parameters;
                foreach (Parameter param in parameters)
                {

                    // Add all parameters from the subgoal, which will later be filled in
                    // with literals or variables
                    _parametersToPass.Add(new Parameter(param.Name, param.Type, false, world));
                }
            }
         
        }

        public string SubGoalName
        {
            get {

                string subGoalName = "*UNKNOWN AUTHOR GOAL*";
                AuthorGoal subgoal = StoryWorldDataProvider.findAuthorGoalById(_subGoalId);
                if (subgoal != null)
                {
                    subGoalName = subgoal.Name;
                }
                return subGoalName;
            }
        }
        public UInt64 SubGoalId 
        {
            get { return _subGoalId; }
            set 
            {

                //Do nothing if subgoal id doesn't change
                if(value == _subGoalId)
                {
                    return;
                }

                AuthorGoal newSubGoal = StoryWorldDataProvider.getStoryData().findAuthorGoalById(value);
                if (newSubGoal != null)
                {
                    _parametersToPass.Clear();

                    List<Parameter> subgoalParameters = newSubGoal.Parameters;
                    foreach (Parameter param in subgoalParameters)
                    {

                        // Add all parameters from the subgoal, which will later be filled in
                        // with literals or variables
                        _parametersToPass.Add(new Parameter(param.Name, param.Type, false, StoryWorldDataProvider.getStoryData()));
                    }
                    _subGoalId = value;
                }
                
            }
        }

        public List<Parameter> ParametersToPass
        {
            get { return _parametersToPass; }
            set { _parametersToPass = value; }
        }

        override public string Description
        {

          
            get 
            {
                
                string desc = "Pursue Subgoal \"" + SubGoalName;
                if (_parametersToPass.Count > 0)
                {
                    desc += " (" + AblCodeGenerator.CommaSeparatedValuesWithInnerSpaces(_parametersToPass) + ")"; 
                }
                desc += "\"";
                return desc;
                    
            }
        }


        public void checkAndUpdateDependencesAsInteraction(Interaction parentInteraction, List<Trait> previouslyBoundVars, StoryData world)
        {
           
            AuthorGoal goalToPursue = world.findAuthorGoalById(_subGoalId);
            if (goalToPursue == null)
            {
                //clear out parameter list
                _parametersToPass.Clear();

                throw new Exception("Interaction \"" + parentInteraction.Title + "\" refers to an Author Goal which does not exist.");
            }

            if (goalToPursue.PlotFragments.Count == 0)
            {
                throw new Exception("Interaction \"" + parentInteraction.Title + "\" refers to an Author Goal which has no child plot fragments that fulfill it." +
                "\nAll Interactions must have at least one Plot Fragment associated with the goal they pursue before generation can begin.");
            }

     

            bool dataUpdated = syncParametersWithSubgoal();

            if (dataUpdated)
            {


                throw new Exception("Interaction \"" + parentInteraction.Title + "\" refers to an Author Goal that has had its parameters changed.\n" +
                "Wide Ruled has corrected the changes, but you should look at the Interaction to make sure the newly updated parameters have the correct value.");
            }
        }

        override public void checkAndUpdateDependences(List<Trait> previouslyBoundVars, StoryData world)
        {
           PlotFragment parentFrag = world.findPlotFragmentById(_parentPlotFragmentId);
            if (parentFrag == null)
            {
                throw new Exception("Pursue Subgoal Action does not have parent Plot Fragment");
            }

            
            AuthorGoal goalToPursue = world.findAuthorGoalById(_subGoalId);
            if (goalToPursue == null)
            {
                //clear out parameter list
                //_parametersToPass.Clear();

                throw new Exception("Pursue Subgoal Action in Plot Fragment \"" + parentFrag.Name + "\" refers to an Author Goal which does not exist.");
            }

            if (goalToPursue.PlotFragments.Count == 0)
            {
                throw new Exception("Pursue Subgoal Action in Plot Fragment \"" + parentFrag.Name + "\" refers to an Author Goal which has no child plot fragments that fulfill it." +
                "\nAll goals which are used during story creation must have at least one Plot Fragment associated with them before generation can begin.");
            }
           

            //Check variable references from previous plot fragment preconditions and actions

            foreach (Parameter param in _parametersToPass)
            {
                if(param.ValueIsBoundToVariable)
                {
                    bool foundIt = false;
                    foreach (Trait traitItem in previouslyBoundVars)
                    {

                        if (
                            (traitItem.Name == (string)param.LiteralValueOrBoundVarName) &&
                            (traitItem.Type == param.Type)
                            )
                        {
                            foundIt = true;
                            break;
                        }

                    }
             
                    if (!foundIt)
                    {
                        throw new Exception("Pursue Subgoal Action in Plot Fragment \"" + 
                            parentFrag.Name + "\" refers to variable \"" + param.LiteralValueOrBoundVarName +
                            "\", \nwhich has a different type or does not exist in any previous Author Goal parameters, precondition statements, or actions .");
                    }
                }
            }


            bool dataUpdated = syncParametersWithSubgoal();

            if (dataUpdated)
            {


                throw new Exception("Pursue Subgoal Action in Plot Fragment \"" + parentFrag.Name + "\" refers to an Author Goal that has had its parameters changed.\n" +
                "Wide Ruled has corrected the changes, but you should look at the Action to make sure the newly updated parameters have the correct value.");
            }
        }

        public bool syncParametersWithSubgoal()
        {
            //Check to see if parameter variables to pass are synced with subgoal to pursue
            // Add new ones, delete non-existent ones, and warn about newly instantiated ones
            // that the user has never edited due to the update

            //NOTE: This synchronization process uses the parameter name as the identifying
            // data, instead of the TypeID variable. This allows for changes in the story goal
            // to not totally destroy any old settings the user has entered. This sync process
            // is therefore not the same as the typeid-based sync for character/environment
            // traits in the Utilities class

            List<Parameter> subgoalParams = StoryWorldDataProvider.findAuthorGoalById(_subGoalId).Parameters;

            List<Parameter> syncList = new List<Parameter>();

            bool dataUpdated = false;
            foreach (Parameter param in subgoalParams)
            {
                Parameter oldParam = Utilities.findParameterByName(_parametersToPass, param.Name);
                if (oldParam == null)
                {
                    //nonexistent parameter that needs to be in the param list
                    syncList.Add(new Parameter(param, StoryWorldDataProvider.getStoryData()));
                    dataUpdated = true;
                }
                else
                {
                    //Synchronize type id's - this is not currently used, but could be in the future

                    oldParam.TypeId = param.TypeId;
                    if (oldParam.Type != param.Type)
                    {
                        dataUpdated = true;

                        //sync types - this will switch the value of the parameter.
                        oldParam.Type = param.Type;

                        //Because types were switched, variable binding is cleared as well
                        oldParam.ValueIsBoundToVariable = false;
                        oldParam.resetValue();
                    }

                    syncList.Add(oldParam);


                }
            }
            // Change list to corrected list
            _parametersToPass = syncList;

            return dataUpdated;
        }


        #region ICloneable Members

        override public Object Clone()
        {
            ActionSubgoal newClone = (ActionSubgoal)MemberwiseClone();

            newClone.Id = StoryWorldDataProvider.getStoryData().getNewId();

            newClone.ParametersToPass = new List<Parameter>();
            foreach(Parameter param in _parametersToPass)
            {
                newClone.ParametersToPass.Add((Parameter)param.Clone());
            }


            return newClone;
            
        }

        #endregion

    }
}

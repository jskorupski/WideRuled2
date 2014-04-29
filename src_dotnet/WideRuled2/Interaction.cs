using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WideRuled2
{

    //An interaction is just a string title and a subgoal to pursue
    //This way the user can make arbitrary actions the world that can
    //happen simultaneously
    [Serializable()]
    public class Interaction : DependencyHolder
    {


        private string _title;
        private UInt64 _id;
        private ActionSubgoal _interactionGoal;
       
        public Interaction (string title, ActionSubgoal interactionGoal, StoryData world) {

            _title = title;
            _id = world.getNewId();
            _interactionGoal = interactionGoal;
        }
        public UInt64 Id 
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Title 
        {
            get { return _title; }
            set { _title = value; }
        }

        public ActionSubgoal SubgoalAction
        {

            get { return _interactionGoal; }
            set { _interactionGoal = value; }
        }

        public string AuthorGoalName
        {
            get
            {
                return StoryWorldDataProvider.getStoryData().findAuthorGoalById(_interactionGoal.SubGoalId).Name;
            }
        }

        virtual public void checkAndUpdateDependences(List<Trait> previouslyBoundVars, StoryData world)
        {
         
            _interactionGoal.checkAndUpdateDependencesAsInteraction(this, previouslyBoundVars, world);
        }


    }
}

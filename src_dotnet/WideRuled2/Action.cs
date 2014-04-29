using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    [Serializable()]
    public class Action : ICloneable, DependencyHolder
    {

        protected UInt64 _actionId;
        protected UInt64 _parentPlotFragmentId;

        public Action(UInt64 parentPlotFragmentId, StoryData world)
        {
            _actionId = world.getNewId();
            _parentPlotFragmentId = parentPlotFragmentId;
        }


        public string AuthorGoalName
        {
            get {


                //Find the parent plot fragment using its stored id, then use that plot fragment to look up
                //the parent author goal id, which is then used to get the actual AuthorGoal object with its name (whew!)
                return StoryWorldDataProvider.getStoryData().findAuthorGoalById(
                            StoryWorldDataProvider.getStoryData().findPlotFragmentById(
                                ParentPlotFragmentId)
                            .ParentAuthorGoalId)
                       .Name;
        
            
            
            }
        }

        public UInt64 Id
        {
            get { return _actionId; }
            set { _actionId = value; }
        }
        public UInt64 ParentPlotFragmentId
        {

            get { return _parentPlotFragmentId; }
            set { _parentPlotFragmentId = value; }
        }

        //Return ordered list of bound variable names
        virtual public List<Trait> getBoundPrimitiveVariables(TraitDataType type, bool allTypes)
        {
            //By default, nothing is bound
            return new List<Trait>();
        }


        virtual public void checkAndUpdateDependences(List<Trait> previouslyBoundVars, StoryData world)
        {
            throw new NotImplementedException();
        }

        virtual public string Description
        {

            get { return "Unknown Story Action"; }
        }

        #region ICloneable Members

        virtual public Object Clone()
        {
            Action newClone = (Action)MemberwiseClone();
            newClone.Id = StoryWorldDataProvider.getStoryData().getNewId();
            return newClone;
        }

        #endregion
 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    [Serializable()]
    public class ActionDeleteEntity : Action
    {
        //In the case of ABL, "delete" means to remove the WME from working memory,
        //instead of actually deleting the entity from the local behavior context
        private string _varNameForDeletion;
        private UInt64 _entityTypeId; //The only character type id, only environment type id,
                                        // or one of the plot point type Ids

        public ActionDeleteEntity(UInt64 parentPlotFragmentId, string varName, UInt64 typeId, StoryData world) :
            base(parentPlotFragmentId, world)
        {
            _varNameForDeletion = varName;
            _entityTypeId = typeId;
        }

        public string VariableName
        {
            get { return _varNameForDeletion; }
            set { _varNameForDeletion = value; }
        }

        public UInt64 TypeId
        {
            get { return _entityTypeId; }
            set { _entityTypeId = value; }
        }

        override public string Description
        {


            get
            {
                string typeString = "";
                if(_entityTypeId == StoryWorldDataProvider.getStoryData().CharTypeId)
                {
                    typeString = "Character";

                }
                else if (_entityTypeId == StoryWorldDataProvider.getStoryData().EnvTypeId)
                {

                    typeString = "Environment";
                }
                else
                {
                    PlotPointType currType = StoryWorldDataProvider.getStoryData().findPlotPointTypeById(_entityTypeId);
                    if(currType != null)
                    {
                        typeString = currType.Description;
                    }
                    else
                    {
                        typeString = "*UNKNOWN PLOT POINT TYPE*";
                    }

                }
                string desc = "Delete " + typeString + " variable \"" + _varNameForDeletion + "\"";
                return desc;
            }
        }

        override public void checkAndUpdateDependences(List<Trait> previouslyBoundVars, StoryData world)
        {
            PlotFragment parentFrag = world.findPlotFragmentById(_parentPlotFragmentId);
            if (parentFrag == null)
            {
                throw new Exception("Delete Object Action not have parent Plot Fragment");
            }

            //check for variables bound to object references
      
 
            List<string> names = null;
            if (_entityTypeId == world.CharTypeId)
            {
                names = parentFrag.getPreviouslyBoundCharacterVarNames(this);
                

            }
            else if (_entityTypeId == world.EnvTypeId)
            {

                names = parentFrag.getPreviouslyBoundEnvironmentVarNames(this);
            }
            else
            {
                PlotPointType currType = world.findPlotPointTypeById(_entityTypeId);
                if (currType != null)
                {
                    names = parentFrag.getPreviouslyBoundPlotPointTypeVarNames(currType, this);

                }

            }
            if (!(names.Contains(_varNameForDeletion)))
            {
                    throw new Exception("Delete Object Action in Plot Fragment \"" +
                            parentFrag.Name + "\" refers to variable \"" + _varNameForDeletion +
                            "\", \nwhich has a different type or does not exist in any previous Author Goal parameters, precondition statements, or actions .");
            }


            //check for any previous deletions of this variable - that would be BAD because another
            //deletion would cause this to fail in ABL
            foreach(Action act in parentFrag.Actions)
            {
                if(act is ActionDeleteEntity)
                {
                    if( (act != this) &&
                        (((ActionDeleteEntity)act).VariableName == _varNameForDeletion)
                      )
                    {

                        throw new Exception("The Plot Fragment \"" +
                                parentFrag.Name + "\" deletes the object saved in the variable \"" + _varNameForDeletion +
                                "\" more than once, which is not allowed. .");
                    }
                }
            }

            return;
        }

  

        #region ICloneable Members

        override public Object Clone()
        {
            return null;
            //DO NOT CLONE THIS - can cause ABL errors if other actions have already
            //deleted the same WME
        }

        #endregion
    }
}

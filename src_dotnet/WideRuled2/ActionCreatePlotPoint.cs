using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    [Serializable()]
    public class ActionCreatePlotPoint : ActionCreateObject
    {

        private PlotPoint _newPP;


        public ActionCreatePlotPoint(UInt64 parentPlotFragmentId, PlotPointType type, string varName, StoryData world) :
            base(parentPlotFragmentId, varName, world)
        {
            _newPP = new PlotPoint(type, world);
        }

        public PlotPoint NewPlotPoint
        {
            get {return _newPP;}
            set {_newPP = value;}
        }

        override public string Description
        {


            get
            {
                string ppTypeName = "*UNKNOWN PLOT POINT TYPE*";
                PlotPointType ppType = StoryWorldDataProvider.getStoryData().findPlotPointTypeById(_newPP.TypeId);
                if(ppType != null)
                {
                    ppTypeName = ppType.Description;
                }
                

                string desc = "Create new " +
                    ppTypeName +
                    ", saved as variable \"" +
                    _variableName + "\"";
                return desc;
            }
        }

        override public void checkAndUpdateDependences(List<Trait> previouslyBoundVars, StoryData world)
        {
            PlotFragment parentFrag = world.findPlotFragmentById(_parentPlotFragmentId);
            if (parentFrag == null)
            {
                throw new Exception("Create plot point action does not have parent Plot Fragment");
            }
            

            if(world.findPlotPointTypeById(_newPP.TypeId) == null)
            {
                throw new Exception("Create Plot Point Action in Plot Fragment \"" + parentFrag.Name + "\" " +
                    "uses a Plot Point Type which no longer exists.");
            }
        }

        #region ICloneable Members

        override  public Object Clone()
        {
            //TODO: make cloning work properly for duplication of entire plot fragments and author goals
            //- add a new clone method or something to 
            //stop the creation of new unique names/variable names
            ActionCreatePlotPoint newClone = (ActionCreatePlotPoint)MemberwiseClone();
            newClone.Id = StoryWorldDataProvider.getStoryData().getNewId();
            newClone.NewPlotPoint = (PlotPoint)_newPP.Clone();

            //Give the new var a unique name, making sure the new name is actually unique.
            //If name is not unique, keep increment suffix number until it is

            string newName = newClone.VariableName;
            int cloneCount = 1;
            PlotFragment parentFrag = StoryWorldDataProvider.getStoryData().findPlotFragmentById(_parentPlotFragmentId);


            Action nullAction = null;
            List<string> prevVars = parentFrag.getAllPreviouslyBoundVariableNames(nullAction, true);


            while (prevVars.Contains(newName))
            {
                newName = newClone.VariableName + cloneCount.ToString();
                cloneCount++;
            }

            newClone.VariableName = newName;


 

            return newClone;
        }

        #endregion
        

    }
}

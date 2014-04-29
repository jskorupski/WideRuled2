using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    [Serializable()]
    public class ActionCreateObject : Action
    {

        protected string _variableName;

        public ActionCreateObject(UInt64 parentPlotFragmentId, string varName, StoryData world) :
            base(parentPlotFragmentId, world)
        {
            _variableName = varName;
        }

        public string VariableName
        {
            get { return _variableName; }
            set { _variableName = value; }
        }

 

        //Create object actions for charcters and environments do not reference any variables, 
        //only the "edit object" action can do that
        //The exception is the plot point creator, which can have an error if the plot point type is
        //missing
        override public void checkAndUpdateDependences(List<Trait> previouslyBoundVars, StoryData world)
        {
            PlotFragment parentFrag = world.findPlotFragmentById(_parentPlotFragmentId);
            if (parentFrag == null)
            {
                throw new Exception("Create object action does not have parent Plot Fragment");
            }
            return;
        }


 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    [Serializable()]
    public class ActionCreateCharacter : ActionCreateObject
    {

        
        private Character _newEntity;


        public ActionCreateCharacter(UInt64 parentPlotFragmentId, string charName, string varName, StoryData world) :
            base(parentPlotFragmentId, varName, world)
        {
            _newEntity = new Character(charName, world.CharTypeId, world);
        }

        public Character NewCharacter
        {
            get {return _newEntity;}
            set {_newEntity = value;}
        }

        override public string Description
        {


            get
            {
                string desc = "Create new Character \"" +
                    _newEntity.Name +
                    "\", saved as variable \"" +
                    _variableName + "\"";

                return desc;

            }
        }



        #region ICloneable Members

        override public Object Clone()
        {
            //TODO: make cloning work properly for duplication of entire plot fragments and author goals
            //- add a new clone method or something to 
            //stop the creation of new unique names/variable names
            ActionCreateCharacter newClone = (ActionCreateCharacter)MemberwiseClone();
            newClone.Id = StoryWorldDataProvider.getStoryData().getNewId();
            newClone.NewCharacter = (Character)_newEntity.Clone();

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WideRuled2
{
    [Serializable()]
    public class ActionTextOutput : Action
    {

        [NonSerialized()] private static Regex _VariableMatcher;

        private string _textOutput;

        public ActionTextOutput(UInt64 parentPlotFragmentId, string textOutput, StoryData world) :
            base(parentPlotFragmentId, world)
        {
            _textOutput = textOutput;
            //if(_VariableMatcher == null)
            //{

                //Matches anything between two outermost "<" and ">" symbols, in that order,
                //which could also mean other "<" or ">" symbols. This allows variables in the Wide Ruled
                //interface to have any type of text in them (even though ABL will need them reformatted anyway).
               // _VariableMatcher = new Regex("(?:[^<]*<)[^<]*(?=>(?!>))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            //}
        }
        
        public string TextOutput
        {
            get { return _textOutput; }
            set { _textOutput = value; }
        }

        override public void checkAndUpdateDependences(List<Trait> previouslyBoundVars, StoryData world)
        {
            PlotFragment parentFrag = world.findPlotFragmentById(_parentPlotFragmentId);
            if (parentFrag == null)
            {
                throw new Exception("Text Output Action does not have parent Plot Fragment");
            }


            //Get all variable references from text

            //Check variable references from previous plot fragment preconditions and actions
            if (_VariableMatcher == null)
            {

                //Matches anything between two outermost "<" and ">" symbols, in that order,
                //which could also mean other "<" or ">" symbols. This allows variables in the Wide Ruled
                //interface to have any type of text in them (even though ABL will need them reformatted anyway).
               // _VariableMatcher = new Regex("(?:[^<]*<)[^<]*(?=>(?!>))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                _VariableMatcher = new Regex("(?:[^<])[^<]*(?=>(?!>))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }

            MatchCollection varMatches = _VariableMatcher.Matches(_textOutput);
            foreach (Match varMatch in varMatches)
            {
                
                //Don't need to check var types. All primitive types can be converted to strings.
                bool foundIt = false;
                foreach (Trait traitItem in previouslyBoundVars)
                {

                    if (traitItem.Name == varMatch.Value)
                    {
                        foundIt = true;
                        break;
                    }
                        
                }

                if (!foundIt)
                {
                    throw new Exception("Text Output Action in Plot Fragment \"" +
                        parentFrag.Name + "\" refers to variable \"" + varMatch.Value +
                        "\", \nwhich does not exist in any previous Author Goal parameters, precondition statements, or actions .");
                }
                
                
            }

        }
        public List<string> getOrderedVariableNameList()
        {
           List<string> orderedMatches = new List<string>();
           MatchCollection varMatches = _VariableMatcher.Matches(_textOutput);
           foreach (Match varMatch in varMatches)
           {
               orderedMatches.Add(varMatch.Value);
           }
           return orderedMatches;
        }

        override public string Description 
        {
            get {

                StringBuilder cleanedStringBuilder = new StringBuilder();
                char[] charArray = _textOutput.ToCharArray();
                //check for numeric first 
                foreach (char item in charArray)
                {
                    char newChar = item;
                    if (item == '\n')
                    {  
                    }
                    else if (item == '\r')
                    {
                    }
                    else if (item == '"')
                    { 
                    }
                    else if (item == '\t')
                    {       
                    }
                    else if (item == '\\')
                    {  
                    }
                    else
                    {
                        cleanedStringBuilder.Append(newChar.ToString());
                    }
                }

                return "Display Text: \"" + cleanedStringBuilder.ToString().Trim() + "\"";
            }
        }

        #region ICloneable Members

        override public Object Clone()
        {
            ActionTextOutput newClone = (ActionTextOutput)MemberwiseClone();
            newClone.Id = StoryWorldDataProvider.getStoryData().getNewId();
            newClone.TextOutput = _textOutput;
            return newClone;
        }

        #endregion
    }
}

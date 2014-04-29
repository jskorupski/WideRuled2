using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WideRuled2
{

    public enum CalculationOperation {Add, Subtract, Multiply, Divide};

    [Serializable()]
    public class ActionCalculation : Action
    {


        private Trait _resultVar;
        private CalculationOperation _op;

        private Parameter _paramLeft;
        private Parameter _paramRight;


        public ActionCalculation(UInt64 parentPlotFragmentId, string varName, StoryData world) :
            base(parentPlotFragmentId, world)
        {
            _resultVar = new Trait(varName, TraitDataType.Number, world.getNewId(), world);
            _paramLeft = new Parameter("paramLeft", TraitDataType.Number, false, world);
            _paramRight = new Parameter("paramRight", TraitDataType.Number, false, world);
            _op = CalculationOperation.Add;
        }

        public static List<string> getCalculationOperationsList()
        {
            List<string> opStrings = new List<string>();
            opStrings.Add(EnumCalculationOperationToString(CalculationOperation.Add));
            opStrings.Add(EnumCalculationOperationToString(CalculationOperation.Subtract));
            opStrings.Add(EnumCalculationOperationToString(CalculationOperation.Multiply));
            opStrings.Add(EnumCalculationOperationToString(CalculationOperation.Divide));
            return opStrings;
        }

        public CalculationOperation CalculationOp
        {
            get { return _op; }
            set { _op = value; }
        }
        public static string EnumCalculationOperationToString(CalculationOperation op)
        {
            switch (op)
            {
                case CalculationOperation.Add:
                    return "+";
      
                case CalculationOperation.Divide:
                    return "/";
       
                case CalculationOperation.Multiply:
                    return "*";
  
                case CalculationOperation.Subtract:
                    return "-";
    
            }
            return "";

        }


        public Parameter ParamLeft
        {
            get { return _paramLeft; }
            set { _paramLeft = value; }
        }

        public Parameter ParamRight
        {
            get { return _paramRight; }
            set { _paramRight = value; }
        }

        public string ResultVarName
        {
            get { return _resultVar.Name; }
            set { _resultVar.Name = value; }
        }

        public Trait Result
        {
            get { return _resultVar; }
            set { _resultVar = value; }
        }

        override public string Description
        {

            get
            {
                return "Calculate: " + _resultVar.Name + " = " + _paramLeft.LiteralValueOrBoundVarName +
                    " " + EnumCalculationOperationToString(_op) + " " + _paramRight.LiteralValueOrBoundVarName;
               

            }
        }

        //Return ordered list of bound variable names
        override public List<Trait> getBoundPrimitiveVariables(TraitDataType type, bool allTypes)
        {
          
            if(allTypes || (type == TraitDataType.Number))
            {
                List<Trait> varResultList = new List<Trait>();
                varResultList.Add(_resultVar);
                return varResultList;
            }
            else
            {
                return new List<Trait>();
            }

        }


        override public void checkAndUpdateDependences(List<Trait> previouslyBoundVars, StoryData world)
        {
            PlotFragment parentFrag = world.findPlotFragmentById(_parentPlotFragmentId);
            if (parentFrag == null)
            {
                throw new Exception("Calculation Action not have parent Plot Fragment");
            }


            if (ParamLeft.ValueIsBoundToVariable)
            {
             
               
                bool foundVar = false;
                foreach (Trait traitItem in previouslyBoundVars)
                {
                    if (
                        (traitItem.Name == (string)ParamLeft.LiteralValueOrBoundVarName) &&
                        (traitItem.Type == ParamLeft.Type)
                        )
                    {
                        foundVar = true;
                        break;
                    }
 
                }
                if (!foundVar)
                {
                    throw new Exception("Calculation Action in Plot Fragment \"" +
                        parentFrag.Name + "\" refers to variable \"" + (string)ParamLeft.LiteralValueOrBoundVarName +
                       "\", \nwhich has a different type or does not exist in any previous Author Goal parameters, precondition statements, or actions .");
                }
    
            }

            if (ParamRight.ValueIsBoundToVariable)
            {


                bool foundVar = false;
                foreach (Trait traitItem in previouslyBoundVars)
                {
                    if (
                        (traitItem.Name == (string)ParamRight.LiteralValueOrBoundVarName) &&
                        (traitItem.Type == ParamRight.Type)
                        )
                    {
                        foundVar = true;
                        break;
                    }

                }
                if (!foundVar)
                {
                    throw new Exception("Calculation Action in Plot Fragment \"" +
                        parentFrag.Name + "\" refers to variable \"" + (string)ParamRight.LiteralValueOrBoundVarName +
                       "\", \nwhich has a different type or does not exist in any previous Author Goal parameters, precondition statements, or actions .");
                }

            }

            previouslyBoundVars.Add(_resultVar);

        }

        #region ICloneable Members

        override public Object Clone()
        {
            //TODO: make cloning work properly for duplication of entire plot fragments and author goals
            //- add a new clone method or something to 
            //stop the creation of new unique names/variable names
            ActionCalculation newClone = (ActionCalculation)MemberwiseClone();
            newClone.Id = StoryWorldDataProvider.getStoryData().getNewId();
            newClone.Result = (Trait)_resultVar.Clone();
            newClone.ParamLeft = (Parameter)_paramLeft.Clone();
            newClone.ParamRight = (Parameter)_paramRight.Clone();


            //Give the new var a unique name, making sure the new name is actually unique.
            //If name is not unique, keep increment suffix number until it is
            string newName = newClone.ResultVarName;
            int cloneCount = 1;
            PlotFragment parentFrag =  StoryWorldDataProvider.getStoryData().findPlotFragmentById(_parentPlotFragmentId);

            Action nullAction = null;
            List<string> prevVars = parentFrag.getAllPreviouslyBoundVariableNames(nullAction, true);


            while (prevVars.Contains(newName))
            {
                newName = newClone.ResultVarName + cloneCount.ToString();
                cloneCount++;
            }

            newClone.ResultVarName = newName;


            return newClone;

        }

        #endregion
    }
}

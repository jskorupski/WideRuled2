using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace WideRuled2
{


    public class AblCodeGenerator
    {

        private const string MAIN_PRIORITY = "500";
        private const string INTERACTIVE_PRIORITY = "750";
        private const string UNDO_PRIORITY = "1000";
        private const string SHUTDOWN_PRIORITY = "3000";
        private const string LONGEXECUTE_PRIORITY = "4000";
        private const string FAILURE_PRIORITY = "5000";
        private const string SUBGOALCOUNT_PRIORITY = "6000";
        private const string SAVERESTORE_PRIORITY = "7000";

        private const string REL_TARGET_PREFIX = "rel_targetname_";
        private const string REL_STRENTH_PREFIX = "rel_strength_";

        //private const string LITERAL_NEWLINE = @"\n";

        private Hashtable _typeIdToTraitsRels;

        private Hashtable _plotPointTypeNames;
        private Hashtable _goalNames;
        List<string> _interactivityDaemonNames;

        private IndentingWriter _tw;
        private StoryData _story;

       // static Random random;

        public AblCodeGenerator(StoryData story)
        {
           // random = new Random();
            _typeIdToTraitsRels = new Hashtable(); //maps entity type id's to HashSet<String> => name mappings for traits
            _story = story;
            _goalNames = new Hashtable();
            _plotPointTypeNames = new Hashtable();
            _interactivityDaemonNames = new List<string>();
            _tw = new IndentingWriter();
        }

        public static string CommaSeparatedNamesWithInnerSpaces(List<Parameter> inputList)
        {

            StringBuilder newString = new StringBuilder();

            int totalItems = inputList.Count;
            int count = 0;
            foreach (Parameter item in inputList)
            {
                count++;
                newString.Append(item.Name);
                if (count != totalItems)
                {
                    newString.Append(", ");
                }
            }
            return newString.ToString();
        }


        public static string CommaSeparatedValuesWithInnerSpaces(List<Parameter> inputList)
        {

            StringBuilder newString = new StringBuilder();

            int totalItems = inputList.Count;
            int count = 0;
            foreach (Parameter item in inputList)
            {
                count++;

                newString.Append(item.LiteralValueOrBoundVarName);
                if (count != totalItems)
                {
                    newString.Append(", ");
                }
            }
            return newString.ToString();
        }

        public static string CommaSeparatedJavaValuesWithInnerSpaces(List<Parameter> inputList)
        {

            StringBuilder newString = new StringBuilder();

            int totalItems = inputList.Count;
            int count = 0;
            foreach (Parameter item in inputList)
            {
                count++;

                newString.Append(generateJavaValueString(item));
                if (count != totalItems)
                {
                    newString.Append(", ");
                }
            }
            return newString.ToString();
        }
       
      

     
        private static string getSetterHeaderFromFieldName(string input)
        {
            return "set" + (input.ToUpper().ToCharArray())[0].ToString() + input.Substring(1);
        }

        private static string getGetterHeaderFromFieldName(string input)
        {
            return "get" + (input.ToUpper().ToCharArray())[0].ToString() + input.Substring(1);
        }

        private static string generateJavaValueString(Parameter input)
        {
          switch(input.Type)
          {
              case TraitDataType.TrueFalse:
                  bool val = (bool)input.LiteralValueOrBoundVarName;
                  if (val) { return "true"; } else { return "false"; }
                  break;
              case TraitDataType.Number:
                  return ((double)input.LiteralValueOrBoundVarName).ToString() + "f";
              case TraitDataType.Text:
                  return generateValidLiteralString((string)input.LiteralValueOrBoundVarName);
          }

          return "";
        }

        private static string generateJavaValueString(Trait input)
        {
            switch (input.Type)
            {
                case TraitDataType.TrueFalse:
                    bool val = (bool)input.Value;
                    if (val) { return "true"; } else { return "false"; }
                    break;
                case TraitDataType.Number:
                    return ((double)input.Value).ToString() + "f";
                case TraitDataType.Text:
                    return generateValidLiteralString((string)input.Value);
            }

            return "";
        }

        public static string generateValidLiteralString(string input)
        {
            if (input == "") { return "\"\""; }

            return "\"" + generateValidString(input) + "\"";

        }

        public static string generateValidString(string input)
        {

            StringBuilder newStringB = new StringBuilder();
            char[] charArray = input.ToCharArray();
            //check for numeric first 
            foreach (char item in charArray)
            {
                char newChar = item;
                if(item == '\n')
                {
                    newStringB.Append(@"\n");
                }
                else if (item == '\r')
                {

                }
                else if (item == '"')
                {
                    newStringB.Append(@"\""");
                }
                else if (item == '\t')
                {
                    newStringB.Append(@"\t");
                }
                else if (item ==  '\\')
                {
                    newStringB.Append(@"\\");
                }
                else
                {
                    newStringB.Append(newChar.ToString());
                }
            }

            return newStringB.ToString();
        }

        public static string generateUniqueValidVariableName(string inputName, Hashtable prevNames)
        {
           
          //  int randInt =  random.Next(0, 10000); 
           
            string resultName = inputName;

            if(resultName == "")
            {
                resultName = getUniqueName("", prevNames);
            }
            else
            {
             
                    string newName = "_";
                    char[] charArray = inputName.ToCharArray();
                    //check for numeric first 
                    foreach(char item in charArray)
                    {
                        if(isLetter(item) || isNumber(item))
                        {
                            newName += item.ToString();
                        }
                    }
                    if(newName == "_")
                    {
                        resultName = getUniqueName(newName, prevNames);
                    }
                    else if (isNumber(newName.ToCharArray()[0]))
                    {
                        resultName = getUniqueName(newName, prevNames);
                    }
                    else
                    {
                        resultName = getUniqueName(newName, prevNames);
                    }
                
            }
            prevNames.Add(inputName, resultName);
            return resultName;
        }

        private static bool isLetter(char input)
        {
            return ((input >= 'A' && input <= 'Z') || (input >= 'a' && input <= 'z'));
        }

        private static bool isNumber(char input)
        {
            return (input >= '0' && input <= '9');
        }

        private static string getUniqueName(string inputName, Hashtable prevNames)
        {
            int count = 0;
            string testName = inputName;
            while(prevNames.ContainsValue(testName))
            {
                testName = inputName + count.ToString();
            }

            return testName;
        }

        private static string dataTypeToJavaTypeString(TraitDataType type)
        {
            switch(type)
            {
                case TraitDataType.Number:
                    return "float";
                case TraitDataType.Text:
                    return "String";
                case TraitDataType.TrueFalse:
                    return "boolean";
            }
            return "";
        }

        private static string getRelTargetAttributeName(string baseName)
        {
            return REL_TARGET_PREFIX + baseName;
        }

        private static string getRelStrengthAttributeName(string baseName)
        {
            return REL_STRENTH_PREFIX + baseName;
        }


        public void BuildCode()
        {

            generateCode();
            Utilities.writeToFile(_tw, App.CurrentWorkingDirectory + "\\abl\\ablcode\\WR_GenAgent.abl");
            //Compile files
            CompileFiles();

        }


        private void generateCode()
        {

            _tw.WriteLine("//------------------------------------------------------------------------//");
            _tw.WriteLine("// ............................Wide Ruled 2 ....,.........................//");
            _tw.WriteLine("// ...............ABL-based Story Generator Source Code...................//");
            _tw.WriteLine("// ......This file is created automatically - please don't modify!....... //");
            _tw.WriteLine("//------------------------------------------------------------------------//");
            _tw.WriteLine();
            _tw.WriteLine();
            _tw.WriteLine();
            _tw.WriteLine();
            //package and imports
            _tw.WriteLine("package javacode;");
            _tw.WriteLine();
            _tw.WriteLine("import java.lang.*;");
            _tw.WriteLine("import java.util.*;");
            _tw.WriteLine("import java.util.ArrayList;");
            _tw.WriteLine("import abl.runtime.*;");
            _tw.WriteLine("import wm.*;");
            _tw.WriteLine();
            _tw.WriteLine();
            _tw.WriteLine();
            _tw.WriteLine();
            declareBehavingEntity();

        }
       

        private void declareBehavingEntity()
        {
            //behaving entity header
            _tw.WriteLine("behaving_entity WR_GenAgent {");
            _tw.Indent();
                _tw.WriteLine();
                _tw.WriteLine();
                //Actions, properties, globals
                actionsWMEsPropertiesGlobals();
                _tw.WriteLine();
                _tw.WriteLine();
                //Story entity types
                declareCharacterWME();
                _tw.WriteLine();
                declareEnvironmentWME();
                _tw.WriteLine();
                declarePlotPointTypeWMEs();

                _tw.WriteLine();
                declareStoryStructure();

                _tw.WriteLine();
                declareStoryStateShuffleBehavior();

                _tw.WriteLine();
                declareInteractivityParentBehavior();

                _tw.WriteLine();
                declareInteractivityIndividualBehaviors();

                _tw.WriteLine();
                declareDaemons();

                _tw.WriteLine();
                _tw.WriteLine();
                //start initial tree
                _tw.WriteLine("initial_tree {");
                _tw.Indent();

                    _tw.WriteLine();
                    intialTreeStartGoal();
                    _tw.WriteLine();
                   // initialTreeInteractivityDaemons();
                   // _tw.WriteLine();
                    intialTreeDeamons();
                    _tw.WriteLine();

                    //end initial tree
                    _tw.OutDent();
                _tw.WriteLine("}");


                //end behaving entity
                _tw.OutDent();
            _tw.WriteLine("}");
        }


        private void actionsWMEsPropertiesGlobals()
        {
            _tw.WriteLine("//------------------------------------------------------------------------//");
            _tw.WriteLine("//--------------Register actions from WR_Bot proxybot-----------------------");
            _tw.WriteLine("//------------------------------------------------------------------------//");
            _tw.WriteLine("register action print(String, ArrayList, Boolean) with WR_PrintAction;");
            _tw.WriteLine("register action print(String, ArrayList) with WR_PrintAction;");
            _tw.WriteLine("register action print(String, Boolean) with WR_PrintAction;");
            _tw.WriteLine("register action abort() with WR_AbortAction;");
            _tw.WriteLine();
            _tw.WriteLine();
            _tw.WriteLine("//------------------------------------------------------------------------//");
            _tw.WriteLine("//--------------------------Register sensors------------------------------//");
            _tw.WriteLine("//------------------------------------------------------------------------//");
            _tw.WriteLine();
            _tw.WriteLine("register wme WR_InteractionWME with WR_InteractionSensor;");
            _tw.WriteLine("register wme WR_UndoWME with WR_UndoSensor;");
            _tw.WriteLine("register wme WR_AbortWME with WR_AbortSensor;");

            _tw.WriteLine();
            _tw.WriteLine();
            _tw.WriteLine("//properties");
            _tw.WriteLine("property boolean storyTop;");
            _tw.WriteLine("property boolean isCounted;");
            _tw.WriteLine();
            _tw.WriteLine();
            _tw.WriteLine("//World state stack");
            _tw.WriteLine("Stack gWorldStateStack = new Stack();");
            _tw.WriteLine();
            _tw.WriteLine("//Global execution counter");
            _tw.WriteLine("int gExecCount = 0;");
            _tw.WriteLine("//Max execution count to catch infinite looping");
            _tw.WriteLine("int gMaxExecCount = 20000;");
            _tw.WriteLine("//True/False boolean objects for parameter passing");
            _tw.WriteLine("Boolean TrueObject = new Boolean(true);");
            _tw.WriteLine("Boolean FalseObject = new Boolean(false);");
            _tw.WriteLine("//Temp data list for WME order randomization hack");
            _tw.WriteLine("ArrayList tempTotalWMEState = new ArrayList();");
        }

        private void declareCharacterWME()
        {
            List<Character> charList = Utilities.getGlobalCharacterList(_story);
            if (charList.Count == 0)
            {
                //no characters are ever used
                return;
            }
            _tw.WriteLine("wme WR_CharacterWME {");
            _tw.Indent();

            Hashtable newTraitRelnames = new Hashtable();
            newTraitRelnames.Add("id", "id");

            _tw.WriteLine("int id;");

            foreach (Trait tr in charList[0].Traits)
            {
                _tw.WriteLine(
                    dataTypeToJavaTypeString(tr.Type) +
                    " " + generateUniqueValidVariableName(tr.Name, newTraitRelnames) +
                    ";");

            }



            foreach (Relationship rel in charList[0].Relationships)
            {
                string newBaseName = generateUniqueValidVariableName(rel.Name, newTraitRelnames);
                _tw.WriteLine(
                   "String" +
                   " " +
                   getRelTargetAttributeName(newBaseName) +
                   ";");

                _tw.WriteLine(
                  dataTypeToJavaTypeString(TraitDataType.Number) +
                  " " +
                  getRelStrengthAttributeName(newBaseName) +
                  ";");


            }


            _typeIdToTraitsRels.Add(_story.CharTypeId, newTraitRelnames);

            _tw.OutDent();
            _tw.WriteLine("}");

        }


        private void declareEnvironmentWME()
        {
           
            List<Environment> envList = Utilities.getGlobalEnvironmentList(_story);
            if(envList.Count == 0)
            {
                //no characters are ever used
                return;
            }
            _tw.WriteLine("wme WR_EnvironmentWME {");
            _tw.Indent();

            Hashtable newTraitRelnames = new Hashtable();
            newTraitRelnames.Add("id", "id");

            _tw.WriteLine("int id;");

            foreach (Trait tr in envList[0].Traits)
            {
                _tw.WriteLine(
                    dataTypeToJavaTypeString(tr.Type) + 
                    " " + generateUniqueValidVariableName(tr.Name, newTraitRelnames) +
                    ";");
    
            }

     
       
            foreach (Relationship rel in envList[0].Relationships)
            {
                string newBaseName = generateUniqueValidVariableName(rel.Name, newTraitRelnames);
                _tw.WriteLine(
                   "String " +
                   " " + 
                   getRelTargetAttributeName(newBaseName) +
                   ";");

                _tw.WriteLine(
                  dataTypeToJavaTypeString(TraitDataType.Number) +
                  " " + 
                  getRelStrengthAttributeName(newBaseName) +
                  ";");

    
            }


            _typeIdToTraitsRels.Add(_story.EnvTypeId, newTraitRelnames);

            _tw.OutDent();
            _tw.WriteLine("}");
        }


        private void declarePlotPointTypeWMEs()
        {
            Hashtable prevPPTypeNames = new Hashtable();
            foreach (PlotPointType ppType in _story.PlotPointTypes)
            {
                string newPPTypeBaseName = generateUniqueValidVariableName(ppType.Name, prevPPTypeNames);
                //prevPPTypeNames.Add(ppType.Name, newPPTypeBaseName);
                
                
                string finalPPTypeName = "WR_PlotPoint_" + newPPTypeBaseName + "WME";
                _plotPointTypeNames.Add(ppType.Id, finalPPTypeName);

                _tw.WriteLine("wme " + finalPPTypeName + " {");
                _tw.Indent();

                Hashtable newPPTypeTraitNames = new Hashtable();
                newPPTypeTraitNames.Add("id", "id");

                _tw.WriteLine("int id;");

                foreach(Trait tr in ppType.Traits)
                {
                    _tw.WriteLine(
                        dataTypeToJavaTypeString(tr.Type) + 
                        " " + generateUniqueValidVariableName(tr.Name, newPPTypeTraitNames) +
                        ";");
                }

                _typeIdToTraitsRels.Add(ppType.Id, newPPTypeTraitNames);

                _tw.OutDent();
                _tw.WriteLine("}");

            }
        }

        private void declareStoryStructure()
        {
            _tw.WriteLine();
            declareAllGoals();
            _tw.WriteLine();
            declareStartGoal();
            _tw.WriteLine();
            declareStartGoalFailureBackup();
            _tw.WriteLine();
        }
        private void declareAllGoals()
        {
            //First cache all goal name conversions so subgoaling can refer to these even when we haven't yet generated
            // the ABL code for that author goal

            Hashtable prevGoalNames = new Hashtable();
            foreach (AuthorGoal goal in _story.AuthorGoals)
            {
                string goalName = generateUniqueValidVariableName(goal.Name, prevGoalNames);
                _goalNames.Add(goal.Name, goalName);
            }

            
            foreach(AuthorGoal goal in _story.AuthorGoals)
            {

               
                string goalName = (string)_goalNames[goal.Name];
               
               /* //Fix to get around ABL != bug
                List<string> parameterVars = new List<string>();
                */
                Hashtable paramNames = new Hashtable();
                string paramString = "(";
                int totalParamCount = goal.Parameters.Count;
                int currParamCount = 0;
                foreach (Parameter param in goal.Parameters)
                {
                    generateUniqueValidVariableName(param.ParameterName, paramNames);
                    paramString += dataTypeToJavaTypeString(param.Type) + " " + (string)paramNames[param.ParameterName];

                    /*//Fix for abl != bug
                    parameterVars.Add((string)paramNames[param.ParameterName]);
                    */
                    currParamCount++;
                    if(currParamCount != totalParamCount)
                    {
                        paramString += ", ";
                    }
                }
                paramString += ")";

                foreach (PlotFragment frag in goal.PlotFragments)
                {
                    Hashtable fragScopeVars = (Hashtable)paramNames.Clone();
                    
                    _tw.WriteLine("//Plot Fragment Name: " + generateValidString(frag.Name));
                    _tw.WriteLine("sequential behavior " + goalName + " " + paramString + " {");
                    _tw.Indent();
                    if (frag.PrecStatements.Count != 0)
                    {
                        List<PreconditionStatement> constraintConditionStatements = new List<PreconditionStatement>();
                        _tw.WriteLine("//Begin preconditions");
                        _tw.WriteLine("precondition {");
                        _tw.Indent();

                        /* != counter used for != parameter comparison bug fix for ABL */
                        //int currNotEqualsTempVarCount = 0;
                        foreach (PreconditionStatement precond in frag.PrecStatements)
                        {
                           /* //Fix to get around ABL != bug
                            List<Trait> parameterToNotEqualsVarMappings = new List<Trait>();
                            */
                            //Keep running list of preconditions that have constraint conditions
                            if(precond.getAlwaysTrueConstraints().Count > 0)
                            {
                                constraintConditionStatements.Add(precond);
                            }

                            string precondStatement = "";
                            if (!precond.ObjectExists)
                            {
                                precondStatement += "!";

                            }

                            if (precond.SaveMatchedObject)
                            {
                                string savedObjName = generateUniqueValidVariableName(precond.SaveObjectVariableName, fragScopeVars);
                                precondStatement += savedObjName + " = (";
                            }
                            else { precondStatement += "("; }

                            Hashtable traitRelTable;

                            if (precond is PreconditionStatementCharacter)
                            {
                                precondStatement += "WR_CharacterWME ";
                                traitRelTable = (Hashtable)_typeIdToTraitsRels[_story.CharTypeId];
                            }
                            else if (precond is PreconditionStatementEnvironment)
                            {
                                precondStatement += "WR_EnvironmentWME ";
                                traitRelTable = (Hashtable)_typeIdToTraitsRels[_story.EnvTypeId];
                            }
                            else
                            {
                                UInt64 ppTypeId = _story.findPlotPointTypeById(((PreconditionStatementPlotPoint)precond).MatchTypeId).Id;
                                traitRelTable = (Hashtable)_typeIdToTraitsRels[ppTypeId];
                                precondStatement += (string)_plotPointTypeNames[ppTypeId] + " ";
                            }
                            
                            foreach (Constraint cons in precond.Constraints)
                            {
                                if (!(cons.ConstraintType == ConstraintComparisonType.None))
                                {
                                    if (cons is TraitConstraint)
                                    {
                                        precondStatement += traitRelTable[cons.ComparisonValue.Name];

                                        //Fix for != string abl bug with parameters
                                        /*if(  
                                            (cons.ConstraintType == ConstraintComparisonType.NotEquals) && 
                                            (cons.ComparisonValue.ValueIsBoundToVariable) &&
                                            (cons.ComparisonValue.Type == TraitDataType.Text) &&
                                            (parameterVars.Contains((string)fragScopeVars[cons.ComparisonValue.LiteralValueOrBoundVarName])))
                                        {
                                            string newVarName = generateUniqueValidVariableName("tempNotEqualsText" + currNotEqualsTempVarCount.ToString(), fragScopeVars);
                                            currNotEqualsTempVarCount++;
                                            precondStatement += "::" + newVarName;
                                            Trait newMapping = new Trait((string)fragScopeVars[cons.ComparisonValue.LiteralValueOrBoundVarName], TraitDataType.Text, newVarName, 0, _story);
                                            parameterToNotEqualsVarMappings.Add(newMapping);
                                        }
                                        else
                                        {*/

                                            precondStatement += " " + Constraint.ConstraintComparisonTypeToString(cons.ConstraintType) + " ";
                                            if (!cons.ComparisonValue.ValueIsBoundToVariable) { precondStatement += generateJavaValueString(cons.ComparisonValue); }
                                            else
                                            {
                                                precondStatement += fragScopeVars[cons.ComparisonValue.LiteralValueOrBoundVarName];
                                            }
                                        /*}*/
                                      

                                    }
                                    else //relationship constraint
                                    {
                                        if (((RelationshipConstraint)cons).TargetNameMode)
                                        {
                                            
                                            precondStatement += getRelTargetAttributeName((string)traitRelTable[cons.ComparisonValue.Name]);
                                        }
                                        else
                                        {
                                            precondStatement += getRelStrengthAttributeName((string)traitRelTable[cons.ComparisonValue.Name]);
                                        }

                                         //Fix for != string abl bug with parameters
                                      /*  if( 
                                            (cons.ConstraintType == ConstraintComparisonType.NotEquals) && 
                                            (cons.ComparisonValue.ValueIsBoundToVariable) &&
                                            (((RelationshipConstraint)cons).TargetNameMode) &&
                                            (parameterVars.Contains((string)fragScopeVars[cons.ComparisonValue.LiteralValueOrBoundVarName])))
                                        {
                                            string newVarName = generateUniqueValidVariableName("tempNotEqualsText" + currNotEqualsTempVarCount.ToString(), fragScopeVars);
                                            currNotEqualsTempVarCount++;
                                            precondStatement += "::" + newVarName;
                                            Trait newMapping = new Trait((string)fragScopeVars[cons.ComparisonValue.LiteralValueOrBoundVarName], TraitDataType.Text, newVarName, 0, _story);
                                            parameterToNotEqualsVarMappings.Add(newMapping);
                                        }
                                        else
                                        {*/
                                            precondStatement += " " + Constraint.ConstraintComparisonTypeToString(cons.ConstraintType) + " ";
                                            if (!cons.ComparisonValue.ValueIsBoundToVariable) { precondStatement += generateJavaValueString(cons.ComparisonValue); }
                                            else
                                            {

                                                precondStatement += fragScopeVars[cons.ComparisonValue.LiteralValueOrBoundVarName];
                                            }

                                        /*}*/
                                        
                                    }
                                }

                                if (cons.ContainsSavedVariable)
                                {
                                    if (cons is TraitConstraint)
                                    {
                                        precondStatement += " " + traitRelTable[cons.ComparisonValue.Name] +
                                            "::" +
                                            generateUniqueValidVariableName(cons.SavedVariable.Name, fragScopeVars);
                                    }
                                    else if (cons is RelationshipConstraint)
                                    {
                                        precondStatement += " ";
                                        if (((RelationshipConstraint)cons).TargetNameMode)
                                        {
                                            precondStatement += getRelTargetAttributeName((string)traitRelTable[cons.ComparisonValue.Name]);
                                        }
                                        else
                                        {
                                            precondStatement += getRelStrengthAttributeName((string)traitRelTable[cons.ComparisonValue.Name]);
                                        }

                                        precondStatement += "::" +
                                            generateUniqueValidVariableName(cons.SavedVariable.Name, fragScopeVars);
                                    }

                                }
                               
                                

                                precondStatement += " ";
                            }
                            precondStatement += ")";

                            
                            _tw.WriteLine(precondStatement);

                           /* //Fix for ABL != param text bug
                            foreach(Trait tr in parameterToNotEqualsVarMappings)
                            {
                                _tw.WriteLine("(!" + tr.Name + ".equals(" + ((string)tr.Value) + "))");
                            }*/
                        }

                       

                        _tw.OutDent();
                        _tw.WriteLine("}");
                        _tw.WriteLine("//End preconditions");


                        if(constraintConditionStatements.Count > 0)
                        {
                            _tw.WriteLine("//Begin context conditions");
                            _tw.WriteLine("context_condition {");
                            _tw.Indent();
                            foreach (PreconditionStatement precond in constraintConditionStatements)
                            {
                                List<Trait> parameterToNotEqualsVarMappings = new List<Trait>();

                                string precondStatement = "";
                                if (!precond.ObjectExists)
                                {
                                    precondStatement += "!";

                                }

                           
                                precondStatement += "(";

                                Hashtable traitRelTable;

                                if (precond is PreconditionStatementCharacter)
                                {
                                    precondStatement += "WR_CharacterWME ";
                                    traitRelTable = (Hashtable)_typeIdToTraitsRels[_story.CharTypeId];
                                }
                                else if (precond is PreconditionStatementEnvironment)
                                {
                                    precondStatement += "WR_EnvironmentWME ";
                                    traitRelTable = (Hashtable)_typeIdToTraitsRels[_story.EnvTypeId];
                                }
                                else
                                {
                                    UInt64 ppTypeId = _story.findPlotPointTypeById(((PreconditionStatementPlotPoint)precond).MatchTypeId).Id;
                                    traitRelTable = (Hashtable)_typeIdToTraitsRels[ppTypeId];
                                    precondStatement += (string)_plotPointTypeNames[ppTypeId] + " ";
                                }

                                foreach (Constraint cons in precond.Constraints)
                                {
                                    if (cons.MustAlwaysBeTrue)
                                    {
                                        if (cons is TraitConstraint)
                                        {

                                            precondStatement += traitRelTable[cons.ComparisonValue.Name];

                                            //Fix for != string abl bug with parameters
                                           /* if (
                                                (cons.ConstraintType == ConstraintComparisonType.NotEquals) &&
                                                (cons.ComparisonValue.ValueIsBoundToVariable) &&
                                                (cons.ComparisonValue.Type == TraitDataType.Text) &&
                                                (parameterVars.Contains((string)fragScopeVars[cons.ComparisonValue.LiteralValueOrBoundVarName])))
                                            {
                                                string newVarName = generateUniqueValidVariableName("tempNotEqualsText" + currNotEqualsTempVarCount.ToString(), fragScopeVars);
                                                currNotEqualsTempVarCount++;
                                                precondStatement += "::" + newVarName;
                                                Trait newMapping = new Trait((string)fragScopeVars[cons.ComparisonValue.LiteralValueOrBoundVarName], TraitDataType.Text, newVarName, 0, _story);
                                                parameterToNotEqualsVarMappings.Add(newMapping);
                                            }
                                            else
                                            {*/

                                                precondStatement += " " + Constraint.ConstraintComparisonTypeToString(cons.ConstraintType) + " ";
                                                if (!cons.ComparisonValue.ValueIsBoundToVariable) { precondStatement += generateJavaValueString(cons.ComparisonValue); }
                                                else
                                                {
                                                    precondStatement += fragScopeVars[cons.ComparisonValue.LiteralValueOrBoundVarName];
                                                }
                                           /* }*/



                                            //precondStatement += traitRelTable[cons.ComparisonValue.Name] + " ";
                                            //precondStatement += Constraint.ConstraintComparisonTypeToString(cons.ConstraintType) + " ";
                                            //if (!cons.ComparisonValue.ValueIsBoundToVariable) { precondStatement += generateJavaValueString(cons.ComparisonValue); }
                                            //else
                                            //{
                                            //    precondStatement += fragScopeVars[cons.ComparisonValue.LiteralValueOrBoundVarName];
                                            //}
                                        }
                                        else //relationship constraint
                                        {
                                            if (((RelationshipConstraint)cons).TargetNameMode)
                                            {
                                                precondStatement += getRelTargetAttributeName((string)traitRelTable[cons.ComparisonValue.Name]) + " ";
                                            }
                                            else
                                            {
                                                precondStatement += getRelStrengthAttributeName((string)traitRelTable[cons.ComparisonValue.Name]) + " ";
                                            }

                                            //Fix for != string abl bug with parameters
                                            /*if (
                                                (cons.ConstraintType == ConstraintComparisonType.NotEquals) &&
                                                (cons.ComparisonValue.ValueIsBoundToVariable) &&
                                                (((RelationshipConstraint)cons).TargetNameMode) &&
                                                (parameterVars.Contains((string)fragScopeVars[cons.ComparisonValue.LiteralValueOrBoundVarName])))
                                            {
                                                string newVarName = generateUniqueValidVariableName("tempNotEqualsText" + currNotEqualsTempVarCount.ToString(), fragScopeVars);
                                                currNotEqualsTempVarCount++;
                                                precondStatement += "::" + newVarName;
                                                Trait newMapping = new Trait((string)fragScopeVars[cons.ComparisonValue.LiteralValueOrBoundVarName], TraitDataType.Text, newVarName, 0, _story);
                                                parameterToNotEqualsVarMappings.Add(newMapping);
                                            }
                                            else
                                            {*/
                                                precondStatement += " " + Constraint.ConstraintComparisonTypeToString(cons.ConstraintType) + " ";
                                                if (!cons.ComparisonValue.ValueIsBoundToVariable) { precondStatement += generateJavaValueString(cons.ComparisonValue); }
                                                else
                                                {

                                                    precondStatement += fragScopeVars[cons.ComparisonValue.LiteralValueOrBoundVarName];
                                                }

                                            /*}*/


                                            //precondStatement += Constraint.ConstraintComparisonTypeToString(cons.ConstraintType) + " ";

                                            //if (!cons.ComparisonValue.ValueIsBoundToVariable) { precondStatement += generateJavaValueString(cons.ComparisonValue); }
                                            //else
                                            //{
                                            //    precondStatement += fragScopeVars[cons.ComparisonValue.LiteralValueOrBoundVarName];
                                            //}

                                        }
                                    }
                                    // Don't need to do any variable saving

                                    precondStatement += " ";
                                }
                                precondStatement += ")";
                                _tw.WriteLine(precondStatement);
                                /*//Fix for ABL != param text bug
                                foreach (Trait tr in parameterToNotEqualsVarMappings)
                                {
                                    _tw.WriteLine("(!" + tr.Name + ".equals(" + ((string)tr.Value) + "))");
                                }*/

                            }
                            _tw.OutDent();
                            _tw.WriteLine("}");
                            _tw.WriteLine("//End context conditions");
                        }
                       

                    } //End preconditions and constraint conditions sections

                    //Constraint conditions


                        _tw.WriteLine();
                        _tw.WriteLine("//Begin local variables");
                        //Local Variables for this plot fragment that must be declared at the top of the behavior
                        if(frag.containsTextOutputAction())
                        {
                            _tw.WriteLine("ArrayList fragLocalPrimitives;");
                        }
                        List<string> calcVarNames = frag.getCalculationVariables();
                        foreach(string name in calcVarNames)
                        {
                            _tw.WriteLine(dataTypeToJavaTypeString(TraitDataType.Number) + " " + generateUniqueValidVariableName(name, fragScopeVars) + ";");
                        }
                        List<string> charVarNames = frag.getNewCharacterActionVarNames(_story.CharTypeId);
                        foreach(string name in charVarNames)
                        {
                             _tw.WriteLine("WR_CharacterWME " + generateUniqueValidVariableName(name, fragScopeVars) + ";");
                        }
                        List<string> envVarNames = frag.getNewEnvironmentActionVarNames(_story.EnvTypeId);
                        foreach(string name in envVarNames)
                        {
                             _tw.WriteLine("WR_EnvironmentWME " + generateUniqueValidVariableName(name, fragScopeVars) + ";");
                        }
                        foreach (PlotPointType ppT in _story.PlotPointTypes)
                        {   
                            List<string> ppTypeVarNames = frag.getNewPlotPointActionVarNames(ppT);
                            foreach(string name in ppTypeVarNames)
                            {
                                _tw.WriteLine(_plotPointTypeNames[ppT.Id] + " " + generateUniqueValidVariableName(name, fragScopeVars) + ";");
                            }
                        }
                        _tw.WriteLine("//End local variables");
                        _tw.WriteLine();
                        _tw.WriteLine("//Begin behavior actions ");
                        //first action is to initialize the hashtable for later printing use
                        if(frag.containsTextOutputAction())
                        {
                            _tw.WriteLine("mental_act { fragLocalPrimitives = new ArrayList(); }");
                    
                        }
                        _tw.WriteLine("//World state management");
                        _tw.WriteLine("with (priority " + SAVERESTORE_PRIORITY + ") subgoal UTIL_SaveWorldState();");
                        if(_story.ShowDebugMessages)
                        {
                            _tw.WriteLine("//Debugging message");
                            _tw.WriteLine("act print (\"**** Begun fragment: " + generateValidString(frag.Name) + ", in goal: " + generateValidString(goal.Name) + " ****\", TrueObject);");
                        }
                       
                        _tw.WriteLine();    
                        foreach (Action fragAct in frag.Actions)
                        {

                            if(fragAct is ActionTextOutput)
                            {
                                ActionTextOutput currAct = (ActionTextOutput)fragAct;
                           
                                List<Trait> localVars = frag.getPreviouslyBoundPrimitiveVariables(TraitDataType.Number, true, fragAct);

                                List<string> textVars = currAct.getOrderedVariableNameList();

                            
                                _tw.WriteLine("mental_act {");
                                _tw.Indent();
                                _tw.WriteLine("fragLocalPrimitives.clear();");
                                foreach (string tVar in textVars)
                                {
                                    if(fragScopeVars[tVar] != null)
                                    {
                                        _tw.WriteLine("fragLocalPrimitives.add(" + fragScopeVars[tVar]+");");
                                        

                                    }
                                    
                                }
                                _tw.OutDent();
                                _tw.WriteLine("}");
                                _tw.WriteLine("act print (" + generateValidLiteralString(currAct.TextOutput) + ", fragLocalPrimitives);");
                                _tw.WriteLine();
                   
                            }
                            else if (fragAct is ActionCalculation)
                            {
                                ActionCalculation currAct = (ActionCalculation)fragAct;
                                _tw.WriteLine("mental_act {");
                                _tw.Indent();
                                string calcString = (string)fragScopeVars[currAct.ResultVarName] + " = ";
                                if(currAct.ParamLeft.ValueIsBoundToVariable)
                                {
                                    calcString += fragScopeVars[currAct.ParamLeft.LiteralValueOrBoundVarName];
                                }
                                else { calcString += generateJavaValueString(currAct.ParamLeft); }
                                calcString += " " + ActionCalculation.EnumCalculationOperationToString(currAct.CalculationOp) + " ";
                                if(currAct.ParamRight.ValueIsBoundToVariable)
                                {
                                    calcString += fragScopeVars[currAct.ParamRight.LiteralValueOrBoundVarName];
                                }
                                else { calcString += generateJavaValueString(currAct.ParamRight); }
                                calcString +=";";
                                _tw.WriteLine(calcString);
                                _tw.OutDent();
                                _tw.WriteLine("}");
                            }
                            else if (fragAct is ActionCreateCharacter)
                            {
                                _tw.WriteLine();
                                _tw.WriteLine("mental_act {");
                                _tw.Indent();
                                ActionCreateCharacter currAct = (ActionCreateCharacter)fragAct;
                                declareNewCharacter(false, currAct.NewCharacter, (string)fragScopeVars[currAct.VariableName]);
                                _tw.WriteLine("BehavingEntity.getBehavingEntity().addWME(" + (string)fragScopeVars[currAct.VariableName] + ");");
                                _tw.OutDent();
                                _tw.WriteLine("}");
                                _tw.WriteLine();
                            }
                            else if (fragAct is ActionCreateEnvironment)
                            {
                                
                                _tw.WriteLine();
                                _tw.WriteLine("mental_act {");
                                _tw.Indent();
                                ActionCreateEnvironment currAct = (ActionCreateEnvironment)fragAct;
                                declareNewEnvironment(false, currAct.NewEnvironment, (string)fragScopeVars[currAct.VariableName]);
                                _tw.WriteLine("BehavingEntity.getBehavingEntity().addWME(" + (string)fragScopeVars[currAct.VariableName] + ");");
                                _tw.OutDent();
                                _tw.WriteLine("}");
                                _tw.WriteLine();
                            }
                            else if (fragAct is ActionCreatePlotPoint)
                            {
                                _tw.WriteLine();
                                _tw.WriteLine("mental_act {");
                                _tw.Indent();
                                ActionCreatePlotPoint currAct = (ActionCreatePlotPoint)fragAct;
                                declareNewPlotPoint(false, currAct.NewPlotPoint, _story.findPlotPointTypeById(currAct.NewPlotPoint.TypeId), (string)fragScopeVars[currAct.VariableName]);
                                _tw.WriteLine("BehavingEntity.getBehavingEntity().addWME(" + (string)fragScopeVars[currAct.VariableName] + ");");
                                _tw.OutDent();
                                _tw.WriteLine("}");
                                _tw.WriteLine();
                            }
                            else if (fragAct is ActionDeleteEntity)
                            {
                                ActionDeleteEntity currAct = (ActionDeleteEntity)fragAct;

                                _tw.WriteLine();
                                _tw.WriteLine("mental_act {");
                                _tw.Indent();
                                _tw.WriteLine("BehavingEntity.getBehavingEntity().deleteWME(" + (string)fragScopeVars[currAct.VariableName] + ");");
                                _tw.OutDent();
                                _tw.WriteLine("}");
                                _tw.WriteLine();
                            }
                            else if (fragAct is ActionEditObject)
                            {
                                ActionEditObject currAct = (ActionEditObject)fragAct;

                                Hashtable traitRelTable = (Hashtable)_typeIdToTraitsRels[currAct.ObjectTypeId];          
                        

                                _tw.WriteLine();
                                _tw.WriteLine("mental_act {");
                                _tw.Indent();
                                string setString = "";
                                if(currAct.Mode == ObjectEditingMode.Trait)
                                {
                                    setString = fragScopeVars[currAct.VariableObjectName] + "." + 
                                        getSetterHeaderFromFieldName((string)traitRelTable[currAct.NewValue.Name]) + "(";
                                    if(currAct.NewValue.ValueIsBoundToVariable)
                                    {
                                        setString += (string)fragScopeVars[currAct.NewValue.LiteralValueOrBoundVarName];
                                    }
                                    else 
                                    {
                                        setString += generateJavaValueString(currAct.NewValue);
                                    }
                                    setString += ");";
                                }
                                else if (currAct.Mode == ObjectEditingMode.RelationshipTarget)
                                {
                                    setString = (string)fragScopeVars[currAct.VariableObjectName] + "." + 
                                        getSetterHeaderFromFieldName(getRelTargetAttributeName((string)traitRelTable[currAct.NewValue.Name])) + "(";
                                    setString += (string)fragScopeVars[(string)currAct.NewTarget.LiteralValueOrBoundVarName] + "." +
                                        getGetterHeaderFromFieldName((string)traitRelTable["Name"]) + "());";
                                }
                                else if (currAct.Mode == ObjectEditingMode.RelationshipStrength)
                                {
                                    setString = (string)fragScopeVars[currAct.VariableObjectName] + "." + 
                                        getSetterHeaderFromFieldName(getRelStrengthAttributeName((string)traitRelTable[currAct.NewValue.Name])) + "(";

                                     if(currAct.NewValue.ValueIsBoundToVariable)
                                     {
                                         setString += (string)fragScopeVars[currAct.NewValue.LiteralValueOrBoundVarName];
                                     }
                                     else 
                                     {
                                         setString += generateJavaValueString(currAct.NewValue);
                                     }
                                     setString += ");";
                                }

                                _tw.WriteLine(setString);
                                _tw.OutDent();
                                _tw.WriteLine("}");
                                _tw.WriteLine();

                            }
                            else if (fragAct is ActionSubgoal)
                            {
                                _tw.WriteLine("/****************************************** Begin Subgoal ******************************************/");
                                _tw.WriteLine("//First, check for interactivity");
                                _tw.WriteLine("with (priority " + INTERACTIVE_PRIORITY + ") subgoal WR_InteractivityCheck();");
                                _tw.WriteLine("//Next, manually shuffle Story State WME's for random selection");
                                _tw.WriteLine("with (priority " + MAIN_PRIORITY + ", property isCounted false) subgoal WR_StoryStateShuffle();");
                                _tw.WriteLine("//Do actual subgoal call now");
                                ActionSubgoal currAct = (ActionSubgoal)fragAct;
                                string headerCallString = "with (priority " + MAIN_PRIORITY + ", property isCounted false, post) subgoal ";
                                string subgoalName = (string)_goalNames[_story.findAuthorGoalById(currAct.SubGoalId).Name];
                                string subgoalLine = headerCallString + subgoalName + " (";
                                int totalParamsCount = currAct.ParametersToPass.Count;
                                int currParam = 0;
                                foreach(Parameter paramToPass in currAct.ParametersToPass)
                                {
                                    if(paramToPass.ValueIsBoundToVariable)
                                    {
                                        subgoalLine += (string)fragScopeVars[paramToPass.LiteralValueOrBoundVarName];
                                    }
                                    else
                                    {
                                        subgoalLine += generateJavaValueString(paramToPass);

                                    }
                                    currParam++;
                                    if(currParam != totalParamsCount)
                                    {
                                        subgoalLine += ", ";
                                    }
                                }

                                subgoalLine += ");";
                                _tw.WriteLine(subgoalLine);
                                _tw.WriteLine("/****************************************** End Subgoal ******************************************/");
                                _tw.WriteLine();
                            }

                        }
                        _tw.WriteLine();
                        _tw.WriteLine("//World state management");
                        
                        _tw.WriteLine("with (priority " + SAVERESTORE_PRIORITY + ") subgoal UTIL_RemoveWorldState();");
                        if (_story.ShowDebugMessages)
                        {
                            _tw.WriteLine("//Debugging message");
                            _tw.WriteLine("act print (\"**** Ended fragment: " + generateValidString(frag.Name) + ", in goal: " + generateValidString(goal.Name) + " ****\", TrueObject);");
                        }
                       
                  
                          
    
                        _tw.OutDent();
                    _tw.WriteLine("}");


                }
                _tw.WriteLine();
                


            }
        }


        private void declareNewCharacter(bool prependType, Character ch, string saveAsName)
        {
            int currentCount = 0;

            int paramCount = 1 + ch.Traits.Count +  (2 * ch.Relationships.Count);

            if(prependType)
            {
                _tw.WriteLine("WR_CharacterWME " + saveAsName + " = new WR_CharacterWME ( ");
            }
            else
            {
                _tw.WriteLine(saveAsName + " = new WR_CharacterWME ( ");
            }
            
            _tw.Indent();

            _tw.WriteLine(ch.Id.ToString() + ", ");
            currentCount++;

            string attribString;
            foreach(Trait tr in ch.Traits)
            {
                attribString = generateJavaValueString(tr);
                currentCount++;
                if(currentCount != paramCount)
                {
                    attribString += ", ";
                }
                _tw.WriteLine(attribString);
            }

            string targetStr;

            foreach(Relationship rel in ch.Relationships)
            {
                targetStr = (string)rel.ToCharacter.Name;
                //if (rel.ToCharacter.Id == StoryData.NullCharacterId)
               // {
                //    targetStr = "";
                //}


                _tw.WriteLine(generateValidLiteralString(targetStr) + ", ");

                currentCount += 2;
                if(currentCount != paramCount)
                {
                    _tw.WriteLine(rel.Strength.ToString() + ", ");
                }
                else
                {
                    _tw.WriteLine(rel.Strength.ToString());
                }
            }
            _tw.OutDent();
            _tw.WriteLine(");");
        }

        private void declareNewEnvironment(bool prependType, Environment env, string saveAsName)
        {
            int currentCount = 0;

            int paramCount = 1 + env.Traits.Count + (2 * env.Relationships.Count);
            if(prependType)
            {
                _tw.WriteLine("WR_EnvironmentWME " + saveAsName + " = new WR_EnvironmentWME ( ");
            }
            else
            {
                _tw.WriteLine(saveAsName + " = new WR_EnvironmentWME ( ");
            }
            
            _tw.Indent();

            _tw.WriteLine(env.Id.ToString() + ", ");
            currentCount++;

            string attribString;
            foreach (Trait tr in env.Traits)
            {
                attribString = generateJavaValueString(tr);
                currentCount++;
                if (currentCount != paramCount)
                {
                    attribString += ", ";
                }
                _tw.WriteLine(attribString);
            }

            string targetStr;

            foreach (Relationship rel in env.Relationships)
            {
                targetStr = (string)rel.ToEnvironment.Name;
                //if(rel.ToEnvironment.Id == StoryData.NullEnvironmentId)
               // {
                //    targetStr = "";
               // }

                _tw.WriteLine(generateValidLiteralString(targetStr) + ", ");

                currentCount += 2;
                if (currentCount != paramCount)
                {
                    _tw.WriteLine(rel.Strength.ToString() + ", ");
                }
                else
                {
                    _tw.WriteLine(rel.Strength.ToString());
                }
            }
            _tw.OutDent();
            _tw.WriteLine(");");
        }

        private void declareNewPlotPoint(bool prependType, PlotPoint pp, PlotPointType ppType, string saveAsName)
        {
            int currentCount = 0;

            string typeName = (string)_plotPointTypeNames[ppType.Id];

            int paramCount = 1 + ppType.Traits.Count;

            if(prependType)
            {
                _tw.WriteLine(typeName + " " + saveAsName + " = new " + typeName + " ( ");
            }
            else
            {
                _tw.WriteLine(saveAsName + " = new " + typeName + " ( ");
            }
            
            _tw.Indent();

            _tw.WriteLine(pp.Id.ToString() + ", ");
            currentCount++;

            string attribString;
            foreach (Trait tr in pp.Traits)
            {
                attribString = generateJavaValueString(tr);
                currentCount++;
                if (currentCount != paramCount)
                {
                    attribString += ", ";
                }
                _tw.WriteLine(attribString);
            }

        
            _tw.OutDent();
            _tw.WriteLine(");");
        }


        private void declareStartGoal()
        {
            string finishMessage = "\"Story Completed!\"";
            _tw.WriteLine("sequential behavior UTIL_GenerateStory() {");
            _tw.Indent();
		        _tw.WriteLine("specificity 2;"); 
        		
                //Create initial characters and environments
		        _tw.WriteLine("mental_act {");
                _tw.Indent();
                    _tw.WriteLine();
                    _tw.WriteLine("tempTotalWMEState.clear();");

                  
                    int entityCount = 0;
                    string baseEntityVarName = "newEntity";
                    foreach (Character ch in _story.Characters)
                    {
                        string newName = baseEntityVarName + entityCount.ToString();
                        declareNewCharacter(true, ch, newName);
                        _tw.WriteLine("tempTotalWMEState.add(" + newName + ");");
                        entityCount++;
                        _tw.WriteLine();
                    }
                    _tw.WriteLine();
                    foreach (Environment env in _story.Environments)
                    {
                        string newName = baseEntityVarName + entityCount.ToString();
                        declareNewEnvironment(true, env, newName);
                        _tw.WriteLine("tempTotalWMEState.add(" + newName + ");");
                        entityCount++;
                        _tw.WriteLine();
                    }

                    _tw.WriteLine();
                    _tw.WriteLine("Collections.shuffle(tempTotalWMEState);");
                    _tw.WriteLine("BehavingEntity.getBehavingEntity().getWorkingMemory().addWMEs(tempTotalWMEState);");
            		_tw.OutDent();
                _tw.WriteLine("}");
                _tw.WriteLine();

                //Start goal declaration
                string startGoalName = (string)_goalNames[_story.findAuthorGoalById(_story.StartGoalId).Name];
                _tw.WriteLine("with (priority " + MAIN_PRIORITY + ", property storyTop true, property isCounted false, post) subgoal " + startGoalName + "();");
                _tw.WriteLine();

                //with (priority 500, property storyTop true, post) subgoal startTest();
                _tw.WriteLine("act print(" + finishMessage + ", TrueObject);");
                _tw.WriteLine("act abort();");
                _tw.OutDent();
            _tw.WriteLine("}");

		
        }

        private void declareStartGoalFailureBackup()
        {
            string failureMessage = "\"Your story has failed :(\"";
            _tw.WriteLine("//Duplicate main goal to exit if failure occurs");
            _tw.WriteLine("sequential behavior UTIL_GenerateStory() {");
            _tw.Indent();
            _tw.WriteLine("specificity 1;");
            _tw.WriteLine("act print (" + failureMessage + ", TrueObject);");
            _tw.WriteLine("act abort();");
            _tw.OutDent();
            _tw.WriteLine("}");
        }



        private void declareDaemons()
        {
            declareStaticDaemons();
            _tw.WriteLine();
            declareSaveWorldStateDaemon();
            _tw.WriteLine();
            declareRestoreWorldStateDaemon();
            _tw.WriteLine();
            //declareInteractivityDaemons();
        }

        private void declareStaticDaemons()
        {
            _tw.WriteLine(Utilities.getFileContents(App.CurrentWorkingDirectory + "\\abl\\static\\abl\\WR_Daemons.abl"));
        }

        private void declareSaveWorldStateDaemon()
        {
            _tw.WriteLine("sequential behavior UTIL_SaveWorldState() {");
            _tw.Indent();
                _tw.WriteLine("mental_act {");
                _tw.Indent();

          

                    _tw.WriteLine("ArrayList wmeState =  new ArrayList();");
                    if(Utilities.getGlobalCharacterList(_story).Count > 0)
                    {
                        _tw.WriteLine("wmeState.addAll(BehavingEntity.getBehavingEntity().lookupWME(\"WR_CharacterWME\"));");
                    }

                    if(Utilities.getGlobalEnvironmentList(_story).Count > 0)
                    {

                        _tw.WriteLine("wmeState.addAll(BehavingEntity.getBehavingEntity().lookupWME(\"WR_EnvironmentWME\"));");
                    }

                    foreach (PlotPointType ppType in _story.PlotPointTypes)
                    {
                        _tw.WriteLine("wmeState.addAll(BehavingEntity.getBehavingEntity().lookupWME(\"" + (string)_plotPointTypeNames[ppType.Id] + "\"));");
                    }
		            _tw.WriteLine("gWorldStateStack.push(wmeState);");
                    _tw.OutDent();
                _tw.WriteLine("}");
                _tw.OutDent();
	        _tw.WriteLine("}");
	    }

        
        private void declareRestoreWorldStateDaemon()
        {
            _tw.WriteLine("sequential behavior UTIL_RestoreWorldState() {");
            _tw.Indent();
                _tw.WriteLine("mental_act {");
                _tw.Indent();

                    _tw.WriteLine("Object[] wmeArray;");
                    _tw.WriteLine("List tempWMEList;");
                    _tw.WriteLine("tempTotalWMEState.clear();");

                    if (Utilities.getGlobalCharacterList(_story).Count > 0)
                    {
                        _tw.WriteLine("tempWMEList = BehavingEntity.getBehavingEntity().lookupWME(\"WR_CharacterWME\");");
                        _tw.WriteLine("tempTotalWMEState.addAll(tempWMEList);");
                        _tw.WriteLine("wmeArray = tempWMEList.toArray();");
                        _tw.WriteLine("for(int i = 0; i < wmeArray.length; i++) {");
                        _tw.Indent();
                        _tw.WriteLine("BehavingEntity.getBehavingEntity().deleteWME((WR_CharacterWME)wmeArray[i]);");
                        _tw.OutDent();
                        _tw.WriteLine("}");
                        _tw.WriteLine();
                    }
                    
                    if (Utilities.getGlobalEnvironmentList(_story).Count > 0)
                    {

                        _tw.WriteLine("tempWMEList = BehavingEntity.getBehavingEntity().lookupWME(\"WR_EnvironmentWME\");");
                        _tw.WriteLine("tempTotalWMEState.addAll(tempWMEList);");
                        _tw.WriteLine("wmeArray = tempWMEList.toArray();");
                        _tw.WriteLine("for(int i = 0; i < wmeArray.length; i++) {");
                        _tw.Indent();
                        _tw.WriteLine("BehavingEntity.getBehavingEntity().deleteWME((WR_EnvironmentWME)wmeArray[i]);");
                        _tw.OutDent();
                        _tw.WriteLine("}");
                        _tw.WriteLine();
                    }
                    
                    foreach (PlotPointType ppType in _story.PlotPointTypes)
                    {
                        _tw.WriteLine("tempWMEList = BehavingEntity.getBehavingEntity().lookupWME(\"" + (string)_plotPointTypeNames[ppType.Id] + "\");");
                        _tw.WriteLine("tempTotalWMEState.addAll(tempWMEList);");
                        _tw.WriteLine("wmeArray = tempWMEList.toArray();");
                        _tw.WriteLine("for(int i = 0; i < wmeArray.length; i++) {");
                        _tw.Indent();
                        _tw.WriteLine("BehavingEntity.getBehavingEntity().deleteWME((" + (string)_plotPointTypeNames[ppType.Id] + ")wmeArray[i]);");
                        _tw.OutDent();
                        _tw.WriteLine("}");
                        
                    }
                    if(_story.PlotPointTypes.Count > 0)
                    {
                        _tw.WriteLine();
                    }

                    _tw.WriteLine("List restoreList = (List)gWorldStateStack.pop();");
                    _tw.WriteLine("Collections.shuffle(restoreList);");
                    _tw.WriteLine("WorkingMemory w = BehavingEntity.getBehavingEntity().getWorkingMemory();");
                    _tw.WriteLine("w.addWMEs(restoreList);");
                    _tw.OutDent();
                _tw.WriteLine("}");
                _tw.OutDent();
	        _tw.WriteLine("}");
	    }


        private void declareInteractivityParentBehavior()
        {
            _tw.WriteLine("//Parent behavior that calls all checks for sensed interactivity WME's");
            _tw.WriteLine("sequential behavior WR_InteractivityCheck() {");
            _tw.Indent();
            _tw.WriteLine();

            interactivityChecks();
            _tw.WriteLine("succeed_step;");
            _tw.OutDent();
            _tw.WriteLine("}");

        }


        private void interactivityChecks()
        {
            _tw.WriteLine();
            _tw.WriteLine("// Interactivity Checks");
            int interactCount = 0;
            foreach (Interaction interact in _story.Interactions)
            {
               
                _tw.WriteLine("with (priority " + INTERACTIVE_PRIORITY + ", property isCounted false, ignore_failure) subgoal InteractivityCheck_" + interactCount.ToString() + "();");
                interactCount++;
                //_tw.WriteLine("with (priority " + INTERACTIVE_PRIORITY + ", persistent) subgoal DAEMON_Interactive_" + interactCount.ToString() + "();");
            }

        }

        private void declareStoryStateShuffleBehavior()
        {
            _tw.WriteLine("sequential behavior WR_StoryStateShuffle() {");
            _tw.Indent();
                _tw.WriteLine("//Total Hack: Manually shuffle story state so author goals can match randomly to WME's");
                _tw.WriteLine("mental_act {");
                _tw.Indent();

                    _tw.WriteLine("Object[] wmeArray;");
                    _tw.WriteLine("List tempWMEList;");
                    _tw.WriteLine("tempTotalWMEState.clear();");

                    if (Utilities.getGlobalCharacterList(_story).Count > 0)
                    {
                        _tw.WriteLine("tempWMEList = BehavingEntity.getBehavingEntity().lookupWME(\"WR_CharacterWME\");");
                        _tw.WriteLine("tempTotalWMEState.addAll(tempWMEList);");
                        _tw.WriteLine("wmeArray = tempWMEList.toArray();");
                        _tw.WriteLine("for(int i = 0; i < wmeArray.length; i++) {");
                        _tw.Indent();
                        _tw.WriteLine("BehavingEntity.getBehavingEntity().deleteWME((WR_CharacterWME)wmeArray[i]);");
                        _tw.OutDent();
                        _tw.WriteLine("}");
                        _tw.WriteLine();
                    }

                    if (Utilities.getGlobalEnvironmentList(_story).Count > 0)
                    {

                        _tw.WriteLine("tempWMEList = BehavingEntity.getBehavingEntity().lookupWME(\"WR_EnvironmentWME\");");
                        _tw.WriteLine("tempTotalWMEState.addAll(tempWMEList);");
                        _tw.WriteLine("wmeArray = tempWMEList.toArray();");
                        _tw.WriteLine("for(int i = 0; i < wmeArray.length; i++) {");
                        _tw.Indent();
                        _tw.WriteLine("BehavingEntity.getBehavingEntity().deleteWME((WR_EnvironmentWME)wmeArray[i]);");
                        _tw.OutDent();
                        _tw.WriteLine("}");
                        _tw.WriteLine();
                    }

                    foreach (PlotPointType ppType in _story.PlotPointTypes)
                    {
                        _tw.WriteLine("tempWMEList = BehavingEntity.getBehavingEntity().lookupWME(\"" + (string)_plotPointTypeNames[ppType.Id] + "\");");
                        _tw.WriteLine("tempTotalWMEState.addAll(tempWMEList);");
                        _tw.WriteLine("wmeArray = tempWMEList.toArray();");
                        _tw.WriteLine("for(int i = 0; i < wmeArray.length; i++) {");
                        _tw.Indent();
                        _tw.WriteLine("BehavingEntity.getBehavingEntity().deleteWME((" + (string)_plotPointTypeNames[ppType.Id] + ")wmeArray[i]);");
                        _tw.OutDent();
                        _tw.WriteLine("}");

                    }
                    if (_story.PlotPointTypes.Count > 0)
                    {
                        _tw.WriteLine();
                    }

                    _tw.WriteLine("Collections.shuffle(tempTotalWMEState);");
                    _tw.WriteLine("BehavingEntity.getBehavingEntity().getWorkingMemory().addWMEs(tempTotalWMEState);");

                    _tw.OutDent();
                _tw.WriteLine("}");
                _tw.OutDent();
                _tw.WriteLine("}");
        }

        private void declareInteractivityIndividualBehaviors()
        {
            string message = "\"Interaction!\"";
            int interactCount = 0;
            foreach (Interaction interact in _story.Interactions)
            {

                _tw.WriteLine("sequential behavior InteractivityCheck_" + interactCount.ToString() + "() {");
                _tw.Indent();
                    //_tw.WriteLine("WR_InteractionWME w;");
                    _tw.WriteLine("precondition {");
                    _tw.Indent();
                    _tw.WriteLine("w = (WR_InteractionWME id == " + interactCount + ")");
                    _tw.OutDent();
                    _tw.WriteLine("}");
                    //_tw.WriteLine("with( success_test { w = (WR_InteractionWME id == " + interactCount + ") } ) wait;");
                    _tw.WriteLine();
		            _tw.WriteLine("act print (" + message + ", TrueObject);");
                    _tw.WriteLine("mental_act { BehavingEntity.getBehavingEntity().deleteWME(w); }");
                    _tw.WriteLine();
                    _tw.WriteLine("//First, manually shuffle Story State WME's for random selection");
                    _tw.WriteLine("with (priority " + MAIN_PRIORITY + ", property isCounted false) subgoal WR_StoryStateShuffle();");
                    _tw.WriteLine("//Do actual subgoal call now");

		            string withHeader = "with (priority " + INTERACTIVE_PRIORITY + ", property isCounted false, post) subgoal ";
                    string goalName = (string)_goalNames[interact.AuthorGoalName];
                    string parameterPass = " (" + CommaSeparatedJavaValuesWithInnerSpaces(interact.SubgoalAction.ParametersToPass) + ");";
                    _tw.WriteLine(withHeader + goalName + parameterPass);
                    
                    _tw.OutDent();
                _tw.WriteLine("}");
                interactCount++;

            }
        }

  
        private void intialTreeStartGoal()
        {
            _tw.WriteLine();
            _tw.WriteLine("//Wrap start goal in sequential behavior to exit program when finished");
            _tw.WriteLine("with (priority " + MAIN_PRIORITY + ") subgoal UTIL_GenerateStory();");
            _tw.WriteLine();
		
        }


        private void intialTreeDeamons()
        {
            _tw.WriteLine();
            _tw.WriteLine("//Utility Daemons");
            _tw.WriteLine("with (priority " + UNDO_PRIORITY + ", persistent) subgoal DAEMON_UndoDaemon();");

            _tw.WriteLine("with (priority " + SHUTDOWN_PRIORITY + ", persistent) subgoal DAEMON_ShutdownDaemon();");

            _tw.WriteLine("with (priority " + LONGEXECUTE_PRIORITY + ", persistent) subgoal DAEMON_LongExecutionShutdownDaemon();");

            _tw.WriteLine("with (priority " + FAILURE_PRIORITY + ", persistent) subgoal DAEMON_StoryGoalFailureNotification();");

            _tw.WriteLine("with (priority " + SUBGOALCOUNT_PRIORITY + ", persistent) subgoal DAEMON_SubgoalCountDaemon();");
        }


        private void CompileFiles()
        {


            string output = "";
            string currWorkingDir = App.CurrentWorkingDirectory;
            System.Diagnostics.Process proc = new System.Diagnostics.Process();

            //  proc.StartInfo.EnvironmentVariables["path"] = proc.StartInfo.EnvironmentVariables["path"] + ";../jdk/bin/;../jdk/jre/bin/";

            //cleanup previous autgen abl code
            Utilities.deleteFilesAndDirectories(currWorkingDir + "\\abl\\ablcode\\javacode");


            //cleanup abl2java code
            Utilities.deleteFilesAndDirectories(currWorkingDir + "\\abl\\javacode");


            //generate, compile, copy new abl code

            //Generate (using CodeGenerator, put into .\\abl\\ablcode)



            //Copy precompiled references for ABL code
            Utilities.copyAllFiles(currWorkingDir + "\\abl\\static\\bin\\javacode", currWorkingDir + "\\abl\\ablcode\\javacode");


            //Compile
            proc.StartInfo.WorkingDirectory = currWorkingDir + "\\abl\\ablcode";
            proc.StartInfo.FileName = currWorkingDir + "\\abl\\jdk6\\jre\\bin\\java.exe";
            proc.StartInfo.Arguments = "-classpath \"..\\abl.jar;..\\hoj.jar;.\" abl.compiler.Abl *.abl";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            try
            {
                output += proc.StandardOutput.ReadToEnd();
                output += proc.StandardError.ReadToEnd();
            }
            catch (System.Exception ex)
            {
                throw new Exception("Compilation error: " + ex.Message);
            }
            proc.WaitForExit();


            //worker.ReportProgress(1, output + "\n\n\n");
            //Thread.Sleep(5000);


            //Copy abl2java code to java directory
            Utilities.copyAllFiles(currWorkingDir + "\\abl\\ablcode\\javacode", currWorkingDir + "\\abl\\javacode");

            //copy static java ABL support code code over
            Utilities.copyAllFiles(currWorkingDir + "\\abl\\static\\java", currWorkingDir + "\\abl\\javacode");


            //compile java code
            proc.StartInfo.WorkingDirectory = currWorkingDir + "\\abl\\javacode";
            proc.StartInfo.FileName = currWorkingDir + "\\abl\\jdk6\\bin\\javac.exe";
            proc.StartInfo.Arguments = "-classpath \"..\\abl.jar;..\\hoj.jar;.\" *.java";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            try
            {
                output += proc.StandardOutput.ReadToEnd();
                output += proc.StandardError.ReadToEnd();
            }
            catch (System.Exception ex)
            {
                throw new Exception("Compilation error: " + ex.Message);
            }
           // proc.WaitForExit();

            Utilities.writeToFile(output, App.CurrentWorkingDirectory + "\\abl\\UI_log.txt");



        }
    }

}

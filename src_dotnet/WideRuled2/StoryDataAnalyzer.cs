using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace WideRuled2
{
    class StoryDataAnalyzer
    {
        //Stats to gather for each story data file

        private const string FILE_NAME = " FILE_NAME";
        
        private const string STORY_AVG_PLOTFRAGS_PER_GOAL = "STORY_AVG_PLOTFRAGS_PER_GOAL";
        private const string STORY_AVG_SUBGOALS_PER_GOAL = "STORY_AVG_SUBGOALS_PER_GOAL";
        private const string STORY_COUNT_PLOTFRAGS = "STORY_COUNT_PLOTFRAGS";
        private const string STORY_COUNT_GOALS = "STORY_COUNT_GOALS";
        private const string STORY_COUNT_CHARACTERS = "STORY_COUNT_CHARACTERS";
        private const string STORY_COUNT_CHARACTER_TRAITS = "STORY_COUNT_CHARACTER_TRAITS";
        private const string STORY_COUNT_CHARACTER_RELATIONSHIPS = "STORY_COUNT_CHARACTER_RELATIONSHIPS";
        private const string STORY_COUNT_ENVIRONMENTS = "STORY_COUNT_ENVIRONMENTS";
        private const string STORY_COUNT_ENVIRONMENT_TRAITS = "STORY_COUNT_ENVIRONMENT_TRAITS";
        private const string STORY_COUNT_ENVIRONMENT_RELATIONSHIPS = "STORY_COUNT_ENVIRONMENT_RELATIONSHIPS";
        private const string STORY_COUNT_PLOTPOINTS = "STORY_COUNT_PLOTPOINTS";
        private const string STORY_COUNT_PLOTPOINT_TRAITS = "STORY_COUNT_PLOTPOINT_TRAITS";
        private const string STORY_INTERACTIVE_ACTIONS = "STORY_INTERACTIVE_ACTIONS";

        private const string STORY_DEEPEST_GOALFRAG_TREE_PATH = "STORY_DEEPEST_GOALFRAG_TREE_PATH";
        private const string STORY_AVG_DEPTH_GOALFRAG_TREE_PATH = "STORY_AVG_DEPTH_GOALFRAG_TREE_PATH";

        private const string PLOTFRAG_COUNT_PARAMETERS = "PLOTFRAG_COUNT_PARAMETERS";
        private const string PLOTFRAG_AVG_PARAMETERS = "PLOTFRAG_AVG_PARAMETERS";

        private const string PRECOND_COUNT_PRECONDS = "PRECOND_COUNT_PRECONDS";
        private const string PRECOND_AVG_PRECONDS = "PRECOND_AVG_PRECONDS";
        private const string PRECOND_COUNT_CONSTRAINTS = "PRECOND_COUNT_CONSTRAINTS";
        private const string PRECOND_AVG_CONSTRAINTS = "PRECOND_AVG_CONSTRAINTS";
        private const string PRECOND_COUNT_TOTAL_VAR_REFERENCES = "PRECOND_COUNT_TOTAL_VAR_REFERENCES";
        private const string PRECOND_AVG_VAR_REFERENCES_PER_PLOTFRAG = "PRECOND_AVG_VAR_REFERENCES_PER_PLOTFRAG";
        private const string PRECOND_COUNT_TOTAL_VAR_BINDINGS = "PRECOND_COUNT_TOTAL_VAR_BINDINGS";
        private const string PRECOND_AVG_VAR_BINDINGS_PER_PLOTFRAG = "PRECOND_AVG_VAR_BINDINGS_PER_PLOTFRAG";
        private const string PRECOND_COUNT_NEGATION = "PRECOND_COUNT_NEGATION";
        private const string PRECOND_AVG_NEGATION = "PRECOND_AVG_NEGATION";


        private const string ACTION_TOTAL_ACTIONS = "ACTION_TOTAL_ACTIONS";
        private const string ACTION_AVG_ACTIONS = "ACTION_AVG_ACTIONS";
        private const string ACTION_COUNT_VAR_REFERENCES = "ACTION_COUNT_VAR_REFERENCES";
        private const string ACTION_AVG_VAR_REFERENCES = "ACTION_AVG_VAR_REFERENCES";
        private const string ACTION_COUNT_VAR_BINDINGS = "ACTION_COUNT_TOTAL_VAR_BINDINGS";
        private const string ACTION_AVG_VAR_BINDINGS_PER_PLOTFRAG = "ACTION_AVG_VAR_BINDINGS_PER_PLOTFRAG";
        private const string ACTION_COUNT_TEXT_OUTPUTS = "ACTION_COUNT_TEXT_OUTPUTS";
        private const string ACTION_AVG_TEXT_OUTPUTS = "ACTION_AVG_TEXT_OUTPUTS";
        private const string ACTION_AVG_TEXT_OUTPUT_LENGTH = "ACTION_AVG_TEXT_OUTPUT_LENGTH";
        private const string ACTION_AVG_TEXT_OUTPUT_VARS = "ACTION_AVG_TEXT_OUTPUT_VARS";
        private const string ACTION_COUNT_TEXT_OUTPUT_VARS = "ACTION_COUNT_TEXT_OUTPUT_VARS";
        private const string ACTION_COUNT_CHAR_ADDS = "ACTION_COUNT_CHAR_ADDS";
        private const string ACTION_AVG_CHAR_ADDS = "ACTION_AVG_CHAR_ADDS";
        private const string ACTION_COUNT_CHAR_DELETES = "ACTION_COUNT_CHAR_DELETES";
        private const string ACTION_AVG_CHAR_DELETES = "ACTION_AVG_CHAR_DELETES";
        private const string ACTION_COUNT_CHAR_EDITS = "ACTION_COUNT_CHAR_EDITS";
        private const string ACTION_AVG_CHAR_EDITS = "ACTION_AVG_CHAR_EDITS";
        private const string ACTION_COUNT_ENV_ADDS = "ACTION_COUNT_ENV_ADDS";
        private const string ACTION_AVG_ENV_ADDS = "ACTION_AVG_ENV_ADDS";
        private const string ACTION_COUNT_ENV_DELETES = "ACTION_COUNT_ENV_DELETES";
        private const string ACTION_AVG_ENV_DELETES = "ACTION_AVG_ENV_DELETES";
        private const string ACTION_COUNT_ENV_EDITS = "ACTION_COUNT_ENV_EDITS";
        private const string ACTION_AVG_ENV_EDITS = "ACTION_AVG_ENV_EDITS";
        private const string ACTION_COUNT_PP_ADDS = "ACTION_COUNT_PP_ADDS";
        private const string ACTION_AVG_PP_ADDS = "ACTION_AVG_PP_ADDS";
        private const string ACTION_COUNT_PP_DELETES = "ACTION_COUNT_PP_DELETES";
        private const string ACTION_AVG_PP_DELETES = "ACTION_AVG_PP_DELETES";
        private const string ACTION_COUNT_PP_EDITS = "ACTION_COUNT_PP_EDITS";
        private const string ACTION_AVG_PP_EDITS = "ACTION_AVG_PP_EDITS";
        private const string ACTION_COUNT_SUBGOALS = "ACTION_COUNT_SUBGOALS";
        private const string ACTION_AVG_SUBGOALS = "ACTION_AVG_SUBGOALS";
        private const string ACTION_COUNT_CALCS = "ACTION_COUNT_CALCS";
        private const string ACTION_AVG_CALCS = "ACTION_AVG_CALCS";
        private const string ACTION_AVG_ACTIONS_BETWEEN_SUBGOALS = "ACTION_AVG_ACTIONS_BETWEEN_SUBGOALS";



        private const string PLOTFRAG_COUNT_VAR_REFERENCES = "PLOTFRAG_COUNT_VAR_REFERENCES";
        private const string PLOTFRAG_AVG_VAR_REFERENCES = "PLOTFRAG_AVG_VAR_REFERENCES";


        private const string PLOTFRAG_COUNT_VAR_BINDINGS = "PLOTFRAG_COUNT_VAR_BINDINGS";
        private const string PLOTFRAG_AVG_VAR_BINDINGS = "PLOTFRAG_AVG_VAR_BINDINGS";
      

        private const string SAMPLE_SIMILARITY = "SAMPLE_SIMILARITY";


        private const string GRADING_POINTS = "GRADING_POINTS";
        private const string GRADING_COMMENT = "GRADING_COMMENT";
       

        public StoryDataAnalyzer(){}

        public static string analyze(string[] storyDataFiles, string sampleStoryFile) 
        {
            

            List<Hashtable> analysisData = new List<Hashtable>();
            Hashtable sampleAnalysisData = new Hashtable();

            //Analyze sample story data first
            try
            {
                StoryData sampleStoryData = Utilities.DeSerializeStoryDataFromDisk(sampleStoryFile);
                if (sampleStoryData != null)
                {


                    sampleAnalysisData = analyzeSingleStory(sampleStoryFile, sampleStoryData, null);

                }
                else
                {
                    throw new Exception("Could not read sample story file \"" + sampleStoryFile + "\"");

                }

            }
            catch (System.Exception openError)
            {

                string errorText = "Error opening file:\n" + openError.Message;

                throw new Exception(errorText);

            }


            //Analyze story data
            int storyCount = 1;
            foreach(string storyFilename in storyDataFiles)
            {
                if (storyFilename.Equals(""))
                {
                    continue;
                }

                try
                {
                    StoryData currentStoryData = Utilities.DeSerializeStoryDataFromDisk(storyFilename);
                    if (currentStoryData != null)
                    {

                        
                       analysisData.Add(analyzeSingleStory(storyFilename, currentStoryData, sampleAnalysisData));

                    }
                    else
                    {
                        throw new Exception("Could not read file \"" + storyFilename + "\"");

                    }

                }
                catch (System.Exception openError)
                {

                    string errorText = "Error opening file:\n" + openError.Message;
                
                    throw new Exception(errorText);

                }
                storyCount++;
                System.Console.WriteLine("Analyzed Story " + storyCount + ": " + storyFilename);

            }

            return ConvertToCSV(analysisData);
        }

        private static Hashtable analyzeSingleStory(string fileName, StoryData inputStory, Hashtable sampleStoryData)
        {
            Hashtable currentStoryStats = new Hashtable();


            #region FILE_NAME
            currentStoryStats[FILE_NAME] = System.IO.Path.GetFileName(fileName);
            #endregion

            #region STORY_COUNT_PLOTFRAGS
            currentStoryStats[STORY_COUNT_PLOTFRAGS] = 0.0;
            foreach(AuthorGoal g in inputStory.AuthorGoals)
            {
                currentStoryStats[STORY_COUNT_PLOTFRAGS] = (double)currentStoryStats[STORY_COUNT_PLOTFRAGS] + (double)g.PlotFragments.Count;
            }
            
            #endregion

            #region STORY_COUNT_GOALS
            currentStoryStats[STORY_COUNT_GOALS] = (double)inputStory.AuthorGoals.Count;
            #endregion

            #region STORY_AVG_PLOTFRAGS_PER_GOAL

            if((double)currentStoryStats[STORY_COUNT_GOALS] > 0.0 )
            {
                currentStoryStats[STORY_AVG_PLOTFRAGS_PER_GOAL] = (double)currentStoryStats[STORY_COUNT_PLOTFRAGS] / (double)currentStoryStats[STORY_COUNT_GOALS];
            }
            else
            {
                currentStoryStats[STORY_AVG_PLOTFRAGS_PER_GOAL] = 0.0;
            }

                                         
            #endregion


            #region STORY_AVG_SUBGOALS_PER_GOAL

            if ((double)currentStoryStats[STORY_COUNT_GOALS] > 0.0)
            {

                currentStoryStats[STORY_AVG_SUBGOALS_PER_GOAL] = 0.0;

                

                foreach (AuthorGoal g in inputStory.AuthorGoals)
                {
                    foreach (PlotFragment f in g.PlotFragments)
                    {
                        foreach (Action a in f.Actions)
                        {
                            if (a is ActionSubgoal)
                            {
                                currentStoryStats[STORY_AVG_SUBGOALS_PER_GOAL] = (double)currentStoryStats[STORY_AVG_SUBGOALS_PER_GOAL] + 1.0;
                            }



                        }

                    }

                }
                //Calculate average by dividing total number of subgoal actions in entire story by number of author goals
                currentStoryStats[STORY_AVG_SUBGOALS_PER_GOAL] = (double)currentStoryStats[STORY_AVG_SUBGOALS_PER_GOAL] / (double)currentStoryStats[STORY_COUNT_GOALS];

                
            }
            else
            {
                currentStoryStats[STORY_AVG_SUBGOALS_PER_GOAL] = 0.0;
            }


            #endregion


            #region STORY_COUNT_CHARACTERS
            currentStoryStats[STORY_COUNT_CHARACTERS] = (double)inputStory.Characters.Count;
            #endregion

            #region STORY_COUNT_CHARACTER_TRAITS
            if ((double)currentStoryStats[STORY_COUNT_CHARACTERS] > 0.0)
            {
                currentStoryStats[STORY_COUNT_CHARACTER_TRAITS] = (double)inputStory.Characters[0].Traits.Count;
            }
            else
            {
                currentStoryStats[STORY_COUNT_CHARACTER_TRAITS] = 0.0;
            }
            #endregion

            #region STORY_COUNT_CHARACTER_RELATIONSHIPS
            if ((double)currentStoryStats[STORY_COUNT_CHARACTERS] > 0.0)
            {
                currentStoryStats[STORY_COUNT_CHARACTER_RELATIONSHIPS] = (double)inputStory.Characters[0].Relationships.Count;
            }
            else
            {
                currentStoryStats[STORY_COUNT_CHARACTER_RELATIONSHIPS] = 0.0;
            }
            #endregion

            #region STORY_COUNT_ENVIRONMENTS
            currentStoryStats[STORY_COUNT_ENVIRONMENTS] = (double)inputStory.Environments.Count;
            #endregion

            #region STORY_COUNT_ENVIRONMENT_TRAITS
            if ((double)currentStoryStats[STORY_COUNT_ENVIRONMENTS] > 0.0)
            {
                currentStoryStats[STORY_COUNT_ENVIRONMENT_TRAITS] = (double)inputStory.Environments[0].Traits.Count;
            }
            else
            {
                currentStoryStats[STORY_COUNT_ENVIRONMENT_TRAITS] = 0.0;
            }
            #endregion

            #region STORY_COUNT_ENVIRONMENT_RELATIONSHIPS
            if ((double)currentStoryStats[STORY_COUNT_ENVIRONMENTS] > 0.0)
            {
                currentStoryStats[STORY_COUNT_ENVIRONMENT_RELATIONSHIPS] = (double)inputStory.Environments[0].Relationships.Count;
            }
            else
            {
                currentStoryStats[STORY_COUNT_ENVIRONMENT_RELATIONSHIPS] = 0.0;
            }
            #endregion

            #region STORY_COUNT_PLOTPOINTS
            currentStoryStats[STORY_COUNT_PLOTPOINTS] = (double)inputStory.PlotPointTypes.Count;
            #endregion

            #region STORY_COUNT_PLOTPOINT_TRAITS
            if ((double)currentStoryStats[STORY_COUNT_PLOTPOINTS] > 0.0)
            {
                currentStoryStats[STORY_COUNT_PLOTPOINT_TRAITS] = (double)inputStory.PlotPointTypes[0].Traits.Count;
            }
            else
            {
                currentStoryStats[STORY_COUNT_PLOTPOINT_TRAITS] = 0.0;
            }
            #endregion

            #region STORY_INTERACTIVE_ACTIONS
            currentStoryStats[STORY_INTERACTIVE_ACTIONS] = (double)inputStory.Interactions.Count;
            #endregion



            #region STORY_DEEPEST_GOALFRAG_TREE_PATH
            ArrayList pathDepths = new ArrayList(10);

            Hashtable goalTable = new Hashtable(inputStory.AuthorGoals.Count);
            Stack goalStack = new Stack();
            foreach(AuthorGoal g in inputStory.AuthorGoals)
            {
                goalTable.Add(g.Id, g);
            }
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
               
                measureGoalDepth(inputStory, goalStack, goalTable, g, pathDepths, 1);
                goalStack.Clear();

            }

            int totalPathDepth = 0;
            int deepestPathDepth = 0;
            foreach(int pathDepth in pathDepths)
            {
                totalPathDepth += pathDepth;
                deepestPathDepth = pathDepth > deepestPathDepth ? pathDepth : deepestPathDepth;

            }

            currentStoryStats[STORY_DEEPEST_GOALFRAG_TREE_PATH] = (double)deepestPathDepth;
            #endregion

            #region STORY_AVG_DEPTH_GOALFRAG_TREE_PATH

            if (pathDepths.Count > 0)
            {
                currentStoryStats[STORY_AVG_DEPTH_GOALFRAG_TREE_PATH] = (double)totalPathDepth /(double)pathDepths.Count;
            }
            else
            {
                currentStoryStats[STORY_AVG_DEPTH_GOALFRAG_TREE_PATH] = 0.0;
            }
              
            #endregion

            #region PLOTFRAG_COUNT_PARAMETERS
            currentStoryStats[PLOTFRAG_COUNT_PARAMETERS] = 0.0;
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
                currentStoryStats[PLOTFRAG_COUNT_PARAMETERS] = (double)currentStoryStats[PLOTFRAG_COUNT_PARAMETERS] + (double)g.Parameters.Count;
            }
            #endregion

            #region PLOTFRAG_AVG_PARAMETERS
            if ((double)currentStoryStats[STORY_COUNT_GOALS] > 0.0)
            {
                currentStoryStats[PLOTFRAG_AVG_PARAMETERS] = (double)currentStoryStats[PLOTFRAG_COUNT_PARAMETERS] / (double)currentStoryStats[STORY_COUNT_GOALS];
            }
            else
            {
                currentStoryStats[PLOTFRAG_AVG_PARAMETERS] = 0.0;
            }
            #endregion

            #region PRECOND_COUNT_PRECONDS
            currentStoryStats[PRECOND_COUNT_PRECONDS] = 0.0;
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
                foreach(PlotFragment f in g.PlotFragments)
                {
                    currentStoryStats[PRECOND_COUNT_PRECONDS] = (double)currentStoryStats[PRECOND_COUNT_PRECONDS] + (double)f.PrecStatements.Count;
                }
                
            }
            #endregion

            #region PRECOND_AVG_PRECONDS
            if ((double)currentStoryStats[STORY_COUNT_PLOTFRAGS] > 0.0)
            {
                currentStoryStats[PRECOND_AVG_PRECONDS] = (double)currentStoryStats[PRECOND_COUNT_PRECONDS] / (double)currentStoryStats[STORY_COUNT_PLOTFRAGS];
            }
            else
            {
                currentStoryStats[PRECOND_AVG_PRECONDS] = 0.0;
            }
            #endregion

            #region PRECOND_COUNT_CONSTRAINTS
            currentStoryStats[PRECOND_COUNT_CONSTRAINTS] = 0.0;
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
                foreach (PlotFragment f in g.PlotFragments)
                {
                    foreach(PreconditionStatement p in f.PrecStatements)
                    {
                        currentStoryStats[PRECOND_COUNT_CONSTRAINTS] = (double)currentStoryStats[PRECOND_COUNT_CONSTRAINTS] + (double)p.Constraints.Count;
                    }
                    
                }

            }
            #endregion

            #region PRECOND_AVG_CONSTRAINTS
            if ((double)currentStoryStats[PRECOND_COUNT_PRECONDS] > 0.0)
            {
                currentStoryStats[PRECOND_AVG_CONSTRAINTS] = (double)currentStoryStats[PRECOND_COUNT_CONSTRAINTS] / (double)currentStoryStats[PRECOND_COUNT_PRECONDS];
            }
            else
            {
                currentStoryStats[PRECOND_AVG_CONSTRAINTS] = 0.0;
            }
            #endregion


            #region PRECOND_COUNT_TOTAL_VAR_BINDINGS
            currentStoryStats[PRECOND_COUNT_TOTAL_VAR_BINDINGS] = 0.0;
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
                foreach (PlotFragment f in g.PlotFragments)
                {
                    foreach (PreconditionStatement p in f.PrecStatements)
                    {
                        if(p.SaveMatchedObject)
                        {
                            currentStoryStats[PRECOND_COUNT_TOTAL_VAR_BINDINGS] = (double)currentStoryStats[PRECOND_COUNT_TOTAL_VAR_BINDINGS] + 1.0;

                        }
                        foreach (Constraint c in p.Constraints)
                        {
                            if(c.ContainsSavedVariable)
                            {
                                currentStoryStats[PRECOND_COUNT_TOTAL_VAR_BINDINGS] = (double)currentStoryStats[PRECOND_COUNT_TOTAL_VAR_BINDINGS] + 1.0;
                            }
                        }
                        
                    }

                }

            }
            #endregion

            #region PRECOND_AVG_VAR_BINDINGS_PER_PLOTFRAG
            if ((double)currentStoryStats[STORY_COUNT_PLOTFRAGS] > 0.0)
            {
                currentStoryStats[PRECOND_AVG_VAR_BINDINGS_PER_PLOTFRAG] = (double)currentStoryStats[PRECOND_COUNT_TOTAL_VAR_BINDINGS] / (double)currentStoryStats[STORY_COUNT_PLOTFRAGS];
            }
            else
            {
                currentStoryStats[PRECOND_AVG_VAR_BINDINGS_PER_PLOTFRAG] = 0.0;
            }
            #endregion

            #region PRECOND_COUNT_NEGATION
            currentStoryStats[PRECOND_COUNT_NEGATION] = 0.0;
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
                foreach (PlotFragment f in g.PlotFragments)
                {
                    foreach (PreconditionStatement p in f.PrecStatements)
                    {
                        if(!p.ObjectExists)
                        {
                            currentStoryStats[PRECOND_COUNT_NEGATION] = (double)currentStoryStats[PRECOND_COUNT_NEGATION] + 1.0;
                        }
                        
                           

                    }

                }

            }
            #endregion


            #region PRECOND_COUNT_TOTAL_VAR_REFERENCES
            currentStoryStats[PRECOND_COUNT_TOTAL_VAR_REFERENCES] = 0.0;
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
                foreach (PlotFragment f in g.PlotFragments)
                {
                    foreach (PreconditionStatement p in f.PrecStatements)
                    {
                        foreach (Constraint c in p.Constraints)
                        {
                            if(c.ComparisonValue.ValueIsBoundToVariable)
                            {
                                currentStoryStats[PRECOND_COUNT_TOTAL_VAR_REFERENCES] = (double)currentStoryStats[PRECOND_COUNT_TOTAL_VAR_REFERENCES] + 1.0;
                            }
                        }
                        
                    }

                }

            }
            #endregion

            #region PRECOND_AVG_VAR_REFERENCES_PER_PLOTFRAG
            if ((double)currentStoryStats[STORY_COUNT_PLOTFRAGS] > 0.0)
            {
                currentStoryStats[PRECOND_AVG_VAR_REFERENCES_PER_PLOTFRAG] = (double)currentStoryStats[PRECOND_COUNT_TOTAL_VAR_REFERENCES] / (double)currentStoryStats[STORY_COUNT_PLOTFRAGS];
            }
            else
            {
                currentStoryStats[PRECOND_AVG_VAR_REFERENCES_PER_PLOTFRAG] = 0.0;
            }
            #endregion

            #region PRECOND_COUNT_NEGATION
            currentStoryStats[PRECOND_COUNT_NEGATION] = 0.0;
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
                foreach (PlotFragment f in g.PlotFragments)
                {
                    foreach (PreconditionStatement p in f.PrecStatements)
                    {
                        if(!p.ObjectExists)
                        {
                            currentStoryStats[PRECOND_COUNT_NEGATION] = (double)currentStoryStats[PRECOND_COUNT_NEGATION] + 1.0;
                        }
                        
                           

                    }

                }

            }
            #endregion



            #region PRECOND_AVG_NEGATION
            if ((double)currentStoryStats[STORY_COUNT_PLOTFRAGS] > 0.0)
            {
                currentStoryStats[PRECOND_AVG_NEGATION] = (double)currentStoryStats[PRECOND_COUNT_NEGATION] / (double)currentStoryStats[STORY_COUNT_PLOTFRAGS];
            }
            else
            {
                currentStoryStats[PRECOND_AVG_NEGATION] = 0.0;
            }
            #endregion


            #region ACTION_TOTAL_ACTIONS
            currentStoryStats[ACTION_TOTAL_ACTIONS] = 0.0;
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
                foreach (PlotFragment f in g.PlotFragments)
                {
                    currentStoryStats[ACTION_TOTAL_ACTIONS] = (double)currentStoryStats[ACTION_TOTAL_ACTIONS] + (double)f.Actions.Count;
                }

            }
            #endregion

            #region ACTION_AVG_ACTIONS
            if ((double)currentStoryStats[STORY_COUNT_PLOTFRAGS] > 0.0)
            {
                currentStoryStats[ACTION_AVG_ACTIONS] = (double)currentStoryStats[ACTION_TOTAL_ACTIONS] / (double)currentStoryStats[STORY_COUNT_PLOTFRAGS];
            }
            else
            {
                currentStoryStats[ACTION_AVG_ACTIONS] = 0.0;
            }
            #endregion

            #region ACTION_COUNT_VAR_REFERENCES
            currentStoryStats[ACTION_COUNT_VAR_REFERENCES] = 0.0;
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
                foreach (PlotFragment f in g.PlotFragments)
                {
                    foreach (Action a in f.Actions)
                    {
                        if(a is ActionCalculation)
                        {

                            if( ((ActionCalculation)a).ParamLeft.ValueIsBoundToVariable)
                            {
                                currentStoryStats[ACTION_COUNT_VAR_REFERENCES] = (double)currentStoryStats[ACTION_COUNT_VAR_REFERENCES] + 1.0;
                            }
                            if (((ActionCalculation)a).ParamRight.ValueIsBoundToVariable)
                            {
                                currentStoryStats[ACTION_COUNT_VAR_REFERENCES] = (double)currentStoryStats[ACTION_COUNT_VAR_REFERENCES] + 1.0;
                            }
                        }
                        else if (a is ActionEditObject)
                        {
                            if (((ActionEditObject)a).Mode == ObjectEditingMode.RelationshipTarget && ((ActionEditObject)a).NewTarget.ValueIsBoundToVariable)
                            {
                                currentStoryStats[ACTION_COUNT_VAR_REFERENCES] = (double)currentStoryStats[ACTION_COUNT_VAR_REFERENCES] + 1.0;
                            }
                            else if (((ActionEditObject)a).NewValue.ValueIsBoundToVariable)
                            {
                                currentStoryStats[ACTION_COUNT_VAR_REFERENCES] = (double)currentStoryStats[ACTION_COUNT_VAR_REFERENCES] + 1.0;
                            }
                        }
                        else if (a is ActionTextOutput)
                        {
                            Regex varMatcher = new Regex("(?:[^<])[^<]*(?=>(?!>))", RegexOptions.Compiled | RegexOptions.IgnoreCase);


                            MatchCollection varMatches = varMatcher.Matches(((ActionTextOutput)a).TextOutput);
                            if(varMatches.Count > 0)
                            {
                                currentStoryStats[ACTION_COUNT_VAR_REFERENCES] = (double)currentStoryStats[ACTION_COUNT_VAR_REFERENCES] + (double)varMatches.Count;
                            }
                        }
                        else if (a is ActionSubgoal)
                        {
                            foreach(Parameter p in ((ActionSubgoal)a).ParametersToPass)
                            {
                                if(p.ValueIsBoundToVariable)
                                {
                                    currentStoryStats[ACTION_COUNT_VAR_REFERENCES] = (double)currentStoryStats[ACTION_COUNT_VAR_REFERENCES] + 1.0;
                                }
                            }
                        }
                        
                                
                       
                    }

                }

            }
            #endregion

            #region ACTION_AVG_VAR_REFERENCES
            if ((double)currentStoryStats[STORY_COUNT_PLOTFRAGS] > 0.0)
            {
                currentStoryStats[ACTION_AVG_VAR_REFERENCES] = (double)currentStoryStats[ACTION_COUNT_VAR_REFERENCES] / (double)currentStoryStats[STORY_COUNT_PLOTFRAGS];
            }
            else
            {
                currentStoryStats[ACTION_AVG_VAR_REFERENCES] = 0.0;
            }
            #endregion

            #region ACTION_COUNT_VAR_BINDINGS
            currentStoryStats[ACTION_COUNT_VAR_BINDINGS] = 0.0;
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
                foreach (PlotFragment f in g.PlotFragments)
                {
                    foreach (Action a in f.Actions)
                    {
                        if(a is ActionCalculation)
                        {

                            currentStoryStats[ACTION_COUNT_VAR_BINDINGS] = (double)currentStoryStats[ACTION_COUNT_VAR_BINDINGS] + 1.0;
                            
                        }
                        else if (a is ActionCreateCharacter)
                        {
                            currentStoryStats[ACTION_COUNT_VAR_BINDINGS] = (double)currentStoryStats[ACTION_COUNT_VAR_BINDINGS] + 1.0;
                            
 
                        }
                        else if (a is ActionCreateEnvironment)
                        {
                            currentStoryStats[ACTION_COUNT_VAR_BINDINGS] = (double)currentStoryStats[ACTION_COUNT_VAR_BINDINGS] + 1.0;


                        }
                        else if (a is ActionCreatePlotPoint)
                        {
                            currentStoryStats[ACTION_COUNT_VAR_BINDINGS] = (double)currentStoryStats[ACTION_COUNT_VAR_BINDINGS] + 1.0;


                        }
                    }

                }

            }
            #endregion

            #region ACTION_AVG_VAR_BINDINGS_PER_PLOTFRAG
            if ((double)currentStoryStats[STORY_COUNT_PLOTFRAGS] > 0.0)
            {
                currentStoryStats[ACTION_AVG_VAR_BINDINGS_PER_PLOTFRAG] = (double)currentStoryStats[ACTION_COUNT_VAR_BINDINGS] / (double)currentStoryStats[STORY_COUNT_PLOTFRAGS];
            }
            else
            {
                currentStoryStats[ACTION_AVG_VAR_BINDINGS_PER_PLOTFRAG] = 0.0;
            }
            #endregion


            #region ACTION_COUNT_TEXT_OUTPUTS
            currentStoryStats[ACTION_COUNT_TEXT_OUTPUTS] = 0.0;
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
                foreach (PlotFragment f in g.PlotFragments)
                {
                    foreach (Action a in f.Actions)
                    {
                        if(a is ActionTextOutput)
                        {
                            currentStoryStats[ACTION_COUNT_TEXT_OUTPUTS] = (double)currentStoryStats[ACTION_COUNT_TEXT_OUTPUTS] + 1.0;
                        }
                    }
                    
                }

            }
            #endregion

            #region ACTION_AVG_TEXT_OUTPUTS
            if ((double)currentStoryStats[STORY_COUNT_PLOTFRAGS] > 0.0)
            {
                currentStoryStats[ACTION_AVG_TEXT_OUTPUTS] = (double)currentStoryStats[ACTION_COUNT_TEXT_OUTPUTS] / (double)currentStoryStats[STORY_COUNT_PLOTFRAGS];
            }
            else
            {
                currentStoryStats[ACTION_AVG_TEXT_OUTPUTS] = 0.0;
            }
            #endregion




            #region ACTION_AVG_TEXT_OUTPUT_VARS and ACTION_COUNT_TEXT_OUTPUT_VARS


            currentStoryStats[ACTION_AVG_TEXT_OUTPUT_VARS] = 0.0;
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
                foreach (PlotFragment f in g.PlotFragments)
                {
                    foreach (Action a in f.Actions)
                    {
                       if (a is ActionTextOutput)
                        {
                            Regex varMatcher = new Regex("(?:[^<])[^<]*(?=>(?!>))", RegexOptions.Compiled | RegexOptions.IgnoreCase);


                            MatchCollection varMatches = varMatcher.Matches(((ActionTextOutput)a).TextOutput);
                            if (varMatches.Count > 0)
                            {
                                currentStoryStats[ACTION_AVG_TEXT_OUTPUT_VARS] = (double)currentStoryStats[ACTION_AVG_TEXT_OUTPUT_VARS] + (double)varMatches.Count;
                            }
                        }
                        

                    }

                }

            }

            //Store the total count of variable references before we caculate the average
            currentStoryStats[ACTION_COUNT_TEXT_OUTPUT_VARS] = (double)currentStoryStats[ACTION_AVG_TEXT_OUTPUT_VARS];

            //Calculate average per text output
            if ((double)currentStoryStats[ACTION_COUNT_TEXT_OUTPUTS] > 0.0)
            {
                currentStoryStats[ACTION_AVG_TEXT_OUTPUT_VARS] = (double)currentStoryStats[ACTION_AVG_TEXT_OUTPUT_VARS] / (double)currentStoryStats[ACTION_COUNT_TEXT_OUTPUTS];
            }
            else
            {
                currentStoryStats[ACTION_AVG_TEXT_OUTPUT_VARS] = 0.0;
            }
            #endregion



            #region ACTION_AVG_TEXT_OUTPUT_LENGTH
            currentStoryStats[ACTION_AVG_TEXT_OUTPUT_LENGTH] = 0.0;
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
                foreach (PlotFragment f in g.PlotFragments)
                {
                    foreach (Action a in f.Actions)
                    {
                        if (a is ActionTextOutput)
                        {
                            //Add up total number of characters
                            currentStoryStats[ACTION_AVG_TEXT_OUTPUT_LENGTH] = (double)currentStoryStats[ACTION_AVG_TEXT_OUTPUT_LENGTH] + (double)((ActionTextOutput)a).TextOutput.Length;
                        }
                    }

                }

            }
            //Divide out total number of text output actions to get average char count
            currentStoryStats[ACTION_AVG_TEXT_OUTPUT_LENGTH] = (double)currentStoryStats[ACTION_AVG_TEXT_OUTPUT_LENGTH] / (double)currentStoryStats[ACTION_COUNT_TEXT_OUTPUTS];
            #endregion

            #region ACTION_COUNT_CHAR_ADDS
            currentStoryStats[ACTION_COUNT_CHAR_ADDS] = 0.0;
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
                foreach (PlotFragment f in g.PlotFragments)
                {
                    foreach (Action a in f.Actions)
                    {
                        if (a is ActionCreateCharacter)
                        {
                            currentStoryStats[ACTION_COUNT_CHAR_ADDS] = (double)currentStoryStats[ACTION_COUNT_CHAR_ADDS] + 1.0;
                        }
                    }

                }

            }


            #endregion

            #region ACTION_AVG_CHAR_ADDS
            if ((double)currentStoryStats[STORY_COUNT_PLOTFRAGS] > 0.0)
            {
                currentStoryStats[ACTION_AVG_CHAR_ADDS] = (double)currentStoryStats[ACTION_COUNT_CHAR_ADDS] / (double)currentStoryStats[STORY_COUNT_PLOTFRAGS];
            }
            else
            {
                currentStoryStats[ACTION_AVG_CHAR_ADDS] = 0.0;
            }
            #endregion

            #region ACTION_COUNT_CHAR_DELETES
            currentStoryStats[ACTION_COUNT_CHAR_DELETES] = 0.0;
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
                foreach (PlotFragment f in g.PlotFragments)
                {
                    foreach (Action a in f.Actions)
                    {
                        if (a is ActionDeleteEntity && ((ActionDeleteEntity)a).TypeId == inputStory.CharTypeId)
                        {
                            currentStoryStats[ACTION_COUNT_CHAR_DELETES] = (double)currentStoryStats[ACTION_COUNT_CHAR_DELETES] + 1.0;
                        }
                    }

                }

            }
            #endregion

            #region ACTION_AVG_CHAR_DELETES
            if ((double)currentStoryStats[STORY_COUNT_PLOTFRAGS] > 0.0)
            {
                currentStoryStats[ACTION_AVG_CHAR_DELETES] = (double)currentStoryStats[ACTION_COUNT_CHAR_DELETES] / (double)currentStoryStats[STORY_COUNT_PLOTFRAGS];
            }
            else
            {
                currentStoryStats[ACTION_AVG_CHAR_DELETES] = 0.0;
            }
            #endregion


            #region ACTION_COUNT_CHAR_EDITS
            currentStoryStats[ACTION_COUNT_CHAR_EDITS] = 0.0;
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
                foreach (PlotFragment f in g.PlotFragments)
                {
                    foreach (Action a in f.Actions)
                    {
                        if (a is ActionEditObject && ((ActionEditObject)a).ObjectTypeId == inputStory.CharTypeId)
                        {
                            currentStoryStats[ACTION_COUNT_CHAR_EDITS] = (double)currentStoryStats[ACTION_COUNT_CHAR_EDITS] + 1.0;
                        }
                    }

                }

            }
            #endregion

            #region ACTION_AVG_CHAR_EDITS
             if ((double)currentStoryStats[STORY_COUNT_PLOTFRAGS] > 0.0)
            {
                currentStoryStats[ACTION_AVG_CHAR_EDITS] = (double)currentStoryStats[ACTION_COUNT_CHAR_EDITS] / (double)currentStoryStats[STORY_COUNT_PLOTFRAGS];
            }
            else
            {
                currentStoryStats[ACTION_AVG_CHAR_EDITS] = 0.0;
            }
            
            #endregion


             #region ACTION_COUNT_ENV_ADDS
             currentStoryStats[ACTION_COUNT_ENV_ADDS] = 0.0;
             foreach (AuthorGoal g in inputStory.AuthorGoals)
             {
                 foreach (PlotFragment f in g.PlotFragments)
                 {
                     foreach (Action a in f.Actions)
                     {
                         if (a is ActionCreateEnvironment)
                         {
                             currentStoryStats[ACTION_COUNT_ENV_ADDS] = (double)currentStoryStats[ACTION_COUNT_ENV_ADDS] + 1.0;
                         }
                     }

                 }

             }


             #endregion

             #region ACTION_AVG_ENV_ADDS
             if ((double)currentStoryStats[STORY_COUNT_PLOTFRAGS] > 0.0)
             {
                 currentStoryStats[ACTION_AVG_ENV_ADDS] = (double)currentStoryStats[ACTION_COUNT_ENV_ADDS] / (double)currentStoryStats[STORY_COUNT_PLOTFRAGS];
             }
             else
             {
                 currentStoryStats[ACTION_AVG_ENV_ADDS] = 0.0;
             }
             #endregion

             #region ACTION_COUNT_ENV_DELETES
             currentStoryStats[ACTION_COUNT_ENV_DELETES] = 0.0;
             foreach (AuthorGoal g in inputStory.AuthorGoals)
             {
                 foreach (PlotFragment f in g.PlotFragments)
                 {
                     foreach (Action a in f.Actions)
                     {
                         if (a is ActionDeleteEntity && ((ActionDeleteEntity)a).TypeId == inputStory.EnvTypeId)
                         {
                             currentStoryStats[ACTION_COUNT_ENV_DELETES] = (double)currentStoryStats[ACTION_COUNT_ENV_DELETES] + 1.0;
                         }
                     }

                 }

             }
             #endregion

             #region ACTION_AVG_ENV_DELETES
             if ((double)currentStoryStats[STORY_COUNT_PLOTFRAGS] > 0.0)
             {
                 currentStoryStats[ACTION_AVG_CHAR_DELETES] = (double)currentStoryStats[ACTION_COUNT_ENV_DELETES] / (double)currentStoryStats[STORY_COUNT_PLOTFRAGS];
             }
             else
             {
                 currentStoryStats[ACTION_AVG_ENV_DELETES] = 0.0;
             }
             #endregion


            #region ACTION_COUNT_ENV_EDITS
            currentStoryStats[ACTION_COUNT_ENV_EDITS] = 0.0;
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
                foreach (PlotFragment f in g.PlotFragments)
                {
                    foreach (Action a in f.Actions)
                    {
                        if (a is ActionEditObject && ((ActionEditObject)a).ObjectTypeId == inputStory.EnvTypeId)
                        {
                            currentStoryStats[ACTION_COUNT_ENV_EDITS] = (double)currentStoryStats[ACTION_COUNT_ENV_EDITS] + 1.0;
                        }
                    }

                }

            }
            #endregion

            #region ACTION_AVG_ENV_EDITS
            if ((double)currentStoryStats[STORY_COUNT_PLOTFRAGS] > 0.0)
            {
                currentStoryStats[ACTION_AVG_ENV_EDITS] = (double)currentStoryStats[ACTION_COUNT_ENV_EDITS] / (double)currentStoryStats[STORY_COUNT_PLOTFRAGS];
            }
            else
            {
                currentStoryStats[ACTION_AVG_ENV_EDITS] = 0.0;
            }
            #endregion

            #region ACTION_COUNT_PP_ADDS
            currentStoryStats[ACTION_COUNT_PP_ADDS] = 0.0;
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
                foreach (PlotFragment f in g.PlotFragments)
                {
                    foreach (Action a in f.Actions)
                    {
                        if (a is ActionCreatePlotPoint)
                        {
                            currentStoryStats[ACTION_COUNT_PP_ADDS] = (double)currentStoryStats[ACTION_COUNT_PP_ADDS] + 1.0;
                        }
                    }

                }

            }


            #endregion

            #region ACTION_AVG_PP_ADDS
            if ((double)currentStoryStats[STORY_COUNT_PLOTFRAGS] > 0.0)
            {
                currentStoryStats[ACTION_AVG_PP_ADDS] = (double)currentStoryStats[ACTION_COUNT_PP_ADDS] / (double)currentStoryStats[STORY_COUNT_PLOTFRAGS];
            }
            else
            {
                currentStoryStats[ACTION_AVG_PP_ADDS] = 0.0;
            }
            #endregion

            #region ACTION_COUNT_PP_DELETES
            currentStoryStats[ACTION_COUNT_PP_DELETES] = 0.0;
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
                foreach (PlotFragment f in g.PlotFragments)
                {
                    foreach (Action a in f.Actions)
                    {
                        if (a is ActionDeleteEntity && ((ActionDeleteEntity)a).TypeId != inputStory.EnvTypeId && ((ActionDeleteEntity)a).TypeId != inputStory.CharTypeId)
                        {
                            currentStoryStats[ACTION_COUNT_PP_DELETES] = (double)currentStoryStats[ACTION_COUNT_PP_DELETES] + 1.0;
                        }
                    }

                }

            }
            #endregion

            #region ACTION_AVG_PP_DELETES
            if ((double)currentStoryStats[STORY_COUNT_PLOTFRAGS] > 0.0)
            {
                currentStoryStats[ACTION_AVG_PP_DELETES] = (double)currentStoryStats[ACTION_COUNT_PP_DELETES] / (double)currentStoryStats[STORY_COUNT_PLOTFRAGS];
            }
            else
            {
                currentStoryStats[ACTION_AVG_PP_DELETES] = 0.0;
            }
            #endregion

            #region ACTION_COUNT_PP_EDITS
            currentStoryStats[ACTION_COUNT_PP_EDITS] = 0.0;
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
                foreach (PlotFragment f in g.PlotFragments)
                {
                    foreach (Action a in f.Actions)
                    {
                        if (a is ActionEditObject && ((ActionEditObject)a).ObjectTypeId != inputStory.EnvTypeId && ((ActionEditObject)a).ObjectTypeId != inputStory.CharTypeId)
                        {
                            currentStoryStats[ACTION_COUNT_PP_EDITS] = (double)currentStoryStats[ACTION_COUNT_PP_EDITS] + 1.0;
                            
                        }
                    }

                }

            }
            #endregion

            #region ACTION_AVG_PP_EDITS
            if ((double)currentStoryStats[STORY_COUNT_PLOTFRAGS] > 0.0)
            {
                currentStoryStats[ACTION_AVG_PP_EDITS] = (double)currentStoryStats[ACTION_COUNT_PP_EDITS] / (double)currentStoryStats[STORY_COUNT_PLOTFRAGS];
            }
            else
            {
                currentStoryStats[ACTION_AVG_PP_EDITS] = 0.0;
            }
            #endregion

            #region ACTION_COUNT_SUBGOALS
            currentStoryStats[ACTION_COUNT_SUBGOALS] = 0.0;
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
                foreach (PlotFragment f in g.PlotFragments)
                {
                    foreach (Action a in f.Actions)
                    {
                        if (a is ActionSubgoal)
                        {
                            currentStoryStats[ACTION_COUNT_SUBGOALS] = (double)currentStoryStats[ACTION_COUNT_SUBGOALS] + 1.0;
                           
                        }
                    }

                }

            }
            #endregion

            #region ACTION_AVG_SUBGOALS
            if ((double)currentStoryStats[STORY_COUNT_PLOTFRAGS] > 0.0)
            {
                currentStoryStats[ACTION_AVG_SUBGOALS] = (double)currentStoryStats[ACTION_COUNT_SUBGOALS] / (double)currentStoryStats[STORY_COUNT_PLOTFRAGS];
            }
            else
            {
                currentStoryStats[ACTION_AVG_SUBGOALS] = 0.0;
            }
            #endregion

            #region ACTION_COUNT_CALCS
            currentStoryStats[ACTION_COUNT_CALCS] = 0.0;
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
                foreach (PlotFragment f in g.PlotFragments)
                {
                    foreach (Action a in f.Actions)
                    {
                        if (a is ActionCalculation)
                        {
                            currentStoryStats[ACTION_COUNT_CALCS] = (double)currentStoryStats[ACTION_COUNT_CALCS] + 1.0;

                        }
                    }

                }

            }
            #endregion

            #region ACTION_AVG_CALCS
            if ((double)currentStoryStats[STORY_COUNT_PLOTFRAGS] > 0.0)
            {
                currentStoryStats[ACTION_AVG_CALCS] = (double)currentStoryStats[ACTION_COUNT_CALCS] / (double)currentStoryStats[STORY_COUNT_PLOTFRAGS];
            }
            else
            {
                currentStoryStats[ACTION_AVG_CALCS] = 0.0;
            }
            #endregion

            #region ACTION_AVG_ACTIONS_BETWEEN_SUBGOALS
            currentStoryStats[ACTION_AVG_ACTIONS_BETWEEN_SUBGOALS] = 0.0;
            int gapCount = 0;
            foreach (AuthorGoal g in inputStory.AuthorGoals)
            {
                foreach (PlotFragment f in g.PlotFragments)
                {
                    int numSubgoalsLeft = 0;
                    int numSubgoals = 0;
                    foreach (Action a in f.Actions)
                    {
                        if(a is ActionSubgoal)
                        {
                            numSubgoals++;
                            numSubgoalsLeft++;
                        }
                    }
                    if(numSubgoals > 1) //Gap between subgoals exists, therefore perform count
                    {
                        foreach (Action a in f.Actions)
                        {
                            if (a is ActionSubgoal)
                            {
                                
                                numSubgoalsLeft--; //Decrement remain subgoal count
                                if(numSubgoalsLeft > 0) //If there is at least one subgoal left, then we are beginning a gap section
                                {
                                    gapCount++;
                                }

                            }
                            else if ((numSubgoals > numSubgoalsLeft) && (numSubgoalsLeft > 0)) 
                            //We are located after first subgoal, and there are still subgoals left, therefore we are currently inside a gap between subgoals,
                            //and should increment the total gap size by one unit
                            {
                                currentStoryStats[ACTION_AVG_ACTIONS_BETWEEN_SUBGOALS] = (double)currentStoryStats[ACTION_AVG_ACTIONS_BETWEEN_SUBGOALS] + 1.0;
                            }
                        }
                    }
                    

                }

            }
            if(gapCount > 0) //Calculate average gap size between subgoals by dividing total gap size of all gaps by number of gaps
            {
                currentStoryStats[ACTION_AVG_ACTIONS_BETWEEN_SUBGOALS] = (double)currentStoryStats[ACTION_AVG_ACTIONS_BETWEEN_SUBGOALS] / (double)gapCount;
            }
            else 
            {
                currentStoryStats[ACTION_AVG_ACTIONS_BETWEEN_SUBGOALS] = 0.0;
            }
            #endregion

            #region PLOTFRAG_COUNT_VAR_REFERENCES
            currentStoryStats[PLOTFRAG_COUNT_VAR_REFERENCES] = (double)currentStoryStats[ACTION_COUNT_VAR_REFERENCES] + (double)currentStoryStats[PRECOND_COUNT_TOTAL_VAR_REFERENCES];
            #endregion


            #region PLOTFRAG_AVG_VAR_REFERENCES
            if ((double)currentStoryStats[STORY_COUNT_PLOTFRAGS] > 0.0)
            {
                currentStoryStats[PLOTFRAG_AVG_VAR_REFERENCES] = (double)currentStoryStats[PLOTFRAG_COUNT_VAR_REFERENCES] / (double)currentStoryStats[STORY_COUNT_PLOTFRAGS];
            }
            else
            {
                currentStoryStats[PLOTFRAG_AVG_VAR_REFERENCES] = 0.0;
            }
            #endregion

            #region PLOTFRAG_COUNT_VAR_BINDINGS
            currentStoryStats[PLOTFRAG_COUNT_VAR_BINDINGS] = (double)currentStoryStats[ACTION_COUNT_VAR_BINDINGS] + (double)currentStoryStats[PRECOND_COUNT_TOTAL_VAR_BINDINGS];
            #endregion


            #region PLOTFRAG_AVG_VAR_BINDINGS
            if ((double)currentStoryStats[STORY_COUNT_PLOTFRAGS] > 0.0)
            {
                currentStoryStats[PLOTFRAG_AVG_VAR_BINDINGS] = (double)currentStoryStats[PLOTFRAG_COUNT_VAR_BINDINGS] / (double)currentStoryStats[STORY_COUNT_PLOTFRAGS];
            }
            else
            {
                currentStoryStats[PLOTFRAG_AVG_VAR_BINDINGS] = 0.0;
            }
            #endregion

            #region SAMPLE SIMILARITY
            if (sampleStoryData == null) 
            {
                //We are analyzing the sample story itself
                currentStoryStats[SAMPLE_SIMILARITY] = 0.0;
            }
            else
            {
                int totalStats = 0; 
                double distanceSummation = 0.0;

                //Compute generalized euclidean distance for N dimensions, where N = the number of statistical values
                foreach (DictionaryEntry statElement in currentStoryStats) 
                {
                    if (!statElement.Key.Equals(SAMPLE_SIMILARITY) && !statElement.Key.Equals(FILE_NAME))
                    {
                        totalStats++;
                        distanceSummation += Math.Pow( 
                                                Math.Abs( 
                                                    ((double)sampleStoryData[statElement.Key]) - ((double)statElement.Value)
                                                ),
                                             2.0);

                    }
                }
                currentStoryStats[SAMPLE_SIMILARITY] = Math.Sqrt(distanceSummation);

            }
            #endregion


            #region GRADING_POINTS

            gradeStory(currentStoryStats, inputStory);

            #endregion

            return currentStoryStats;
        }

        private static void measureGoalDepth(StoryData inputStory, Stack goalStack, Hashtable goalTable, AuthorGoal currentGoal, ArrayList pathDepths, int currentDepth)
        {


            if(currentDepth > 50)
            {
                return;
            }
            if(goalStack.Contains(currentGoal))
            {
                //recursion occurring, end search
                return;
            }
            if(currentGoal == null)
            {
                //Null author goal

                pathDepths.Add(currentDepth);
                return;

            }

            if(currentGoal.PlotFragments.Count == 0)
            {
                //No plot fragment for subgoal, count as leaf of tree

                pathDepths.Add(currentDepth);
                return;
            }

            foreach (PlotFragment f in currentGoal.PlotFragments)
            {
                int numSubgoals = 0;
                foreach(Action a in f.Actions)
                {

                    if(a is ActionSubgoal)
                    {
                        numSubgoals++;
                        AuthorGoal goalToSearch = (AuthorGoal)goalTable[((ActionSubgoal)a).SubGoalId];
                        goalStack.Push(currentGoal);
                        measureGoalDepth(inputStory,goalStack, goalTable, goalToSearch, pathDepths, currentDepth + 1);
                        goalStack.Pop();
                        
                    }
                }
                if (numSubgoals == 0)
                {
                    //No subgoaling occured, so this path is a leaf of the tree
                    pathDepths.Add(currentDepth);
                }
            }


        }

        private static void gradeStory(Hashtable storyStats, StoryData storyData)
        {

            double gradePointTotal = 0.0;
            string gradingComment = "";

            ///
            // ***** Grading rubric: ******
            //             
            //   Must use each of these features at least once:
            //    Characters - traits and relationships - 5pts 

            if (
                ((double)storyStats[STORY_COUNT_CHARACTER_TRAITS]) > 0.1 &&
                ((double)storyStats[STORY_COUNT_CHARACTER_RELATIONSHIPS]) > 0.1)
            {
                gradePointTotal += 5.0;
            }
            else
            {
                gradingComment += "Need at least one Character trait and one Character relationship; ";
            }


            //    Environments - traits and relationships - 5pts
            if (
               ((double)storyStats[STORY_COUNT_ENVIRONMENT_TRAITS]) > 0.1 &&
               ((double)storyStats[STORY_COUNT_ENVIRONMENT_RELATIONSHIPS]) > 0.1)
            {
                gradePointTotal += 5.0;
            }
            else
            {
                gradingComment += "Need at least one Environment trait and one Environment relationship; ";
            }

            //     Plot Points - traits - 5pts
            if (((double)storyStats[STORY_COUNT_PLOTPOINT_TRAITS]) > 0.1)
            {
                gradePointTotal += 5.0;
            }
            else
            {
                gradingComment += "No Plot Point Traits; ";
            }

            //    . Author goal - parameters - 5pts
            if (((double)storyStats[PLOTFRAG_COUNT_PARAMETERS]) > 0.1)
            {
                gradePointTotal += 5.0;
            }
            else
            {
                gradingComment += "No Author Goal parameters; ";
            }

            //    .Plot fragments
            //       Preconditions - must have at least two diff kinds 7pts
            //         . Character precondition
            //         . environment prec
            //         . plot point prec.
            bool precondChar = false;
            bool precondEnv = false;
            bool precondPP = false;
            foreach (AuthorGoal g in storyData.AuthorGoals)
            {
                foreach (PlotFragment f in g.PlotFragments)
                {
                    foreach (PreconditionStatement p in f.PrecStatements)
                    {
                        if (p is PreconditionStatementCharacter)
                        {
                            precondChar = true;
                        }
                        else if (p is PreconditionStatementEnvironment)
                        {
                            precondEnv = true;
                        }
                        else if (p is PreconditionStatementPlotPoint)
                        {
                            precondPP = true;
                        }
                    }
                }
            }

            if (precondChar && precondEnv && precondPP) { gradePointTotal += 7.0; }
            else
            {
                if (precondChar) { gradePointTotal += 2.333333; }
                else {  gradingComment += "No Character precondition; ";}
                if (precondEnv) { gradePointTotal += 2.3333333; }
                else { gradingComment += "No Environment precondition; "; }
                if (precondPP) { gradePointTotal += 2.3333333; }
                else { gradingComment += "No Plot Point precondition; "; }
            }


            //      . Save a trait/relationship info to a variable 5pts
            if (((double)storyStats[PRECOND_COUNT_TOTAL_VAR_BINDINGS]) > 0.1)
            {
                gradePointTotal += 5.0;
            }
            else
            {
                gradingComment += "No saving of trait/relationship information to variable; ";
            }

            //      . Use variable within precondition 5pts
            if (((double)storyStats[PRECOND_COUNT_TOTAL_VAR_REFERENCES]) > 0.1)
            {
                gradePointTotal += 5.0;
            }
            else
            {
                gradingComment += "No referencing of saved variable within precondition; ";
            }

            //      . Actions 
            //        . Print text 5pts
            if (((double)storyStats[ACTION_COUNT_TEXT_OUTPUTS]) > 0.1)
            {
                gradePointTotal += 5.0;
            }
            else
            {
                gradingComment += "No Text Output action; ";
            }
            //      . Print text with inserted variables 5pts
            if (((double)storyStats[ACTION_COUNT_TEXT_OUTPUT_VARS]) > 0.1)
            {
                gradePointTotal += 5.0;
            }
            else
            {
                gradingComment += "No variable usage in Text Output actions; ";
            }

            //        . subgoal 10pts
            if (((double)storyStats[ACTION_COUNT_SUBGOALS]) > 0.1)
            {
                gradePointTotal += 10.0;
            }
            else
            {
                gradingComment += "No subgoal actions; ";
            }

            //         . calculate value 2pts
            if (((double)storyStats[ACTION_COUNT_CALCS]) > 0.1)
            {
                gradePointTotal += 2.0;
            }
            else
            {
                gradingComment += "No calculate actions; ";
            }

            //        . edit character 2pts
            if (
                ((double)storyStats[ACTION_COUNT_CHAR_EDITS]) > 0.1 ||
                ((double)storyStats[ACTION_COUNT_CHAR_ADDS]) > 0.1 ||
                ((double)storyStats[ACTION_COUNT_CHAR_DELETES]) > 0.1)
            {
                gradePointTotal += 2.0;
            }
            else
            {
                gradingComment += "No Character editing deletion or creation actions; ";
            }

            //        . edit environment 2pts
            if (((double)storyStats[ACTION_COUNT_ENV_EDITS]) > 0.1 ||
                ((double)storyStats[ACTION_COUNT_ENV_ADDS]) > 0.1 ||
                ((double)storyStats[ACTION_COUNT_ENV_DELETES]) > 0.1)
            {
                gradePointTotal += 2.0;
            }
            else
            {
                gradingComment += "No Environment editing deletion or creation actions; ";
            }

            //        . create plot point 5pts
            if (((double)storyStats[ACTION_COUNT_PP_ADDS]) > 0.1)
            {
                gradePointTotal += 5.0;
            }
            else
            {
                gradingComment += "No Plot Point creation actions; ";
            }

            //        . edit plot point 5pts
            if (((double)storyStats[ACTION_COUNT_PP_EDITS]) > 0.1)
            {
                gradePointTotal += 5.0;
            }
            else
            {
                gradingComment += "No Plot Point editing actions; ";
            }
            //        . delete plot point 2pts
            if (((double)storyStats[ACTION_COUNT_PP_DELETES]) > 0.1)
            {
                gradePointTotal += 2.0;
            }
            else
            {
                gradingComment += "No Plot Point deletion actions; ";
            }
            //    . Interactive Actions 5pts
            if (((double)storyStats[STORY_INTERACTIVE_ACTIONS]) > 0.1)
            {
                gradePointTotal += 5.0;
            }
            else
            {
                gradingComment += "No Interactive actions; ";
            }

            storyStats[GRADING_POINTS] = gradePointTotal;
            storyStats[GRADING_COMMENT] = gradingComment;
        }
        private static string ConvertToCSV(List<Hashtable> analysisData)
        {

            StringBuilder outputData = new StringBuilder();
            List<string> sortedFieldStrings = new List<string>();

            if(analysisData.Count == 0)
            {
                return "";
            }

            
            //Make sorted list of fields from first stat record
            foreach (DictionaryEntry statElement in analysisData[0]) 
            {
                sortedFieldStrings.Add((string)statElement.Key);
               
            }
            
            sortedFieldStrings.Sort(StringComparer.Ordinal);

            //Output sorted field header list
            bool firstField = true;
            foreach (string fieldName in sortedFieldStrings)
            {
                if (firstField)
                {
                    outputData.Append(fieldName);
                    firstField = false;
                }
                else
                {
                    outputData.Append("," + fieldName);
                }
            }
            outputData.AppendLine();


            // Output each field, in order, for every set of story stats
            foreach(Hashtable storyStats in analysisData)
            {
                firstField = true;
                foreach(string fieldName in sortedFieldStrings)
                {
                    if(firstField)
                    {
                        if (fieldName.Equals(FILE_NAME) || fieldName.Equals(GRADING_COMMENT))
                        {
                            //Filter out commas
                            outputData.Append(((string)storyStats[fieldName]).Replace(",", ""));
                        }
                        else
                        {
                            outputData.Append((double)storyStats[fieldName]);
                        }
                        firstField = false;
                    }
                    else 
                    {
                        if (fieldName.Equals(FILE_NAME) || fieldName.Equals(GRADING_COMMENT))
                        {
                            //Filter out commas
                            outputData.Append("," + ((string)storyStats[fieldName]).Replace(",", ""));
                        }
                        else
                        {
                            outputData.Append("," + (double)storyStats[fieldName]);
                        }

                     
                    }
                }
                outputData.AppendLine();
            }

            return outputData.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace WideRuled2
{
    public class Utilities
    {

     


        public static bool deleteFilesAndDirectories(string path) {
            
            if (path.ToLower().Equals("c:\\"))
            {
                return false;
            }
            
            DirectoryInfo di;

            try
            {
                di = new DirectoryInfo(path);
            }
            catch (System.Exception e)
            {
                return false;
            }


            if (!di.Exists)
            {
                return false;
            }



            FileInfo[] rgFiles = di.GetFiles();
            foreach (FileInfo fi in rgFiles)
            {

                if(fi.Name.StartsWith("."))
                {
                    //Skip file if it begins with '.', since it is meant to be 
                    //hidden (Wide Ruled does not generate these types of files)
                    continue;
                }
             
                if( ((fi.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) )
                {
                    //Skip file if it is hidden (Wide Ruled does not make hidden files)
                    continue;
                }

                try
                {
                    fi.Delete();
                }
                catch (System.UnauthorizedAccessException) { /* catch access denied error and fail silently */}
                catch (System.Exception e) {

                    throw new Exception("Error deleting old generated files: " + e.Message);
                }
        
            }

            DirectoryInfo[] subDirs = di.GetDirectories();
            foreach (DirectoryInfo dirInfo in subDirs) 
            {
                if (dirInfo.Name.StartsWith("."))
                {
                    //Skip directory if it begins with '.', since it is meant to be 
                    //hidden (Wide Ruled does not generate these types of directories)
                    continue;
                }

                if ( ((dirInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) )
                {
                    //Skip directory if it is hidden (Wide Ruled does not make hidden directories)
                    continue;
                }

                try 
                {
                    dirInfo.Delete(true);
                }
                catch (System.UnauthorizedAccessException) { /* catch access denied error and fail silently */}
                catch (System.Exception e) {

                    throw new Exception("Error deleting old generated directories: " + e.Message);
                }
            }

            return true;
        }

        public static void writeToFile(IndentingWriter contents, string path)
        {


            try
            {
                // create a writer and open the file
                TextWriter tw = new StreamWriter(path);

                // write a line of text to the file
                tw.Write(contents.ToString());

                // close the stream
                tw.Close();
            }
            catch (Exception e)
            {
                throw new Exception("Error writing generated ABL code: " + e.Message);
            }

        }

        public static void writeToFile(string contents, string path)
        {


            try
            {
                // create a writer and open the file
                TextWriter tw = new StreamWriter(path);

                // write a line of text to the file
                tw.Write(contents);

                // close the stream
                tw.Close();
            }
            catch (Exception e)
            {
                
            }

        }

        public static string getFileContents(string path)
        {
            try
            {
                // create reader & open file
                TextReader tr = new StreamReader(path);

                string result = tr.ReadToEnd();
                tr.Close();

                return result;
            }
            catch (Exception e)
            {
                return "//ERROR READING EXTERNAL FILE: " + e.Message;
            }

        }

        public static bool copyAllFiles(string sourceDir, string destinationDir)
        {
            DirectoryInfo diSource;
            DirectoryInfo diDestination;

            try
            {
                diSource = new DirectoryInfo(sourceDir);
            }
            catch (System.Exception e)
            {
                return false;
            }

            try
            {
                diDestination = new DirectoryInfo(destinationDir);
            }
            catch (System.Exception e)
            {
                return false;
            }

            if (!diSource.Exists || !diDestination.Exists)
            {
                return false;
            }

            FileInfo[] rgFiles = diSource.GetFiles();
            foreach (FileInfo fi in rgFiles)
            {
                try
                {
                    fi.CopyTo(destinationDir + "/" + fi.Name, true);
                }
                catch (System.Exception e) { }

            }

            return true;
        }

        public static void SerializeStoryDataToDisk(string filename, StoryData data) {
            

            Stream stream = File.Open(filename, FileMode.Create);
            BinaryFormatter bformatter = new BinaryFormatter();

            bformatter.Serialize(stream, data);
            stream.Close();

 
        }

        public static void SaveStringToDisk(string filename, string data)
        {
            // using block also closes stream automatically
            using (StreamWriter sw = new StreamWriter(filename))
            {
                // Put data into file
                sw.Write(data);
            }


        }

        public static StoryData DeSerializeStoryDataFromDisk(string filename) {

            StoryData result = null;
            Stream stream = File.Open(filename, FileMode.Open);
            BinaryFormatter bformatter = new BinaryFormatter();
            try
            {
                           
                result = (StoryData)bformatter.Deserialize(stream);
            }
            finally 
            {
                stream.Close();
            }

          

            return result;

        }
        public static void SavePPTypeTraits(PlotPointType currPPType, List<Trait> newList, StoryData world)
        {


            List<Trait> properList = new List<Trait>();

            foreach (Trait newTrait in newList)
            {
               // Trait oldTrait = currPPType.getTraitByTypeId(newTrait.TypeId);
               // if (oldTrait == null)
               // {

                    Trait convertedTrait = new Trait(newTrait, world);

                    Trait oldTrait = currPPType.findTraitByName(newTrait.Name);
                    if ((oldTrait != null) && (oldTrait.Type == newTrait.Type))
                    {
                        convertedTrait.Value = oldTrait.Value;
                    }


                    properList.Add(convertedTrait);

                   // properList.Add(new Trait(newTrait, world));

               // }
               // else
               // {
                   // oldTrait.Name = newTrait.Name;
                    //oldTrait.Type = newTrait.Type;
                    //properList.Add(oldTrait);
                //}
            }
            currPPType.Traits = properList;

        }

        public static void SaveAuthorGoalParams(AuthorGoal currGoal, List<Parameter> newList, StoryData world)
        {


            List<Parameter> properList = new List<Parameter>();

            foreach (Parameter newParam in newList)
            {
               // Parameter oldParam = currGoal.getParameterByTypeId(newParam.TypeId);
               // if (oldParam == null)
               // {

                Parameter convertedParam = new Parameter(newParam, world);

                Parameter oldParam = currGoal.findParameterByName(newParam.Name);
                if ((oldParam != null) && (oldParam.Type == newParam.Type))
                {
                    convertedParam.Value = oldParam.Value;
                }


                properList.Add(convertedParam);

                  //  properList.Add(newParam);

               // }
               // else
               // {
                   // oldParam.Name = newParam.Name;
                   // oldParam.Type = newParam.Type;
                    //properList.Add(oldParam);
                //}
            }
            currGoal.Parameters = properList;

        }

        public static void SynchronizeEntityTraits(List<Character> entities, List<Trait> newList, StoryData world)
        {
            foreach (Character currEntity in entities)
            {
                List<Trait> properList = new List<Trait>();

                foreach(Trait newTrait in newList)
                {
                   // Trait oldTrait = currEntity.getTraitByTypeId(newTrait.TypeId);
                   // if(oldTrait == null)
                    //{
                        Trait convertedTrait = new Trait(newTrait, world);

                        Trait oldTrait = currEntity.findTraitByName(newTrait.Name);
                        if((oldTrait != null) && (oldTrait.Type == newTrait.Type))
                        {
                            convertedTrait.Value = oldTrait.Value;
                        }

                        
                        properList.Add(convertedTrait);
                  // /     
                   // }
                   // else
                   // {
                     //   oldTrait.Name = newTrait.Name;
                     //   oldTrait.Type = newTrait.Type;
                       // properList.Add(oldTrait);
                   // }
                }
                currEntity.Traits = properList;
            }

        }

        public static void SynchronizeEntityRelationships(List<Character> entities, List<Relationship> newList, StoryData world)
        {
            foreach (Character currEntity in entities)
            {
                List<Relationship> properList = new List<Relationship>();

                foreach (Relationship newRel in newList)
                {
                   // Relationship oldRel = currEntity.getRelationshipByTypeId(newRel.TypeId);
                   // if (oldRel == null)
                    //{
                        Relationship convertedRel = new Relationship(newRel, currEntity, world);

                        Relationship oldRel = currEntity.findRelationshipByName(newRel.Name);
                        if (oldRel != null)
                        {
                            convertedRel.ToId = oldRel.ToId;
                            convertedRel.Strength = oldRel.Strength;
                        }


                        properList.Add(convertedRel);
                   // }
                   // else
                    //{
                      //  oldRel.Name = newRel.Name;
                       // properList.Add(oldRel);
                    //}
                }
                currEntity.Relationships = properList;
            }

        }

        public static List<Character> getGlobalCharacterList(StoryData world)
        {
            List<Character> fullCharList = new List<Character>();
            fullCharList.AddRange(world.Characters);

            //Go get all new characters created in plot fragment actions
            foreach (AuthorGoal goal in world.AuthorGoals)
            {
                foreach (PlotFragment frag in goal.PlotFragments)
                {
                    foreach (Action act in frag.Actions)
                    {
                        if (act is ActionCreateCharacter)
                        {
                            fullCharList.Add(((ActionCreateCharacter)act).NewCharacter);
                        }
                    }
                }
            }

            return fullCharList;

        }


        public static List<Environment> getGlobalEnvironmentList(StoryData world)
        {
            List<Environment> fullEnvList = new List<Environment>();
            fullEnvList.AddRange(world.Environments);

            //Go get all new characters created in plot fragment actions
            foreach (AuthorGoal goal in world.AuthorGoals)
            {
                foreach (PlotFragment frag in goal.PlotFragments)
                {
                    foreach (Action act in frag.Actions)
                    {
                        if (act is ActionCreateEnvironment)
                        {
                            fullEnvList.Add(((ActionCreateEnvironment)act).NewEnvironment);
                        }
                    }
                }
            }

            return fullEnvList;

        }

        public static List<PlotPoint> getGlobalPlotPointList(PlotPointType type, StoryData world)
        {
            List<PlotPoint> fullPPList = new List<PlotPoint>();
        

            //Go get all new characters created in plot fragment actions
            foreach (AuthorGoal goal in world.AuthorGoals)
            {
                foreach (PlotFragment frag in goal.PlotFragments)
                {
                    foreach (Action act in frag.Actions)
                    {
                        if ((act is ActionCreatePlotPoint) && 
                            ((ActionCreatePlotPoint)act).NewPlotPoint.TypeId == type.Id)
                        {
                            fullPPList.Add(((ActionCreatePlotPoint)act).NewPlotPoint);
                        }
                    }
                }
            }

            return fullPPList;

        }





        public static void SynchronizeEntityTraits(List<Environment> entities, List<Trait> newList, StoryData world)
        {

            foreach (Environment currEntity in entities)
            {
                List<Trait> properList = new List<Trait>();

                foreach (Trait newTrait in newList)
                {
                   // Trait oldTrait = currEntity.getTraitByTypeId(newTrait.TypeId);
                    //if (oldTrait == null)
                   // {
                        Trait convertedTrait = new Trait(newTrait, world);

                        Trait oldTrait = currEntity.findTraitByName(newTrait.Name);
                        if ((oldTrait != null) && (oldTrait.Type == newTrait.Type))
                        {
                            convertedTrait.Value = oldTrait.Value;
                        }


                        properList.Add(convertedTrait);

                    //}
                   // else
                   // {
                       // oldTrait.Name = newTrait.Name;
                       // oldTrait.Type = newTrait.Type;
                        //properList.Add(oldTrait);
                    //}
                }
                currEntity.Traits = properList;
            }
        }

        public static void SynchronizeGlobalCharacterTraits(List<Trait> newList, StoryData world)
        {

            SynchronizeEntityTraits(getGlobalCharacterList(world), newList, world);

        }

        public static void SynchronizeGlobalEnvironmentTraits(List<Trait> newList, StoryData world)
        {

            SynchronizeEntityTraits(getGlobalEnvironmentList(world), newList, world);

        }
        public static void SynchronizeGlobalCharacterRelationships(List<Relationship> newList, StoryData world)
        {

            SynchronizeEntityRelationships(getGlobalCharacterList(world), newList, world);

        }

        public static void SynchronizeGlobalEnvironmentRelationships(List<Relationship> newList, StoryData world)
        {

            SynchronizeEntityRelationships(getGlobalEnvironmentList(world), newList, world);

        }



        public static void SynchronizeEntityRelationships(List<Environment> entities, List<Relationship> newList, StoryData world)
        {
            foreach (Environment currEntity in entities)
            {
                List<Relationship> properList = new List<Relationship>();

                foreach (Relationship newRel in newList)
                {
                   // Relationship oldRel = currEntity.getRelationshipByTypeId(newRel.TypeId);
                   // if (oldRel == null)
                    //{
                        Relationship convertedRel = new Relationship(newRel, currEntity, world);

                        Relationship oldRel = currEntity.findRelationshipByName(newRel.Name);
                        if (oldRel != null)
                        {
                            convertedRel.ToId = oldRel.ToId;
                            convertedRel.Strength = oldRel.Strength;
                        }

                        properList.Add(convertedRel);
                   // }
                   // else
                   // {
                        //oldRel.Name = newRel.Name;
                       // properList.Add(oldRel);
                    //}
                }
                currEntity.Relationships = properList;
            }

        }



        public static void SynchronizeGlobalPlotPointsWithType(PlotPointType type, StoryData world)
        {
            List<PlotPoint> globalPlotPoints = getGlobalPlotPointList(type, world);

            foreach (PlotPoint currEntity in globalPlotPoints)
            {
                List<Trait> properList = new List<Trait>();

                foreach (Trait newTrait in type.Traits)
                {
                   // Trait oldTrait = currEntity.getTraitByTypeId(newTrait.TypeId);
                   // if (oldTrait == null)
                   // {

                        Trait convertedTrait = new Trait(newTrait, world);

                        Trait oldTrait = currEntity.findTraitByName(newTrait.Name);
                        if ((oldTrait != null) && (oldTrait.Type == newTrait.Type))
                        {
                            convertedTrait.Value = oldTrait.Value;
                        }


                        properList.Add(convertedTrait);

                   // }
                   // else
                   // {
                      //  oldTrait.Name = newTrait.Name;
                       // oldTrait.Type = newTrait.Type;
                       // properList.Add(oldTrait);
                    //}
                }
                currEntity.Traits = properList;
            }


        }

        public static void MakeErrorDialog(string displayText, Window owner)
        {

            MakeErrorDialog(displayText, "Error", owner);
        }

        public static void MakeErrorDialog(string displayText, string header, Window owner)
        {
           
           
            MessageBox.Show(owner, displayText, header, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static string MakeTextDialog(string displayText, Window owner)
        {

            WindowTextInputDialog newDialog = new WindowTextInputDialog(displayText);
            newDialog.Owner = owner;
            newDialog.ShowDialog();

            Nullable<bool> result = newDialog.DialogResult;
            if(result == true)
            {
                return newDialog.ResultText;
            }
            return null;


        }

        public static string MakeConstrainedTextDialog(string displayText, string constraintErrorText, List<string> constraints, Window owner)
        {

            WindowTextInputDialog newDialog = new WindowTextInputDialog(displayText, constraintErrorText, constraints);
            newDialog.Owner = owner;
            newDialog.ShowDialog();

            Nullable<bool> result = newDialog.DialogResult;
            if (result == true)
            {
                return newDialog.ResultText;
            }
            return null;


        }

        public static int MakeListChoiceDialog(string displayText, List<string> choices, Window owner)
        {

            WindowChooseActionDialog newWin = new WindowChooseActionDialog(displayText, choices);
            newWin.Owner = owner;
            newWin.ShowDialog();

            return newWin.ResultIndex;

        }
        public static MessageBoxResult MakeYesNoWarningDialog(string displayText, string headerText, Window owner)
        {
            // Configure the message box to be displayed
            string messageBoxText = displayText;
            string caption = headerText;
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;

            return MessageBox.Show(owner, messageBoxText, caption, button, icon);
        }

        public static void SynchronizeTwoCharacters(Character oldChar, Character newChar, StoryData world)
        {

            List<Character> characterSyncList = new List<Character>();
            characterSyncList.Add(newChar);

            Utilities.SynchronizeEntityRelationships(characterSyncList, oldChar.Relationships, world);
            Utilities.SynchronizeEntityTraits(characterSyncList, oldChar.Traits, world);
        }

        public static void SynchronizeTwoEnvironments(Environment oldEnv, Environment newEnv, StoryData world)
        {

            List<Environment> envSyncList = new List<Environment>();
            envSyncList.Add(newEnv);

            Utilities.SynchronizeEntityRelationships(envSyncList, oldEnv.Relationships, world);
            Utilities.SynchronizeEntityTraits(envSyncList, oldEnv.Traits, world);
        }


        public static void SynchronizePlotPointWithType(PlotPointType ppType, PlotPoint pp, StoryData world)
        {

 
            List<Trait> properList = new List<Trait>();

            foreach (Trait newTrait in ppType.Traits)
            {
               // Trait oldTrait = pp.getTraitByTypeId(newTrait.TypeId);
               // if (oldTrait == null)
                //{

                    Trait convertedTrait = new Trait(newTrait, world);

                    Trait oldTrait = pp.findTraitByName(newTrait.Name);
                    if ((oldTrait != null) && (oldTrait.Type == newTrait.Type))
                    {
                        convertedTrait.Value = oldTrait.Value;
                    }


                    properList.Add(convertedTrait);


                   // properList.Add(new Trait(newTrait, world));

               // }
               // else
                //{
                   // oldTrait.Name = newTrait.Name;
                   // oldTrait.Type = newTrait.Type;
                    //properList.Add(oldTrait);
                //}
            }
            pp.Traits = properList;

        }

        public static void removeRelationshipTargetReferencesFromStoryWorld(Character characterToWipe, StoryData world)
        {

            List<Character> globalChars = Utilities.getGlobalCharacterList(world);
            foreach (Character ch in globalChars)
            {
                foreach(Relationship rel in ch.Relationships)
                {
                    if(rel.ToId == characterToWipe.Id)
                    {
                        rel.ToId = StoryData.NullCharacterId;
                    }
                }
            }
        }

        public static void removeRelationshipTargetReferencesFromStoryWorld(Environment envToWipe, StoryData world)
        {

            List<Environment> globalEnvs = Utilities.getGlobalEnvironmentList(world);
            foreach (Environment env in globalEnvs)
            {
                foreach (Relationship rel in env.Relationships)
                {
                    if(rel.ToId == envToWipe.Id)
                    {
                        rel.ToId = StoryData.NullEnvironmentId;
                    }
                }
            }
        }

        public static Relationship findRelationshipByName(List<Relationship> inputList, string name)
        {
            foreach (Relationship relItem in inputList)
            {
                if (relItem.Name.Equals(name))
                {
                    return relItem;
                }
            }
            return null;
        }

        public static Trait findTraitByName(List<Trait> inputList, string name)
        {
            foreach (Trait traitItem in inputList)
            {
                if (traitItem.Name.Equals(name))
                {
                    return traitItem;
                }
            }
            return null;
        }

        public static Trait findTraitByNameAndType(List<Trait> inputList, string name, TraitDataType type)
        {
            foreach (Trait traitItem in inputList)
            {
                if (traitItem.Name.Equals(name) && (traitItem.Type == type))
                {
                    return traitItem;
                }
            }
            return null;
        }
  

        public static Parameter findParameterByName(List<Parameter> inputList, string name)
        {
            foreach (Parameter param in inputList)
            {
                if(param.Name.Equals(name))
                {
                    return param;
                }
            }
            return null;
        }

        public static Parameter findParameterByNameAndType(List<Parameter> inputList, string name, TraitDataType type)
        {
            foreach (Parameter param in inputList)
            {
                if ( param.Name.Equals(name) && (param.Type == type))
                {
                    return param;
                }
            }
            return null;
        }
       

        #region Object Moving Crap
        public static void MoveItemUp(List<Character> items, Object targetItem)
        {
            //Total hack way to convert a list of something to a list of objects,
            //and obviously introduces annoying additional constructors every time
            //it's called.

            List<Object> tempList = new List<Object>();
            foreach(Object item in items)
            {
                tempList.Add(item);
            }
            MoveItemUp(tempList, targetItem);
            items.Clear();
            foreach(Object item in tempList)
            {
                items.Add((Character)item);
            }
        }

        public static void MoveItemDown(List<Character> items, Object targetItem)
        {
            //Total hack way to convert a list of something to a list of objects,
            //and obviously introduces annoying additional constructors every time
            //it's called.

            List<Object> tempList = new List<Object>();
            foreach (Object item in items)
            {
                tempList.Add(item);
            }
            MoveItemDown(tempList, targetItem);
            items.Clear();
            foreach (Object item in tempList)
            {
                items.Add((Character)item);
            }
        }


        public static void MoveItemUp(List<Environment> items, Object targetItem)
        {
            //Total hack way to convert a list of something to a list of objects,
            //and obviously introduces annoying additional constructors every time
            //it's called.

            List<Object> tempList = new List<Object>();
            foreach (Object item in items)
            {
                tempList.Add(item);
            }
            MoveItemUp(tempList, targetItem);
            items.Clear();
            foreach (Object item in tempList)
            {
                items.Add((Environment)item);
            }
        }

        public static void MoveItemDown(List<Environment> items, Object targetItem)
        {
            //Total hack way to convert a list of something to a list of objects,
            //and obviously introduces annoying additional constructors every time
            //it's called.

            List<Object> tempList = new List<Object>();
            foreach (Object item in items)
            {
                tempList.Add(item);
            }
            MoveItemDown(tempList, targetItem);
            items.Clear();
            foreach (Object item in tempList)
            {
                items.Add((Environment)item);
            }
        }

        public static void MoveItemUp(List<PlotPointType> items, Object targetItem)
        {
            //Total hack way to convert a list of something to a list of objects,
            //and obviously introduces annoying additional constructors every time
            //it's called.

            List<Object> tempList = new List<Object>();
            foreach (Object item in items)
            {
                tempList.Add(item);
            }
            MoveItemUp(tempList, targetItem);
            items.Clear();
            foreach (Object item in tempList)
            {
                items.Add((PlotPointType)item);
            }
        }

        public static void MoveItemDown(List<PlotPointType> items, Object targetItem)
        {
            //Total hack way to convert a list of something to a list of objects,
            //and obviously introduces annoying additional constructors every time
            //it's called.

            List<Object> tempList = new List<Object>();
            foreach (Object item in items)
            {
                tempList.Add(item);
            }
            MoveItemDown(tempList, targetItem);
            items.Clear();
            foreach (Object item in tempList)
            {
                items.Add((PlotPointType)item);
            }
        }

        public static void MoveItemUp(List<AuthorGoal> items, Object targetItem)
        {
            //Total hack way to convert a list of something to a list of objects,
            //and obviously introduces annoying additional constructors every time
            //it's called.

            List<Object> tempList = new List<Object>();
            foreach (Object item in items)
            {
                tempList.Add(item);
            }
            MoveItemUp(tempList, targetItem);
            items.Clear();
            foreach (Object item in tempList)
            {
                items.Add((AuthorGoal)item);
            }
        }

        public static void MoveItemDown(List<AuthorGoal> items, Object targetItem)
        {
            //Total hack way to convert a list of something to a list of objects,
            //and obviously introduces annoying additional constructors every time
            //it's called.

            List<Object> tempList = new List<Object>();
            foreach (Object item in items)
            {
                tempList.Add(item);
            }
            MoveItemDown(tempList, targetItem);
            items.Clear();
            foreach (Object item in tempList)
            {
                items.Add((AuthorGoal)item);
            }
        }


        public static void MoveItemUp(List<PlotFragment> items, Object targetItem)
        {
            //Total hack way to convert a list of something to a list of objects,
            //and obviously introduces annoying additional constructors every time
            //it's called.

            List<Object> tempList = new List<Object>();
            foreach (Object item in items)
            {
                tempList.Add(item);
            }
            MoveItemUp(tempList, targetItem);
            items.Clear();
            foreach (Object item in tempList)
            {
                items.Add((PlotFragment)item);
            }
        }

        public static void MoveItemDown(List<PlotFragment> items, Object targetItem)
        {
            //Total hack way to convert a list of something to a list of objects,
            //and obviously introduces annoying additional constructors every time
            //it's called.

            List<Object> tempList = new List<Object>();
            foreach (Object item in items)
            {
                tempList.Add(item);
            }
            MoveItemDown(tempList, targetItem);
            items.Clear();
            foreach (Object item in tempList)
            {
                items.Add((PlotFragment)item);
            }
        }
        public static void MoveItemUp(List<PreconditionStatement> items, Object targetItem)
        {
            //Total hack way to convert a list of something to a list of objects,
            //and obviously introduces annoying additional constructors every time
            //it's called.

            List<Object> tempList = new List<Object>();
            foreach (Object item in items)
            {
                tempList.Add(item);
            }
            MoveItemUp(tempList, targetItem);
            items.Clear();
            foreach (Object item in tempList)
            {
                items.Add((PreconditionStatement)item);
            }
        }

        public static void MoveItemDown(List<PreconditionStatement> items, Object targetItem)
        {
            //Total hack way to convert a list of something to a list of objects,
            //and obviously introduces annoying additional constructors every time
            //it's called.

            List<Object> tempList = new List<Object>();
            foreach (Object item in items)
            {
                tempList.Add(item);
            }
            MoveItemDown(tempList, targetItem);
            items.Clear();
            foreach (Object item in tempList)
            {
                items.Add((PreconditionStatement)item);
            }
        }

        public static void MoveItemUp(List<Action> items, Object targetItem)
        {
            //Total hack way to convert a list of something to a list of objects,
            //and obviously introduces annoying additional constructors every time
            //it's called.

            List<Object> tempList = new List<Object>();
            foreach (Object item in items)
            {
                tempList.Add(item);
            }
            MoveItemUp(tempList, targetItem);
            items.Clear();
            foreach (Object item in tempList)
            {
                items.Add((Action)item);
            }
        }

        public static void MoveItemDown(List<Action> items, Object targetItem)
        {
            //Total hack way to convert a list of something to a list of objects,
            //and obviously introduces annoying additional constructors every time
            //it's called.

            List<Object> tempList = new List<Object>();
            foreach (Object item in items)
            {
                tempList.Add(item);
            }
            MoveItemDown(tempList, targetItem);
            items.Clear();
            foreach (Object item in tempList)
            {
                items.Add((Action)item);
            }
        }

        public static void MoveItemUp(List<Object> items, Object targetItem)
        {

            int index = 0;
            foreach (Object obj in items)
            {
                if(obj == targetItem)
                {
                    break;
                }
                index++;
            }

            if (index == 0) { return; }

            Object tempObj = items[index - 1];
            items[index - 1] = targetItem;
            items[index] = tempObj;
        }


        public static void MoveItemDown(List<Object> items, Object targetItem)
        {

            int index = 0;
            int lastIndex = items.Count - 1;
            foreach (Object obj in items)
            {
                if (obj == targetItem)
                {
                    break;
                }
                index++;
            }

            if (index == lastIndex) { return; }

            Object tempObj = items[index + 1];
            items[index + 1] = targetItem;
            items[index] = tempObj;
        }
        #endregion

    }
}

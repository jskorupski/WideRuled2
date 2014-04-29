using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    public enum ObjectEditingMode {Trait, RelationshipTarget, RelationshipStrength};

    [Serializable()]
    public class ActionEditObject : Action
    {

        private string _varObjectName;
        private UInt64 _varObjectTypeId;
        private ObjectEditingMode _mode;
        private Parameter _newValue;  //for trait or relationship strength
        private Parameter _newTarget; //for relationship target (always bound to variable)

        //_newValue.Name => name of trait or relationship
        //_newValue.LiteralValueOrBoundVarName => literal value or variable bound value of trait or relationship strength
        //_newTarget.Name => name of relationship (should be same as _newValue.Name)
        //_newTarget.LiteralValueOrBoundVarName => character variable that is bound to new relationship

        public ActionEditObject(UInt64 parentPlotFragmentId, string varObjectName, UInt64 objectTypeId, ObjectEditingMode mode, StoryData world) :
            base(parentPlotFragmentId, world)
        {
            _varObjectName = varObjectName;
            _varObjectTypeId = objectTypeId;
            _mode = mode;


            //Find editing type
            if (objectTypeId == world.CharTypeId) //Character
            {
                _newValue = new Parameter("Name", TraitDataType.Text, false, world);
                _newTarget = new Parameter("", TraitDataType.Text, true, world);
            }
            else if (objectTypeId == world.EnvTypeId) //Environment
            {
                _newValue = new Parameter("Name", TraitDataType.Text, false, world);
                _newTarget = new Parameter("", TraitDataType.Text, true, world);
            }
            else //Plot Point
            {
                PlotPointType currType = world.findPlotPointTypeById(objectTypeId);

                _newValue = new Parameter(currType.Traits[0].Name, currType.Traits[0].Type, false, world);
                _newTarget = new Parameter("", TraitDataType.Text, true, world);
            }

        }

        public ObjectEditingMode Mode
        {
            get { return _mode; }
            set 
            {
                _mode = value;

                if (_mode == ObjectEditingMode.RelationshipStrength)
                {
                    _newValue.Type = TraitDataType.Number;
                }
                
            }    

        }
        public string VariableObjectName
        {
            get { return _varObjectName; }
            set { _varObjectName = value; }
        }
     
        public UInt64 ObjectTypeId
        {
            get{return _varObjectTypeId; }
            set{_varObjectTypeId = value;}
        }
        public Parameter NewValue
        {
            get { return _newValue; }
            set { _newValue = value; }
        }

        public Parameter NewTarget 
        {
            get { return _newTarget; }
            set { _newTarget = value; }
        }

        override public string Description
        {
             get
            {
                string desc = "";
                string typeString = "";
                if (_varObjectTypeId == StoryWorldDataProvider.getStoryData().CharTypeId)
                {
                    typeString = "Character";
                     

                }
                else if (_varObjectTypeId == StoryWorldDataProvider.getStoryData().EnvTypeId)
                {

                    typeString = "Environment";
                }
                else
                {
                    PlotPointType currType = StoryWorldDataProvider.getStoryData().findPlotPointTypeById(_varObjectTypeId);
                    if(currType != null)
                    {
                        typeString = currType.Description;
                    }
                    else
                    {
                        typeString = "*UNKNOWN PLOT POINT TYPE*";
                    }

                }

                desc = "Edit saved " + typeString + " \"" + _varObjectName + "\": ";

                if (_mode == ObjectEditingMode.Trait)
                {
                    if (_newValue.ValueIsBoundToVariable)
                    {
                       
                        desc += "set trait \"" + _newValue.Name + "\" to saved variable \"" + _newValue.LiteralValueOrBoundVarName + "\"";
                    }
                    else
                    {
                        desc += "set trait \"" + _newValue.Name + "\" to " + _newValue.LiteralValueOrBoundVarName;
                    }
                }
                else if (_mode == ObjectEditingMode.RelationshipStrength)
                {
                    if (_newValue.ValueIsBoundToVariable)
                    {

                        desc += "set strength of relationship \"" + _newValue.Name + "\" to saved variable \"" + _newValue.LiteralValueOrBoundVarName + "\"";
                    }
                    else
                    {
                        desc += "set strength of relationship \"" + _newValue.Name + "\" to " + _newValue.LiteralValueOrBoundVarName;
                    }
                }
                else if (_mode == ObjectEditingMode.RelationshipTarget)
                {
                    desc += "set target of relationship \"" + _newValue.Name + "\" to saved " + typeString + " variable \"" + _newTarget.LiteralValueOrBoundVarName + "\"";
                    
                }
                
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
            string objectTypeString = "";
            if (_varObjectTypeId == world.CharTypeId)
            {
                names = parentFrag.getPreviouslyBoundCharacterVarNames(this);
                objectTypeString = "Character";

            }
            else if (_varObjectTypeId == world.EnvTypeId)
            {

                names = parentFrag.getPreviouslyBoundEnvironmentVarNames(this);
                objectTypeString = "Environment";
            }
            else
            {
                PlotPointType currType = world.findPlotPointTypeById(_varObjectTypeId);
                if (currType != null)
                {
                    names = parentFrag.getPreviouslyBoundPlotPointTypeVarNames(currType, this);
                    objectTypeString = currType.Description;
                }
                else 
                {
                    throw new Exception("Edit Plot Point Action in Plot Fragment \"" + parentFrag.Name + "\" " +
                        "uses a Plot Point Type which no longer exists.");
                }

            }
            if (!(names.Contains(_varObjectName)))
            {
                throw new Exception("Edit Object Action in Plot Fragment \"" +
                        parentFrag.Name + "\" refers to saved " + objectTypeString + " \"" + _varObjectName +
                        "\", \nwhich has a different type or does not exist in any previous Author Goal parameters, precondition statements, or actions .");
            }

            //make sure variable for relationship target edit exists
            if ((_mode == ObjectEditingMode.RelationshipTarget) && !(names.Contains((string)_newTarget.LiteralValueOrBoundVarName)))
            {
                throw new Exception("Edit Object Action in Plot Fragment \"" +
                        parentFrag.Name + "\" refers to saved " + objectTypeString + " \"" + (string)_newTarget.LiteralValueOrBoundVarName +
                        "\", \nwhich has a different type or does not exist in any previous Author Goal parameters, precondition statements, or actions .");
            }


        


            //Check for bound primitive variables when saving trait or relationship strength
            if(
                ((_mode == ObjectEditingMode.RelationshipStrength) || (_mode == ObjectEditingMode.Trait)) &&
                (_newValue.ValueIsBoundToVariable == true)
              )
            {
                bool foundIt = false;
                foreach (Trait traitItem in previouslyBoundVars)
                {

                    if (
                        (traitItem.Name == (string)_newValue.LiteralValueOrBoundVarName) &&
                        (traitItem.Type == _newValue.Type)
                        )
                    {
                        foundIt = true;
                        break;
                    }

                }

                if (!foundIt)
                {
                    throw new Exception("Edit Object Action in Plot Fragment \"" +
                        parentFrag.Name + "\" refers to variable \"" + (string)_newValue.LiteralValueOrBoundVarName +
                        "\", \nwhich has a different type or does not exist in any previous Author Goal parameters, precondition statements, or actions .");
                }
            }

            //Check to make sure all trait or relationships exist on the object to be edited
            List<Trait> traitListToCompare = new List<Trait>();
            List<Relationship> relListToCompare = new List<Relationship>();

            if (_varObjectTypeId == world.CharTypeId)
            {
                if(world.Characters.Count > 0)
                {
                    traitListToCompare = world.Characters[0].Traits;
                    relListToCompare = world.Characters[0].Relationships;
                }

            }
            else if (_varObjectTypeId == world.EnvTypeId)
            {

                if (world.Environments.Count > 0)
                {
                    traitListToCompare = world.Environments[0].Traits;
                    relListToCompare = world.Environments[0].Relationships;
                }
            }
            else
            {
                PlotPointType currType = world.findPlotPointTypeById(_varObjectTypeId);
                if (currType != null)
                {
                    traitListToCompare = currType.Traits;
                }
                else
                {
                    throw new Exception("Edit Object Action in Plot Fragment \"" +
                        parentFrag.Name + "\" refers to a Plot Point type that no longer exists.");
                }

            }

            if(_mode == ObjectEditingMode.Trait)
            {
                //look for trait we are editing
                if (null == Utilities.findTraitByNameAndType(traitListToCompare, _newValue.Name, _newValue.Type))
                {
                    throw new Exception("Edit Object Action in Plot Fragment \"" +
                        parentFrag.Name + "\" refers to a trait with name \"" + 
                        _newValue.Name + "\" and type \"" + 
                        Trait.TraitDataTypeToString(_newValue.Type) + 
                        "\" that no longer exists.");
                }

            }
            else 
            {
                //look for relationship we are editing
                if (null == Utilities.findRelationshipByName(relListToCompare, _newValue.Name))
                {
                    throw new Exception("Edit Object Action in Plot Fragment \"" +
                        parentFrag.Name + "\" refers to a relationship with name \"" +
                        _newValue.Name + " that no longer exists.");
                }
            }
            
            //No new variables to add to the binding list, so we can just exit after all the error checks


        }

        #region ICloneable Members

        override public Object Clone()
        {
            ActionEditObject newClone = (ActionEditObject)MemberwiseClone();
            newClone.Id = StoryWorldDataProvider.getStoryData().getNewId();

            newClone.NewValue = (Parameter)_newValue.Clone();
            newClone.NewTarget = (Parameter)_newTarget.Clone();

            return newClone;
        }

        #endregion
    }
}

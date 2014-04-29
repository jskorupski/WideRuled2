using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WideRuled2
{

    [Serializable()]
    public class Parameter : Trait
    {

        private bool _valueIsBoundToVariable; //Use text value as variable name instead of literal string
       

        public Parameter(string name, TraitDataType type, bool valueIsBoundToVariable, StoryData world) :
            base(name, type, world.getNewId(), world)
        {
            _valueIsBoundToVariable = valueIsBoundToVariable;


        }
        public Parameter(Parameter anotherParameter, StoryData world) :
            base(anotherParameter, world)
        {
            _valueIsBoundToVariable = anotherParameter.ValueIsBoundToVariable;
        }
        public bool ValueIsBoundToVariable 
        {
            get { return _valueIsBoundToVariable; }
            set 
            {
                bool previous = _valueIsBoundToVariable;
                _valueIsBoundToVariable = value; 
                if((previous != value) && (!_valueIsBoundToVariable))
                {
                    _textValue = "";
                }

            }
        }
        override public TraitDataType Type
        {
            get { return _type; }
            set 
            { 
                _type = value;
                //_valueIsBoundToVariable = false;
                resetValue();

            }
        }

        public void resetValue()
        {
            _booleanValue = false;
            _textValue = "";
            _numValue = 0.0;
        }
        
        public string ParameterName
        {
            get { return Name;  }
            set { Name = value; }
        }

        public Object LiteralValueOrBoundVarName
        {
            get { 

                if(_valueIsBoundToVariable)
                {
                    return _textValue;
                }
                else
                {
                    return Value; 
                }

            }
            set {
                if (_valueIsBoundToVariable)
                {
                    _textValue = (string)value;
                }
                else
                {
                    Value = value;
                }
            }

        }

        #region ICloneable Members

        override public Object Clone()
        {
            Parameter newClone = (Parameter)MemberwiseClone();
            newClone.Id = StoryWorldDataProvider.getStoryData().getNewId();

            return newClone;
        }

        #endregion

    }
}

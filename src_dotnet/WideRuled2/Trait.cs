using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    public enum TraitDataType { Text, Number, TrueFalse };
   

    [Serializable()]
    public class Trait : ICloneable
    {

        public const string TextString = "Text";
        public const string NumberString = "Number";
        public const string TrueFalseString = "True/False";

        protected UInt64 _traitId;
        protected UInt64 _traitTypeId;

        protected string _name;
        protected TraitDataType _type;

        protected bool _booleanValue;
        protected double _numValue;
        protected string _textValue;


        public Trait(string name, TraitDataType type, Object Value, UInt64 traitTypeId, StoryData world) 
        {

            Constructor(name, type, Value, traitTypeId, world);

        }

        public Trait (string name, TraitDataType type, UInt64 traitTypeId, StoryData world)
        {
            switch (type)
            {
                case TraitDataType.TrueFalse:
                    Constructor(name, type, false, traitTypeId, world);
                    break;
                case TraitDataType.Number:
                    Constructor(name, type, 0.0, traitTypeId, world);
                    break;
                case TraitDataType.Text:
                    Constructor(name, type, "", traitTypeId, world);
                    break;
            }
            
        }

        public Trait(string name, UInt64 traitTypeId, StoryData world)
        {
            Constructor(name, TraitDataType.Text, "", traitTypeId, world);

        }

        private void Constructor(string name, TraitDataType type, Object Value, UInt64 traitTypeId, StoryData world)
        {
            if (world == null) 
            {
                _traitId = 0;
 
            }
            else {
                _traitId = world.getNewId();

            }
            _traitTypeId = traitTypeId;

            //default values of the traits, just in case the given value
            //is not valid
            _name = name;
            _type = type;
            _booleanValue = false;
            _numValue = 0.0;
            _textValue = "";

            Type valType = Value.GetType();

            if (valType == typeof(Boolean))
            {
                _booleanValue = (Boolean)Value;
            }
            else if (valType == typeof(Double))
            {
                _numValue = (Double)Value;
            }
            else if (valType == typeof(String))
            {
                _textValue = (String)Value;
            }  
        }

        // Copy trait and keep trait type id to identify it as the same trait elsewhere in the program
        public Trait(Trait anotherTrait, StoryData world)
        {
            switch (anotherTrait.Type)
            {
                case TraitDataType.TrueFalse:
                    Constructor(anotherTrait.Name, anotherTrait.Type, false, anotherTrait.TypeId, world);
                    break;
                case TraitDataType.Number:
                    Constructor(anotherTrait.Name, anotherTrait.Type, 0.0, anotherTrait.TypeId, world);
                    break;
                case TraitDataType.Text:
                    Constructor(anotherTrait.Name, anotherTrait.Type, "", anotherTrait.TypeId, world);
                    break;
            }

        }

        public UInt64 Id
        {
            get { return _traitId; }
            set { _traitId = value; }
        }

        public UInt64 TypeId
        {
            get { return _traitTypeId; }
            set { _traitTypeId = value; }
        }
            
        public string Name 
        {
            get { return _name; }
            set { _name = value; }
        }

        virtual public TraitDataType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public string Description
        {
            get { return Name + " (" + TypeString + ")"; }
        }

        public string TypeString
        {
            get
            {
                switch (_type)
                {
                    case TraitDataType.Number:
                        return NumberString;
                    case TraitDataType.TrueFalse:
                        return TrueFalseString;
                    case TraitDataType.Text:
                        return TextString;
                }
                return "";
            }
            set
            {
                if (value.Equals(NumberString))
                {
                    _type = TraitDataType.Number;
                }
                else if (value.Equals(TrueFalseString))
                {
                    _type = TraitDataType.TrueFalse;
                }
                else if (value.Equals(TextString))
                {
                    _type = TraitDataType.Text;
                }

            }
        }



        public object Value
        {
            get {
                object returnValue = _numValue;
                switch (_type)
                {
                    case TraitDataType.TrueFalse:
                        returnValue = _booleanValue;
                        break;
                    case TraitDataType.Number:
                        returnValue = _numValue;
                        break;
                    case TraitDataType.Text:
                        returnValue = _textValue;
                        break;
                }
                return returnValue;
            }
            set {
                if (value == null) throw new Exception("Trait value being set to null");

                Type valType = value.GetType();

                if (valType == typeof(Boolean))
                {
                    _booleanValue = (Boolean)value;
                }
                else if (valType == typeof(Double))
                {
                    _numValue = (Double)value;
                }
                else if (valType == typeof(String))
                {
                    _textValue = (String)value;
                }

            }
        }


        public static string TraitDataTypeToString(TraitDataType input)
        {
            switch (input)
            {
                case TraitDataType.Number:
                    return NumberString;
                case TraitDataType.TrueFalse:
                    return TrueFalseString;
                case TraitDataType.Text:
                    return TextString;
            }
            return "";

        }



        #region ICloneable Members

        virtual public Object Clone()
        {
            Trait newClone = (Trait)MemberwiseClone();
            newClone.Id = StoryWorldDataProvider.getStoryData().getNewId();

            return newClone;
        }

        #endregion
 

    }
}

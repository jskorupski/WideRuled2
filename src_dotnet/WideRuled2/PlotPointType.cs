using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    [Serializable()]
    public class PlotPointType
    {

        private UInt64 _ppTypeId;
        private string _name;

        private List<Trait> _traits;

        public PlotPointType(string name, StoryData world)
        {
            _name = name;
            _ppTypeId = world.getNewId();
 
            _traits = new List<Trait>();

        }

        public string Name
        {
  
            get { return _name; }
            set { _name = value; }
        }

        public UInt64 Id
        {
            get { return _ppTypeId; }
            set { _ppTypeId = value; }
        }

        public List<Trait> Traits 
        {
            get { return _traits; }
            set { _traits = value; }
        }

        public string Description
        {
            get { return Name + " Plot Point"; }
        }

        public Trait findTraitByName(string name) {

            foreach (Trait currTrait in _traits)
            {
                if(currTrait.Name.Equals(name))
                {
                    return currTrait;
                }
            }
            return null;
        }


        public Trait getTraitByTypeId(UInt64 typeId)
        {

            foreach (Trait currTrait in _traits)
            {

                if (currTrait.TypeId == typeId)
                {
                    return currTrait;
                }
            }

            return null;
        }
    }


}

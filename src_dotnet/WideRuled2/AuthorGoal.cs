using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    [Serializable()]
    public class AuthorGoal : DependencyHolder
    {     
        private UInt64 _goalId;
        private string _name;

        private List<Parameter> _parameters;
        private List<PlotFragment> _plotFragments;


        public AuthorGoal(string name, StoryData world) 
        {
                _name = name;
              _goalId = world.getNewId();
              _parameters = new List<Parameter>();
              _plotFragments = new List<PlotFragment>();
             
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Description
        {
            //Pretty print version of the author goal with listed parameters
            get 
            {
                StringBuilder newDesc = new StringBuilder();
                newDesc.Append(_name);

                
                

                int totalItems = _parameters.Count;

                if(totalItems == 0)
                {
                    return newDesc.ToString();
                }

                int count = 0;

                newDesc.Append(" (");
                foreach (Parameter item in _parameters)
                {
                    count++;
                    newDesc.Append(Trait.TraitDataTypeToString(item.Type) + " ");
                    newDesc.Append(item.Name);
                    if (count != totalItems)
                    {
                        newDesc.Append(", ");
                    }
                }
                newDesc.Append(")");
                return newDesc.ToString();
            }

        }

        public UInt64 Id
        {
            get { return _goalId; }
            set { _goalId = value; }

        }

        public List<Parameter> Parameters
        {

            get { return _parameters; }
            set { _parameters = value; }
        }

        public List<PlotFragment> PlotFragments
        {

            get { return _plotFragments; }
            set { _plotFragments = value; }
        }

        public Parameter findParameterByName(string name)
        {

            foreach (Parameter currParam in _parameters)
            {
                if (currParam.Name.Equals(name))
                {
                    return currParam;
                }
            }
            return null;
        }


        public Parameter getParameterByTypeId(UInt64 typeId) 
        {
  
            foreach (Parameter currParam in _parameters)
            {

                if (currParam.TypeId == typeId)
                {
                    return currParam;
                }
            }

            return null;
        }


        public void checkAndUpdateDependences(List<Trait> previouslyBoundVars, StoryData world)
        {
            //Convert to trait list first
            List<Trait> paramToTraitList = new List<Trait>();
            foreach(Parameter param in _parameters)
            {
                previouslyBoundVars.Add(param);
            }

            //previouslyBoundVars.AddRange(paramToTraitList);

            
            foreach(PlotFragment frag in _plotFragments)
            {
                //New variable list for each fragment (with common parameter variables from parent Author Goal)
                List<Trait> fragSpecificTraitList = new List<Trait>(previouslyBoundVars.Count);
                fragSpecificTraitList.AddRange(previouslyBoundVars);

                frag.checkAndUpdateDependences(fragSpecificTraitList, world);
            }
        }

    }
}

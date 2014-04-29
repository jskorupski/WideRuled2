using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    [Serializable()]
    public class PlotPoint : StoryEntity
    {


        

        //Plot Point is story entity without relationships

        public PlotPoint(PlotPointType type, StoryData world) :
            base("", world.getNewId(), type.Id, world)
        {

            _traits.Clear(); //Don't need name trait

            Utilities.SynchronizePlotPointWithType(type, this, world);
        }

        public override object Clone()
        {
            PlotPoint newClone = (PlotPoint)MemberwiseClone();
            newClone.Id = StoryWorldDataProvider.getStoryData().getNewId();

            newClone.Traits = new List<Trait>();
            newClone.Relationships = new List<Relationship>();

            foreach (Trait traitItem in _traits)
            {
                newClone.Traits.Add((Trait)traitItem.Clone());
            }

            //no relationships

         
            return newClone;
        }

    }
}

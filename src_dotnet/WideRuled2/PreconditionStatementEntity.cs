using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    [Serializable()]
    public class PreconditionStatementEntity : PreconditionStatement
    {



        public PreconditionStatementEntity(UInt64 parentPlotFragmentId, StoryData world) :
            base(parentPlotFragmentId, world)
        {
   
        }



    }
}

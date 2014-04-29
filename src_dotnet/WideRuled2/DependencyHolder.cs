using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    public interface DependencyHolder
    {
       

        void checkAndUpdateDependences(List<Trait> previouslyBoundVars, StoryData world);
    }
}

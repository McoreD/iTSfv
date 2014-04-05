using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iTSfvLib
{
    public abstract class Task
    {
        public int ProgressCurrent { get; set; }
        public int ProgressMax { get; set; }

        public abstract string Name { get; }
    }
}

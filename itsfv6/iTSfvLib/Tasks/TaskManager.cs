using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iTSfvLib
{
    public class TaskManager
    {
        public List<Task> TaskList = new List<Task>();

        public double GetProgressCurrent()
        {
            if (TaskList.Count > 0)
            {
                double dProgressCurr = 0.0;
                foreach (Task task in TaskList)
                {
                    if (task.ProgressMax > 0)
                    {
                        dProgressCurr += 100 * task.ProgressCurrent / task.ProgressMax;
                    }
                }
                return dProgressCurr / TaskList.Count;
            }
            return 0.0;
        }
    }
}

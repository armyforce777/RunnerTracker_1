using RunnerTracker.Core.Model;
using RunnerTracker.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunnerTracker.Core.Service
{
    public class RunnerTrackerService
    {
        private static RunnerTrackerRepository runnerTrackerRepository = new RunnerTrackerRepository();

        public List<RunnerTrackerMenu> GetMenuList()
        {
            return runnerTrackerRepository.GetMenuList();
        }

        public RunnerTrackerMenu GetMenuById(int id)
        {
            return runnerTrackerRepository.GetMenuById(id);
        }
    }
}

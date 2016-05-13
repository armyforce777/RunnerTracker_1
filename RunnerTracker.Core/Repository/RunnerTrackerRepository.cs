using RunnerTracker.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunnerTracker.Core.Repository
{
    public class RunnerTrackerRepository
    {
        private static List<RunnerTrackerMenu> RunnerTrackerMenuList = new List<RunnerTrackerMenu>()
        {
            new RunnerTrackerMenu()
            {
                Index = 1,
                RecordDate = DateTime.Parse("04/21/2016"),
                Details = "Ran 13 miles"
            },
            new RunnerTrackerMenu()
            {
                Index = 2,
                RecordDate = DateTime.Parse("04/22/2016"),
                Details = "Swam 4 miles and ran 30 mins"
            }
        };

        public List<RunnerTrackerMenu> GetMenuList()
        {
            IEnumerable<RunnerTrackerMenu> RunnerTrackerMenus = from RunnerTrackerMenu in RunnerTrackerMenuList
                                                                select RunnerTrackerMenu;
            return RunnerTrackerMenuList.ToList<RunnerTrackerMenu>();
        }


        public RunnerTrackerMenu GetMenuById(int menuId)
        {
            IEnumerable<RunnerTrackerMenu> RunnerTrackerMenus = from RunnerTrackerMenu in RunnerTrackerMenuList
                                                                where RunnerTrackerMenu.Index == menuId
                                                                select RunnerTrackerMenu;

            return RunnerTrackerMenus.FirstOrDefault();
        }
    }
}

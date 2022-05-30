using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamnote.Relational;
using Streamnote.Relational.Data;
using Streamnote.Relational.Models;
using Streamnote.Relational.Models.Descriptors;
using Streamnote.Relational.Models.Descriptors.Analytics;

namespace Streamnote.Web.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly ApplicationDbContext Context;
        private readonly UserManager<ApplicationUser> UserManager;

        public AnalyticsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            Context = context;
            UserManager = userManager;
        }

        public async Task<IActionResult> View()
        {
            
            return View("View");
        }


        // POST: Step/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> GetTaskData()
        {
            var user = await UserManager.GetUserAsync(User);

            var descriptor = new AnalyticsDescriptor()
            {
                TotalTaskAnalytics = new TaskAnalyticsDescriptor()
                {

                },
                TaskAnalyticsOverTime = new List<TaskAnalyticsDescriptor>()
            };

            var tasks = Context.Tasks.FromSqlRaw($"SELECT * FROM Tasks WHERE CreatedBy = {user.Id} OR OwnedByUsername = {user.UserName}");

            var lastDate = new DateTime().Date;
            var dailyAnalytics = new TaskAnalyticsDescriptor();

            foreach (var task in tasks)
            {


                var date = task.Created.Date;


                if (date != lastDate)
                {
                    descriptor.TaskAnalyticsOverTime.Add(dailyAnalytics);
                    dailyAnalytics = new TaskAnalyticsDescriptor();
                }

                descriptor.TotalTaskAnalytics.Count++;

                switch (task.Status)
                {
                    case TodoStatus.Unstarted:
                        descriptor.TotalTaskAnalytics.UnstartedCount++;
                        dailyAnalytics.UnstartedCount++;
                        break;
                    case TodoStatus.Started:
                        descriptor.TotalTaskAnalytics.StartedCount++;
                        dailyAnalytics.StartedCount++;
                        break;
                    case TodoStatus.Finished:
                        descriptor.TotalTaskAnalytics.FinishedCount++;
                        dailyAnalytics.FinishedCount++;
                        break;
                    case TodoStatus.Delivered:
                        descriptor.TotalTaskAnalytics.DeliveredCount++;
                        dailyAnalytics.DeliveredCount++;
                        break;
                    case TodoStatus.Accepted:
                        descriptor.TotalTaskAnalytics.AcceptedCount++;
                        dailyAnalytics.AcceptedCount++;
                        break;
                }
            }

            return PartialView("_Task", new TaskDescriptor()); // TODO: UPDATE THIS! 
        }
    }
}

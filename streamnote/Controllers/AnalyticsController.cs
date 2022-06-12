using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.DataClassification;
using Microsoft.EntityFrameworkCore;
using Streamnote.Relational;
using Streamnote.Relational.Data;
using Streamnote.Relational.Models;
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
                Charts = new List<ChartDescriptor>()
            };

            var tasks = Context.Tasks.Where(t => t.CreatedBy.Id == user.Id || t.OwnedByUsername == user.UserName);

            var lastDate = 0;
            var dailyAnalytics = new TaskAnalyticsDescriptor();
            var dailyAnalyticsOverTime = new System.Collections.Generic.List<TaskAnalyticsDescriptor>();

            var dateNow = DateTime.UtcNow;
            foreach (var task in await tasks.Where(t => t.Created > new DateTime(dateNow.Year, dateNow.Month, 1)).ToListAsync())
            {
                var date = task.Created.Day;

                if (date != lastDate)
                {
                    dailyAnalytics.Date = date.ToString();
                    dailyAnalyticsOverTime.Add(dailyAnalytics);
                    dailyAnalytics = new TaskAnalyticsDescriptor();
                    lastDate = date;
                }

                dailyAnalytics.Count++;

                switch (task.Status)
                {
                    case TodoStatus.Unstarted:
                        dailyAnalytics.UnstartedCount++;
                        break;
                    case TodoStatus.Started:
                        dailyAnalytics.StartedCount++;
                        break;
                    case TodoStatus.Finished:
                        dailyAnalytics.FinishedCount++;
                        break;
                    case TodoStatus.Delivered:
                        dailyAnalytics.DeliveredCount++;
                        break;
                    case TodoStatus.Accepted:
                        dailyAnalytics.AcceptedCount++;
                        break;
                }
            }

            var dailyAnalyticsChart = new ChartDescriptor()
            {
                Title = "Tasks Today",
                Type = "pie",
                ChartIdentifier = "taskPieChart", 
                XValues = new string[]
                {
                    TodoStatus.Unstarted.ToString(),
                    TodoStatus.Started.ToString(),
                    TodoStatus.Finished.ToString(),
                    TodoStatus.Delivered.ToString(),
                    TodoStatus.Accepted.ToString(),
                },
                Datasets = new ChartDataset[]
                {
                    new ChartDataset
                    {
                        Label = "asdf",
                        BackgroundColor = new string[]
                        {
                            "grey",
                            "red", 
                            "blue",
                            "green",
                            "chartreuse"
                        },
                        BorderColor = "transparent",
                        Data = new int[]
                        {
                            dailyAnalytics.UnstartedCount,
                            dailyAnalytics.StartedCount,
                            dailyAnalytics.FinishedCount,
                            dailyAnalytics.DeliveredCount,
                            dailyAnalytics.AcceptedCount,
                        },
                        Fill = false
                    }
                }
            };

            descriptor.Charts.Add(dailyAnalyticsChart);

            int days = DateTime.UtcNow.Day;
            var xValues = new string[] { };

            for (var i = 1; i <= days; i++)
            {
                var allValues = xValues.ToList();
                allValues.Add(i.ToString());
                xValues = allValues.ToArray();
            }

            var overtimeAnalyticsChart = new ChartDescriptor()
            {
                Title = "Tasks Overtime",
                Type = "line",
                ChartIdentifier = "taskLineChart",
                XValues = xValues,
                Datasets = new ChartDataset[]
                {
                    new()
                    {
                        Label = "Unstarted",
                        BackgroundColor = new string[]{},
                        BorderColor = "grey",
                        Data = new int[] {},
                        Fill = false
                    },
                    new ChartDataset
                    {
                        Label = "Started",
                        BackgroundColor = new string[]{},
                        BorderColor = "red",
                        Data = new int[] {},
                        Fill = false
                    },
                    new ChartDataset
                    {
                        Label = "Finished",
                        BackgroundColor = new string[]{},
                        BorderColor = "blue",
                        Data = new int[] {},
                        Fill = false
                    },
                    new ChartDataset
                    {
                        Label = "Delivered",
                        BackgroundColor = new string[]{},
                        BorderColor = "green",
                        Data = new int[] {},
                        Fill = false
                    },
                    new ChartDataset
                    {
                        Label = "Accepted",
                        BackgroundColor = new string[]{},
                        BorderColor = "chartreuse",
                        Data = new int[] {},
                        Fill = false
                    }
                },
                Colors = new string[]
                {
                    "#b91d47",
                    "#00aba9",
                    "#2b5797",
                    "#e8c3b9",
                    "#1e7145"
                }
            };

            foreach (var dateVal in xValues)
            {
                var day = dailyAnalyticsOverTime.FirstOrDefault(t => t.Date == dateVal);

                var unStarted = overtimeAnalyticsChart.Datasets[0].Data.ToList(); 
                var started = overtimeAnalyticsChart.Datasets[1].Data.ToList();  
                var finished = overtimeAnalyticsChart.Datasets[2].Data.ToList();
                var delivered = overtimeAnalyticsChart.Datasets[3].Data.ToList();
                var accepted = overtimeAnalyticsChart.Datasets[4].Data.ToList();
                 
                if (day == null)
                {        
                    unStarted.Add(overtimeAnalyticsChart.Datasets[0].Data.Length > 0 ? overtimeAnalyticsChart.Datasets[0].Data.Last() : 0);
                    started.Add(overtimeAnalyticsChart.Datasets[1].Data.Length > 0 ? overtimeAnalyticsChart.Datasets[1].Data.Last() : 0);
                    finished.Add(overtimeAnalyticsChart.Datasets[2].Data.Length > 0 ? overtimeAnalyticsChart.Datasets[2].Data.Last() : 0);
                    delivered.Add(overtimeAnalyticsChart.Datasets[3].Data.Length > 0 ? overtimeAnalyticsChart.Datasets[3].Data.Last() : 0);
                    accepted.Add(overtimeAnalyticsChart.Datasets[4].Data.Length > 0 ? overtimeAnalyticsChart.Datasets[4].Data.Last() : 0);
                }
                else
                {
                    unStarted.Add(overtimeAnalyticsChart.Datasets[0].Data.Length > 0 ? overtimeAnalyticsChart.Datasets[0].Data.Last() + day.UnstartedCount : 0 + day.UnstartedCount); 
                    started.Add(overtimeAnalyticsChart.Datasets[1].Data.Length > 0 ? overtimeAnalyticsChart.Datasets[1].Data.Last() + day.StartedCount : 0 + day.StartedCount);   
                    finished.Add(overtimeAnalyticsChart.Datasets[2].Data.Length > 0 ? overtimeAnalyticsChart.Datasets[2].Data.Last() + day.FinishedCount : 0 + day.FinishedCount);
                    delivered.Add(overtimeAnalyticsChart.Datasets[3].Data.Length > 0 ? overtimeAnalyticsChart.Datasets[3].Data.Last() + day.DeliveredCount : 0 + day.DeliveredCount);
                    accepted.Add(overtimeAnalyticsChart.Datasets[4].Data.Length > 0 ? overtimeAnalyticsChart.Datasets[4].Data.Last() + day.AcceptedCount : 0 + day.AcceptedCount);
                }

                overtimeAnalyticsChart.Datasets[0].Data = unStarted.ToArray();
                overtimeAnalyticsChart.Datasets[1].Data = started.ToArray();
                overtimeAnalyticsChart.Datasets[2].Data = finished.ToArray();
                overtimeAnalyticsChart.Datasets[3].Data = delivered.ToArray();
                overtimeAnalyticsChart.Datasets[4].Data = accepted.ToArray();
            }

            overtimeAnalyticsChart.XValues = xValues.ToArray();

            descriptor.Charts.Add(overtimeAnalyticsChart);

            return Json(descriptor);
        }
    }
}

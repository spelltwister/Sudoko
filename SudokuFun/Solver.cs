using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;

using Newtonsoft.Json;

using Sudoko;

namespace SudokuFun
{
    public static class Solver
    {
        private const string _queueConnKey = "queueConnection";
        private const string _solveMeQueueName = "solveme";
        private const string _solverWorkQueueName = "solverwork";
        private const string _solutionsTableName = "solutions";

        [FunctionName("SolveMe")]        
        public static async Task SolveMeAsync([QueueTrigger(_solveMeQueueName, Connection = _queueConnKey)]string boardData,
            [Queue(_solverWorkQueueName, Connection = _queueConnKey)] IAsyncCollector<string> workQueueCollector,
            TraceWriter log)
        {
            log.Info($"C# Queue trigger to SolveMe: `{boardData}`");

            var data = JsonConvert.DeserializeObject<int?[,]>(boardData);
            
            var workItem = new SolverWorkItem
            {
                BoardId = Guid.NewGuid().ToString("N"),
                BoardData = data
            };

            log.Info($"Work item created with id: `{workItem.BoardId}`");

            await workQueueCollector.AddAsync(JsonConvert.SerializeObject(workItem)).ConfigureAwait(false);

            log.Info($"Work item added with id: `{workItem.BoardId}`");
        }

        [FunctionName("SolverWork")]
        public static async Task SolverWorkAsync([QueueTrigger(_solverWorkQueueName, Connection = _queueConnKey)]string workItem,
            [Table(_solutionsTableName, Connection = _queueConnKey)] IAsyncCollector<SolutionRecord> solutionsCollector,
            [Queue(_solverWorkQueueName, Connection = _queueConnKey)] IAsyncCollector<SolverWorkItem> newWorkCollector,
            TraceWriter log)
        {
            log.Info($"C# Queue trigger to SolverWork: `{workItem}`");

            var work = JsonConvert.DeserializeObject<SolverWorkItem>(workItem);

            Grid workGrid = new Grid(work.BoardData);

            int row, col;
            if(!workGrid.TryFindUnassignedLocation(out row, out col))
            {
                await solutionsCollector.AddAsync(new SolutionRecord()
                {
                    BoardId = work.BoardId,
                    BoardDataJson = JsonConvert.SerializeObject(work.BoardData),
                    IsSolved = true
                }).ConfigureAwait(false);

                return;
            }

            List<Task> newWorkTasks = new List<Task>(workGrid.Size);
            List<Task> solutionsTasks = new List<Task>(workGrid.Size);
            for (int num = 1; num <= workGrid.Size; num++)
            {
                if(workGrid.TryAssign(row, col, num))
                {
                    newWorkTasks.Add(newWorkCollector.AddAsync(new SolverWorkItem
                    {
                        BoardId = work.BoardId,
                        BoardData = workGrid.Data
                    }));
                    workGrid.Unassign(row, col);
                }
                else
                {
                    solutionsTasks.Add(solutionsCollector.AddAsync(new SolutionRecord
                    {
                        BoardId = work.BoardId,
                        BoardDataJson = JsonConvert.SerializeObject(workGrid.Data),
                        IsSolved = false,
                        FailsOn = num
                    }));
                }
            }

            await Task.WhenAll(newWorkTasks.Concat(solutionsTasks)).ConfigureAwait(false);
        }
    }

    public class SolutionRecord : TableEntity
    {
        public SolutionRecord()
        {
            this.RowKey = $"{(DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks):D19}{Guid.NewGuid():N}";
        }

        /// <summary>
        /// Gets or sets the board id to which this solution applies
        /// </summary>
        public string BoardId
        {
            get { return this.PartitionKey; }
            set { this.PartitionKey = value; }
        }

        /// <summary>
        /// Gets or sets the JSON representation of the board
        /// </summary>
        public string BoardDataJson { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether this solution solves the
        /// board or identifies a constrain violation
        /// </summary>
        public bool IsSolved { get; set; }

        /// <summary>
        /// Gets or sets the value which would cause a constrain violation
        /// and therefore allows pruning of this branch
        /// </summary>
        public int? FailsOn { get; set; }
    }

    public class SolverWorkItem
    {
        public string BoardId { get; set; }
        public int?[,] BoardData { get; set; }
    }
}
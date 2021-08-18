using Quartz;
using Quartz.Impl;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Val
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            Valuta.Ptr().GetValInfo();
            Valuta.Ptr().GetCurrentVal();

            INI ini = new INI("settings.ini");
            string[] time = ini.Read("time", "ScheduleJobTime").Split(':');

            // Планировщик задач
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            await scheduler.Start();

            // Функция, которая будет выполняться
            IJobDetail job = JobBuilder.Create<GetValJob>()
                .WithIdentity("getVal", "group1")
                .Build();

            // Триггер, который срабатывает каждый день в определённое время
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("triggerVal", "group1")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(Int32.Parse(time[0]), Int32.Parse(time[1])))
                .Build();

            await scheduler.ScheduleJob(job, trigger);

            // Бесконечный цикл, чтобы окно не закрылось
            while (true)
            {
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                    break;

                Thread.Sleep(100);
            }

            await scheduler.Shutdown();

            Console.WriteLine("Press any key to close the application");
            Console.ReadKey();
        }

        public class GetValJob : IJob
        {
            public async Task Execute(IJobExecutionContext context)
            {
                Valuta.Ptr().GetValutaDaily();
            }
        }
    }
}

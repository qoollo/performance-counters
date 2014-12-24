using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Qoollo.PerformanceCounters;

namespace TestProject
{
    internal class PerfCounters: Qoollo.PerformanceCounters.PerfCountersContainer
    {
        private static TestSingleInstance _singleInstance = CreateNullCategoryWrapper<TestSingleInstance>();
        private static TestMultiInstance _multiInstance = CreateNullCategoryWrapper<TestMultiInstance>();

        public static TestSingleInstance TestSingle { get { return _singleInstance; } }
        public static TestMultiInstance TestMulti { get { return _multiInstance; } }


        [PerfCountersInitializationMethod]
        public static void Init(CategoryWrapper parent)
        {
            var intermediate = parent.CreateEmptySubCategory("PerfCounterTest", "description");
            _singleInstance = intermediate.CreateSubCategory<TestSingleInstance>();
            _multiInstance = intermediate.CreateSubCategory<TestMultiInstance>();

            //InitializeCountersInAssembly(intermediate, typeof(int).Assembly);
        }

        // =================

        public class TestSingleInstance: SingleInstanceCategoryWrapper
        {
            public TestSingleInstance()
                : base("SingleInstance", "For tests")
            {
            }

            protected override void AfterInit()
            {
                this.ResetAllCounters();
            }


            [Counter("Test ElapsedTimeCounter")]
            public ElapsedTimeCounter Elapsed { get; private set; }

            [Counter("Test NumberOfItemsCounter")]
            public NumberOfItemsCounter Count { get; private set; }

            [Counter("Test OperationsPerSecondCounter")]
            public OperationsPerSecondCounter OperationPerSec { get; private set; }

            [Counter("Test AverageCountCounter")]
            public AverageCountCounter Avg { get; private set; }

            [Counter("Test AverageTimeCounter")]
            public AverageTimeCounter AvgTime { get; private set; }

            [Counter("Test MomentTimeCounter")]
            public MomentTimeCounter MomentTime { get; private set; }
        }



        public class TestInstance: InstanceInMultiInstanceCategoryWrapper
        {
            protected override void AfterInit()
            {
                this.ResetAllCounters();
            }

            [Counter("Count")]
            public NumberOfItemsCounter Count { get; private set; }
        }

        public class TestMultiInstance : MultiInstanceCategoryWrapper<TestInstance>
        {
            public TestMultiInstance()
                : base("MultiInstance", "For tests")
            {
            }
        }
    }




    class Program
    {
        static void Main(string[] args)
        {
            var counterFactory = PerfCountersInstantiationFactory.CreateCounterFactoryFromAppConfig("PerfCountersConfigurationSection");
            PerfCounters.Init(counterFactory.CreateRootWrapper());
            counterFactory.InitAll();

            var momentTimer = PerfCounters.TestSingle.MomentTime.StartNew();


            for (int i = 0; i < 1000; i++)
                PerfCounters.TestSingle.Count.Increment();
            Console.WriteLine("Count = " + PerfCounters.TestSingle.Count.CurrentValue.ToString());



            for (int i = 0; i < 1000; i++)
            {
                Thread.Sleep(5);
                PerfCounters.TestSingle.OperationPerSec.OperationFinished();
            }
            Console.WriteLine("OperationPerSec = " + PerfCounters.TestSingle.OperationPerSec.CurrentValue.ToString());



            Random rnd = new Random();

            for (int i = 0; i < 1000; i++)
                PerfCounters.TestSingle.Avg.RegisterValue(rnd.Next(0, 100));
            Console.WriteLine("Avg = " + PerfCounters.TestSingle.Avg.CurrentValue.ToString());



            for (int i = 0; i < 1000; i++)
            {
                var timer = PerfCounters.TestSingle.AvgTime.StartNew();
                Thread.Sleep(rnd.Next(0, 10));
                timer.Complete();
            }
            Console.WriteLine("AvgTime = " + PerfCounters.TestSingle.AvgTime.CurrentValue.ToString());


            momentTimer.Complete();
            Console.WriteLine("MomentTime = " + PerfCounters.TestSingle.MomentTime.CurrentValue.ToString());
            Console.WriteLine("Elapsed = " + PerfCounters.TestSingle.Elapsed.CurrentValue.ToString());

            counterFactory.Dispose();




            for (int i = 0; i < 100000; i++)
                PerfCounters.TestMulti[rnd.Next(0, 10).ToString()].Count.Increment();

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Instance #" + i.ToString() + ": " + PerfCounters.TestMulti[i.ToString()].Count.CurrentValue.ToString());
                PerfCounters.TestMulti[i.ToString()].Remove();
            }

            Console.ReadLine();
        }
    }
}

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
        private static TestSingleInstanceCategory _singleInstance = CreateNullCategoryWrapper<TestSingleInstanceCategory>();
        private static TestMultiInstanceCategory _multiInstance = CreateNullCategoryWrapper<TestMultiInstanceCategory>();

        public static TestSingleInstanceCategory TestSingle { get { return _singleInstance; } }
        public static TestMultiInstanceCategory TestMulti { get { return _multiInstance; } }


        [PerfCountersInitializationMethod]
        public static void Init(CategoryWrapper parent)
        {
            var intermediate = parent.CreateEmptySubCategory("PerfCounterTest", "description");
            _singleInstance = intermediate.CreateSubCategory<TestSingleInstanceCategory>("SingleInstanceCategory", "for tests");
            _multiInstance = intermediate.CreateSubCategory<TestMultiInstanceCategory>();

            //InitializeCountersInAssembly(intermediate, typeof(int).Assembly);
        }

        // =================

        public class TestEmptyCategory: EmptyCategoryWrapper
        {
            public TestEmptyCategory() : base("EmptyCategory", "desc") { }
        }

        public class TestSingleInstanceCategory: SingleInstanceCategoryWrapper
        {
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

            [Counter("Test DeltaCounter")]
            public DeltaCounter Delta { get; private set; }
        }



        public class TestInstance: InstanceInMultiInstanceCategoryWrapper
        {
            protected override void AfterInit()
            {
                this.ResetAllCounters();
            }

            [Counter("Count")]
            public NumberOfItemsCounter Count { get; private set; }

            [Counter("Elapsed")]
            public ElapsedTimeCounter Elapsed { get; private set; }

            [Counter("OperationPerSec")]
            public OperationsPerSecondCounter OperationPerSec { get; private set; }

            [Counter("Avg")]
            public AverageCountCounter Avg { get; private set; }

            [Counter("AvgTime")]
            public AverageTimeCounter AvgTime { get; private set; }

            [Counter("MomentTime")]
            public MomentTimeCounter MomentTime { get; private set; }

            [Counter("Delta")]
            public DeltaCounter Delta { get; private set; }
        }

        public class TestMultiInstanceCategory : MultiInstanceCategoryWrapper<TestInstance>
        {
            public TestMultiInstanceCategory()
                : base("MultiInstanceCategory", "For tests")
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

            for (int i = 0; i < 100; i++)
            {
                PerfCounters.TestSingle.Delta.Increment();
                Thread.Sleep(50);

                if (i % 10 == 9)
                {
                    Console.WriteLine("Delta Current = " + PerfCounters.TestSingle.Delta.CurrentValue.ToString());
                    Console.WriteLine("Delta Measure = " + PerfCounters.TestSingle.Delta.Measure().ToString());
                }
            }
            Console.WriteLine("Delta Last = " + PerfCounters.TestSingle.Delta.CurrentValue.ToString());



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
            for (int i = 0; i < 1000; i++)
            {
                using (PerfCounters.TestSingle.AvgTime.StartNew())
                {
                    Thread.Sleep(rnd.Next(0, 10));
                }
            }
            Console.WriteLine("AvgTime = " + PerfCounters.TestSingle.AvgTime.CurrentValue.ToString());


            momentTimer.Complete();
            Console.WriteLine("MomentTime = " + PerfCounters.TestSingle.MomentTime.CurrentValue.ToString());
            Console.WriteLine("Elapsed = " + PerfCounters.TestSingle.Elapsed.CurrentValue.ToString());



            for (int i = 0; i < 100000; i++)
                PerfCounters.TestMulti[rnd.Next(0, 10).ToString()].Count.Increment();

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Instance #" + i.ToString() + ": " + PerfCounters.TestMulti[i.ToString()].Count.CurrentValue.ToString());
                PerfCounters.TestMulti[i.ToString()].Remove();
            }


            counterFactory.Dispose();

            Console.WriteLine("========= Completed =========");
            Console.ReadLine();
        }
    }
}

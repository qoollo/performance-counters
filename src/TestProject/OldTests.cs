using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Qoollo.PerformanceCounters;
using Qoollo.PerformanceCounters.WinCounters;
using Qoollo.PerformanceCounters.InternalCounters;

namespace TestProject
{
    [PerfCountersContainer]
    internal class AttributeCounters : SingleInstanceCategoryWrapper
    {
        #region Singleton

        public AttributeCounters()
            : base("TestAttributeProject", "Версия основанная на атрибутах")
        {
        }

        private static readonly object LockCreate = new object();
        private static volatile AttributeCounters _instance = null;

        public static AttributeCounters Instance
        {
            get
            {
                return _instance;
            }
        }

        [PerfCountersInitializationMethod]
        public static void Init(CategoryWrapper wrapper)
        {
            _instance = wrapper.CreateSubCategory<AttributeCounters>();
            MultyCategory.Init(_instance);
        }

        #endregion

        protected override void AfterInit()
        {
            this.ResetAllCounters();
        }


        [Counter("NumCntr111", "Числовой счетчик")]
        public NumberOfItemsCounter NumericalCounter { get; private set; }
        //public NumericalWinCounter RateCounter { get; set; }    // одинаковы
        //public NumericalWinCounter SampleCounter { get; set; }  // одинаковы

        [Counter("Временной счетчик")]
        public AverageTimeCounter AverageTimer32 { get; private set; }

        [Counter("Средний счетчик")]
        public AverageCountCounter AvgCount { get; private set; }

        [Counter("Операции")]
        public OperationsPerSecondCounter OpPerSec { get; private set; }

        [Counter]
        public ElapsedTimeCounter ElapsedTime { get; private set; }

        [Counter]
        public MomentTimeCounter MomentTime { get; private set; }
    }



    internal class MultyCategory : MultiInstanceCategoryWrapper<SingleCategory>
    {
        #region Singleton

        private static readonly object LockCreate = new object();
        private static volatile MultyCategory _instance;

        public MultyCategory()
            : base("TestAttributeMultyInstance", "Версия многоинстовой категории, основанная на атрибутах")
        {
        }

        public static MultyCategory Instance
        {
            get
            {
                return _instance;
            }
        }

        public static void Init(CategoryWrapper wrapper)
        {
            _instance = wrapper.CreateSubCategory<MultyCategory>();
        }

        #endregion
    }

    internal class SingleCategory : InstanceInMultiInstanceCategoryWrapper
    {
        protected override void AfterInit()
        {
            this.ResetAllCounters();
        }

        [Counter("Временной счетчик")]
        public NumberOfItemsCounter ItemCnt { get; set; }
    }



    //internal class Counters
    //{
    //    private static readonly object LockCreate = new object();
    //    private static volatile Counters _instance;

    //    private Counters()
    //    {
    //    }

    //    public static Counters Instance
    //    {
    //        get
    //        {
    //            if (_instance == null)
    //            {
    //                lock (LockCreate)
    //                {
    //                    if (_instance == null)
    //                    {
    //                        _instance = new Counters();
    //                    }
    //                }
    //            }

    //            return _instance;
    //        }
    //    }

    //    public void Inicialize()
    //    {
    //        // Создаем конфигурацию счетчиков, которыми мы хотим воспользоваться
    //        var category = WinFactory.Instance.CreateSingleInstanceSubCategory(null, "TestProject", null);

    //        Instance.NumericalCounter = category.CreateNumberOfItemsCounter("NumberOfItems32", null);
    //        //Instance.RateCounter = category.GetNumericalCounter("RateOfCountsPerSecond32", "", CounterTypes.RateOfCountsPerSecond32);
    //        //Instance.SampleCounter = category.GetNumericalCounter("SampleCounter", "", CounterTypes.SampleCounter);

    //        Instance.AverageTimer32 = category.CreateAverageTimeCounter("AverageTimer32", null);
    //        Instance.ElapsedTime = category.CreateElapsedTimeCounter("ElapsedTime", null);

    //        WinFactory.Instance.Init();
    //    }

    //    public INumericalCounter NumericalCounter { get; set; }
    //    //public NumericalWinCounter RateCounter { get; set; }    // одинаковы
    //    //public NumericalWinCounter SampleCounter { get; set; }  // одинаковы

    //    public ITimeCounterMultyThreadWrapper AverageTimer32 { get; set; }
    //    public IElapsedTimeCounter ElapsedTime { get; set; }
    //}

    /// <summary>
    /// Для нормального просмотра результатов нужно открыть Activity Monitor
    /// команда perfmon
    /// и добавить туда счетчики из категории TestProject
    /// </summary>
    class OldTests
    {
        static void CompareCounters()
        {
            var winCounters = new WinCounterFactory();
            var internalCounters = new InternalCounterFactory();

            var winOpPerSec = winCounters.CreateSingleInstanceCategory("testCat1111", "").CreateOperationsPerSecondCounter("opCntTest", "");
            var intOpPerSec = internalCounters.CreateSingleInstanceCategory("testCat1111", "").CreateOperationsPerSecondCounter("opCntTest", "");

            winCounters.InitAll();
            internalCounters.InitAll();


            Task.Run(() =>
            {
                Random rnd = new Random();
                Stopwatch sw = Stopwatch.StartNew();
                for (int i = 0; i < 1000000000; i++)
                {
                    //winOpPerSec.OperationFinished();
                    intOpPerSec.OperationFinished();

                    //while (sw.ElapsedMilliseconds < 100)
                    //    Thread.Sleep(0);

                    //sw.Restart();
                }
            });

            while (true)
            {
                double winVal = winOpPerSec.CurrentValue;
                double intVal = intOpPerSec.CurrentValue;

                Console.WriteLine("win = " + winVal.ToString() + ", int = " + intVal.ToString());
                Thread.Sleep(250);
            }
        }


        static void CompareCounters2()
        {
            var winCounters = new WinCounterFactory();
            var internalCounters = new InternalCounterFactory();

            var winAvCnt = winCounters.CreateSingleInstanceCategory("testCat2222", "").CreateAverageCountCounter("avCntTest", "");
            var intAvCnt = internalCounters.CreateSingleInstanceCategory("testCat2222", "").CreateAverageCountCounter("avCntTest", "");

            winCounters.InitAll();
            internalCounters.InitAll();


            Task.Run(() =>
            {
                Random rnd = new Random();
                Stopwatch sw = Stopwatch.StartNew();
                for (int i = 0; i < 1000000000; i++)
                {
                    if (i % 100 < 50)
                    {
                        winAvCnt.RegisterValue(i % 100);
                        intAvCnt.RegisterValue(i % 100);
                    }
                    else
                    {
                        winAvCnt.RegisterValue(100 - i % 100);
                        intAvCnt.RegisterValue(100 - i % 100);
                    }
                    //if (i % 2 == 0)
                    //{
                    //    winAvCnt.RegisterValue(1000);
                    //    intAvCnt.RegisterValue(1000);
                    //}
                    //else
                    //{
                    //    winAvCnt.RegisterValue(0);
                    //    intAvCnt.RegisterValue(0);
                    //}

                    Thread.Sleep(10);

                    //while (sw.ElapsedMilliseconds < 100)
                    //    Thread.Sleep(0);

                    //sw.Restart();
                }
            });

            while (true)
            {
                double winVal = winAvCnt.CurrentValue;
                double intVal = intAvCnt.CurrentValue;

                Console.WriteLine("win = " + winVal.ToString() + ", int = " + intVal.ToString());
                Thread.Sleep(250);
            }
        }

        public static long diff = 0;

        //[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private static bool Condition(int v)
        {
            //return (v % 4) == 0;
            return (v & 1) == 0;
        }

        private static void TestPerf()
        {
            long strt = Stopwatch.GetTimestamp();
            Stopwatch sw = Stopwatch.StartNew();

            for (int i = 0; i < 5000000; i++)
                diff = Stopwatch.GetTimestamp() - strt;

            sw.Stop();

            Console.WriteLine("Stopwatch = " + sw.ElapsedMilliseconds.ToString() + "ms");


            strt = (uint)Environment.TickCount;
            sw = Stopwatch.StartNew();

            for (int i = 0; i < 10000000; i++)
                diff = (uint)Environment.TickCount - strt;

            sw.Stop();

            Console.WriteLine("Environment.TickCount = " + sw.ElapsedMilliseconds.ToString() + "ms");

            strt = (uint)Environment.TickCount;
            sw = Stopwatch.StartNew();

            for (int i = 0; i < 10000000; i++)
            {
                if (Condition(i))
                    diff = (uint)Environment.TickCount - strt;
            }

            sw.Stop();

            Console.WriteLine("Environment.TickCount 2 = " + sw.ElapsedMilliseconds.ToString() + "ms");


            DateTime strtTm = DateTime.Now;
            sw = Stopwatch.StartNew();

            for (int i = 0; i < 5000000; i++)
                diff = (int)((DateTime.Now - strtTm).TotalMilliseconds);

            sw.Stop();

            Console.WriteLine("DateTime = " + sw.ElapsedMilliseconds.ToString() + "ms");

            Console.ReadLine();

        }


        public static void RunTest()
        {
            //TestPerf();
            //CompareCounters2();

            // Console.ReadLine();

            //var f = PerformanceCounterCategory.GetCategories();

            //CounterCreationDataCollection f = new CounterCreationDataCollection();
            //f.Add(new CounterCreationData("RawFraction", "RawFraction", PerformanceCounterType.RawFraction));
            //f.Add(new CounterCreationData("RawFractionBase", "RawFractionBase", PerformanceCounterType.RawBase));
            //var cat = PerformanceCounterCategory.Create("testCat3", "testCat3", f);

            //var perfCnt = new PerformanceCounter("testCat3", "RawFraction", false);
            //var perfCntBase = new PerformanceCounter("testCat3", "RawFractionBase", false);

            //double valDbl = 51.4;


            //perfCnt.RawValue = (long)(valDbl * 10000);
            //perfCntBase.RawValue = 100 * 10000;

            //for (int i = 0; i < 1000; i++)
            //{
            //    perfCnt.IncrementBy(10);
            //    Thread.Sleep(100);
            //}

            //return;


            //var rootFact = new Qoollo.PerformanceCounters.WinCounters.WinCounterFactory(Qoollo.PerformanceCounters.WinCounters.WinCountersInstantiationMode.UseExistedIfPossible);
            //var rootFact = new Qoollo.PerformanceCounters.InternalCounters.InternalCounterFactory();
            //var rootFact = Qoollo.PerformanceCounters.NullCounters.NullCounterFactory.Default;
            //var rootFact = new Qoollo.PerformanceCounters.CompositeCounters.CompositeCounterFactory(new CounterFactory[]
            //    {
            //        new Qoollo.PerformanceCounters.InternalCounters.InternalCounterFactory(),
            //        new Qoollo.PerformanceCounters.WinCounters.WinCounterFactory(Qoollo.PerformanceCounters.WinCounters.WinCountersInstantiationMode.UseExistedIfPossible)
            //    });

            var rootFact = PerfCountersInstantiationFactory.CreateCounterFactoryFromAppConfig("PerfCountersConfigurationSection");

            //var si = rootFact.CreateSingleInstanceCategory("aaa", "aaa");
            //var cntr = si.CreateNumberOfItemsCounter("asd", "asd");
            //cntr.Increment();

            //rootFact.InitAll();
            //cntr.SetValue(1000);

            AttributeCounters.Init(rootFact.CreateRootWrapper());
            rootFact.InitAll();


            var tmr = AttributeCounters.Instance.MomentTime.StartNew();
            var avg = AttributeCounters.Instance.AverageTimer32.StartNew();
            Thread.Sleep(1000);
            tmr.Complete();
            avg.Complete();

            Console.ReadLine();

            AttributeCounters.Instance.ElapsedTime.Reset();
            Thread.Sleep(10000);
            var tm = AttributeCounters.Instance.ElapsedTime.CurrentValue;
            tm = AttributeCounters.Instance.ElapsedTime.CurrentValue;
            tm = AttributeCounters.Instance.ElapsedTime.CurrentValue;
            AttributeCounters.Instance.ElapsedTime.Reset();
            var mv = TimeSpan.MaxValue;

            Stopwatch sw = Stopwatch.StartNew();

            for (int i = 0; i < 10000000; i++)
            {
                AttributeCounters.Instance.NumericalCounter.Increment();
                //Thread.Sleep(20);

                //var val = AttributeCounters.Instance.OpPerSec.CurrentValue;
                //if (val == -1)
                //    throw new Exception();
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);


            sw = Stopwatch.StartNew();
            for (int i = 0; i < 10000000; i++)
            {
                MultyCategory.Instance[(i % 10).ToString()].ItemCnt.IncrementBy(2);
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);

            for (int i = 0; i < 1000; i++)
            {
                AttributeCounters.Instance.OpPerSec.OperationFinished();
                Thread.Sleep(20);

                var val = AttributeCounters.Instance.OpPerSec.CurrentValue;
                if (val == -1)
                    throw new Exception();
            }

            Random rnd111 = new Random();

            //for (int i = 0; i < 100; i++)
            //{
            //    var timer = AttributeCounters.Instance.AverageTimer32.StartNew();
            //    Thread.Sleep(rnd111.Next(1000));
            //    timer.TimePoint();

            //    var val = AttributeCounters.Instance.AverageTimer32.CurrentValue;
            //    if (val == TimeSpan.MinValue)
            //        throw new Exception();
            //}

            //for (int i = 0; i < 1000; i++)
            //{
            //    AttributeCounters.Instance.AvgCount.RegisterValue(rnd111.Next(1000));
            //    Thread.Sleep(50);

            //    var val = AttributeCounters.Instance.AvgCount.CurrentValue;
            //    if (val == long.MinValue)
            //        throw new Exception();
            //}


            for (int cnt = 0; cnt < 4; cnt++)
            {
                int vvv = cnt;
                MultyCategory.Instance[vvv.ToString()].ItemCnt.Reset();
                Task.Run(() =>
                {
                    Random rnd = new Random(vvv * 10);
                    for (int i = 0; i < 500; i++)
                    {
                        MultyCategory.Instance[vvv.ToString()].ItemCnt.IncrementBy(rnd.Next(20));
                        Thread.Sleep(rnd.Next(100));
                    }
                });
            }

            Thread.Sleep(1000);

            MultyCategory.Instance["0"].Remove();


            Console.ReadLine();

            //rootFact.Dispose();

            //  CounterCreationDataCollection cntCol = new CounterCreationDataCollection();
            //  cntCol.Add(new CounterCreationData("c1", "c1", PerformanceCounterType.NumberOfItems32));
            //  cntCol.Add(new CounterCreationData("c2", "c2", PerformanceCounterType.NumberOfItems32));
            ////  PerformanceCounterCategory.Create("TestCat", "TestCat", cntCol);

            //  PerformanceCounter c1_i1 = new PerformanceCounter("TestCat", "c1", false);
            //  PerformanceCounter c1_i2 = new PerformanceCounter("TestCat", "c1", "inst2", false);

            //  c1_i1.Increment();
            //  c1_i2.Increment();

            //  Console.ReadLine();

            //  Counters.Instance.Inicialize();

            //  //Task.Factory.StartNew(Logic, TaskCreationOptions.LongRunning);
            //  Task.Factory.StartNew(AttributeCounterTest, TaskCreationOptions.LongRunning);
            //  Task.Factory.StartNew(MultyInstanceTest, TaskCreationOptions.LongRunning);

            //  Console.ReadLine();
        }

        //private static void Logic()
        //{
        //    Counters.Instance.ElapsedTime.Start();

        //    int min1 = 50;
        //    int max1 = 200;

        //    var tasks = new List<Task>
        //        {
        //            Task.Factory.StartNew(() => SimpleTest(min1, max1)),
        //            Task.Factory.StartNew(() => SimpleTest(min1, max1)),
        //            Task.Factory.StartNew(() => SimpleTest(min1, max1)),
        //            Task.Factory.StartNew(() => SimpleTest(min1, max1)),
        //            Task.Factory.StartNew(() => SimpleTest(min1, max1))
        //        };

        //    Task.WaitAll(tasks.ToArray());

        //    var elapsed = Counters.Instance.ElapsedTime.GetTimespan();
        //    var avarage = Counters.Instance.AverageTimer32.GetTimespan();
        //    Console.WriteLine();
        //    Console.WriteLine("Первый тест длился {0} minutes {1} seconds", elapsed.Minutes, elapsed.Seconds);
        //    Console.WriteLine("Время прохода последнего цикла {0} minutes {1} seconds {2} milliseconds", avarage.Minutes, avarage.Seconds, avarage.Milliseconds);
        //    Console.WriteLine();

        //    Counters.Instance.ElapsedTime.Stop();

        //    Counters.Instance.ElapsedTime.Restart();

        //    int min2 = 75;
        //    int max2 = 300;

        //    tasks.Add(Task.Factory.StartNew(() => SimpleTest(min2, max2)));
        //    tasks.Add(Task.Factory.StartNew(() => SimpleTest(min2, max2)));
        //    tasks.Add(Task.Factory.StartNew(() => SimpleTest(min2, max2)));

        //    Task.WaitAll(tasks.ToArray());

        //    Console.WriteLine();

        //    elapsed = Counters.Instance.ElapsedTime.GetTimespan();
        //    avarage = Counters.Instance.AverageTimer32.GetTimespan();


        //    Console.WriteLine("Второй тест длился {0} minutes {1} seconds", elapsed.Minutes, elapsed.Seconds);
        //    Console.WriteLine("Время прохода последнего прохода цикла {0} minutes {1} seconds {2} milliseconds", avarage.Minutes, avarage.Seconds, avarage.Milliseconds);

        //    Counters.Instance.ElapsedTime.Stop();
        //}

        //private static void AttributeCounterTest()
        //{
        //    AttributeCounters.Instance.ElapsedTime.Start();

        //    int min1 = 50;
        //    int max1 = 200;

        //    var tasks = new List<Task>
        //        {
        //            Task.Factory.StartNew(() => AttributeTest(min1, max1)),
        //            Task.Factory.StartNew(() => AttributeTest(min1, max1)),
        //            Task.Factory.StartNew(() => AttributeTest(min1, max1)),
        //            Task.Factory.StartNew(() => AttributeTest(min1, max1)),
        //            Task.Factory.StartNew(() => AttributeTest(min1, max1))
        //        };

        //    Task.WaitAll(tasks.ToArray());

        //    var elapsed = AttributeCounters.Instance.ElapsedTime.GetTimespan();
        //    var avarage = AttributeCounters.Instance.AverageTimer32.GetTimespan();
        //    Console.WriteLine();
        //    Console.WriteLine("Первый тест длился {0} minutes {1} seconds", elapsed.Minutes, elapsed.Seconds);
        //    Console.WriteLine("Время прохода последнего цикла {0} minutes {1} seconds {2} milliseconds", avarage.Minutes, avarage.Seconds, avarage.Milliseconds);
        //    Console.WriteLine();

        //    AttributeCounters.Instance.ElapsedTime.Stop();

        //    AttributeCounters.Instance.ElapsedTime.Restart();

        //    int min2 = 75;
        //    int max2 = 300;

        //    tasks.Add(Task.Factory.StartNew(() => AttributeTest(min2, max2)));
        //    tasks.Add(Task.Factory.StartNew(() => AttributeTest(min2, max2)));
        //    tasks.Add(Task.Factory.StartNew(() => AttributeTest(min2, max2)));

        //    Task.WaitAll(tasks.ToArray());

        //    Console.WriteLine();

        //    elapsed = AttributeCounters.Instance.ElapsedTime.GetTimespan();
        //    avarage = AttributeCounters.Instance.AverageTimer32.GetTimespan();


        //    Console.WriteLine("Второй тест длился {0} minutes {1} seconds", elapsed.Minutes, elapsed.Seconds);
        //    Console.WriteLine("Время прохода последнего прохода цикла {0} minutes {1} seconds {2} milliseconds", avarage.Minutes, avarage.Seconds, avarage.Milliseconds);

        //    AttributeCounters.Instance.ElapsedTime.Stop();
        //}

        //private static void MultyInstanceTest()
        //{
        //    var tasks = new List<Task>
        //    {
        //        Task.Factory.StartNew(MultyTest, "task 1", TaskCreationOptions.LongRunning),
        //        Task.Factory.StartNew(MultyTest, "task 2", TaskCreationOptions.LongRunning),
        //        Task.Factory.StartNew(MultyTest, "task 3", TaskCreationOptions.LongRunning),
        //    };

        //    Task.WaitAll(tasks.ToArray());
        //}

        //private static void MultyTest(object arg)
        //{
        //    var instance = MultyCategory.Instance[(string) arg];
        //    var random = new Random(DateTime.Now.Millisecond + arg.GetHashCode());
        //    var timeCounter = instance.AverageTimer32.StartNew();

        //    for (int i = 0; i < 200; i++)
        //    {
        //        timeCounter.Restart();

        //        var value = random.Next(70, 300);

        //        Thread.Sleep(value);
        //        timeCounter.TimePoint();
        //    }
        //}

        //private static void SimpleTest(int min, int max)
        //{
        //    var averageTimer = Counters.Instance.AverageTimer32.StartNew();
        //    var random = new Random(DateTime.Now.Millisecond);

        //    averageTimer.Start();

        //    for (int i = 0; i < 200; i++)
        //    {
        //        // Начинаем замер среднего времени прохода цикла
        //        averageTimer.Restart();

        //        var value = random.Next(min, max);

        //        Thread.Sleep(value);

        //        Counters.Instance.NumericalCounter.Increment();
        //        //Counters.Instance.RateCounter.Increment();
        //        //Counters.Instance.SampleCounter.Increment();

        //        // Фиксируем время прохода цикла
        //        averageTimer.TimePoint();
        //    }
        //}

        //private static void AttributeTest(int min, int max)
        //{
        //    var averageTimer = AttributeCounters.Instance.AverageTimer32.StartNew();
        //    var random = new Random(DateTime.Now.Millisecond);

        //    averageTimer.Start();

        //    for (int i = 0; i < 200; i++)
        //    {
        //        // Начинаем замер среднего времени прохода цикла
        //        averageTimer.Restart();

        //        var value = random.Next(min, max);

        //        Thread.Sleep(value);

        //        AttributeCounters.Instance.NumericalCounter.Increment();
        //        //Counters.Instance.RateCounter.Increment();
        //        //Counters.Instance.SampleCounter.Increment();

        //        // Фиксируем время прохода цикла
        //        averageTimer.TimePoint();
        //    }
        //}
    }
}

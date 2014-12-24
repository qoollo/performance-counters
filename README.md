Qoollo Performance Counters
====================

Extremely useful library for collecting and tracking performance metrics in your .NET applications.



This library allows you to measure performance of your application by some common metrics and send this values to any target.
Main features:
- support most commonly used metrics;
- support several targets to distribute counter values;
- targets can be combined;
- XSD schema for configuration section in App.config.



Supported metrics (counter types):
- number of items (example: measure number of executed operations, number of received items and so on);
- operations per second (example: measure performance of your code);
- average count (example: measure average size of queue);
- average time (example: measure average execution time of your functions);
- moment time (example: fixate execution time of process that executed once);
- elapsed time (example: measure the total execution time of your application).



Supported targets:
- internal (values of counters available only inside your app);
- windows performance counters (values can be read by windows performance monitor);
- network (values sent to network service);
- graphite (values sent to graphite/carbon server).




## Quick start guide
- add reference to performance counters library;
- define category with required counters:
```C#
internal class TestSingleInstance: SingleInstanceCategoryWrapper
{
    public TestSingleInstance() : base("SingleInstance", "For tests") { }

    [Counter("Test NumberOfItemsCounter")]
    public NumberOfItemsCounter Count { get; private set; }
}
```
- define your counters container (if you need a singleton):
```C#
internal class PerfCounters: Qoollo.PerformanceCounters.PerfCountersContainer
{
    private static TestSingleInstance _singleInstance = CreateNullCategoryWrapper<TestSingleInstance>();
    public static TestSingleInstance TestSingle { get { return _singleInstance; } }

    public static void Init(CategoryWrapper parent)
    {
        _singleInstance = parent.CreateSubCategory<TestSingleInstance>();
    }
}
```
- create counters factory on application start-up and initialize you PerfCounters container:
```C#
var counterFactory = new WinCounterFactory();
// or load from App.config:
// var counterFactory = PerfCountersInstantiationFactory.CreateCounterFactoryFromAppConfig("PerfCountersConfigurationSection");

PerfCounters.Init(counterFactory.CreateRootWrapper());
counterFactory.InitAll();
```
- use counters in this way:
```C#
PerfCounters.TestSingle.Count.Increment();
```
- don't forget to dispose counters factory when application is closing:
```C#
counterFactory.Dispose();
```


## Extended materials
- Configuration
- Usage
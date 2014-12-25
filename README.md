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
- Number of Items (example: measure umber of received requests);
- Operations per Second (example: measure performance of your code);
- Average Count (example: measure average size of queue);
- Average Time (example: measure average execution time of your functions);
- Moment Time (example: fixate time of some process that executing only once);
- Elapsed Time (example: measure the total execution time of your application).



Supported targets:
- Internal (values are available only inside your app);
- Windows Performance Counters (values can be read by windows performance monitor);
- Network (values are sent to network service);
- Graphite (values are sent to graphite/carbon server).




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
- use counters like this:
```C#
PerfCounters.TestSingle.Count.Increment();
```
- don't forget to dispose counters factory when application is closing:
```C#
counterFactory.Dispose();
```


## Extended materials
- [Factories, Categories, Counters](https://github.com/qoollo/performance-counters/wiki/Factories,-Categories,-Counters)
- [Configuration](https://github.com/qoollo/performance-counters/wiki/Configuration)
- [Usage](https://github.com/qoollo/performance-counters/wiki/Usage)


## NuGet
- [Qoollo.PerformanceCounters](https://www.nuget.org/packages/Qoollo.PerformanceCounters)
- [Qoollo.PerformanceCounters.Config](https://www.nuget.org/packages/Qoollo.PerformanceCounters.Config)

using System;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Counter category with a single instance
    /// </summary>
    public abstract class SingleInstanceCategory : Category
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="categoryDescription">Category description</param>
        protected SingleInstanceCategory(string categoryName, string categoryDescription)
            : base(categoryName, categoryDescription, CategoryTypes.SingleInstance)
        {
        }

        /// <summary>
        /// Create counter
        /// </summary>
        /// <param name="type">Counter type</param>
        /// <param name="counterName">Existing counter name</param>
        /// <param name="counterDescription">Counter description</param>
        /// <returns>Counter</returns>
        public abstract Counter CreateCounter(CounterTypes type, string counterName, string counterDescription);

        /// <summary>
        /// Create counter (NumberOfItemsCounter)
        /// </summary>
        /// <param name="counterName">Existing counter name</param>
        /// <param name="counterDescription">Counter description</param>
        /// <returns>Counter</returns>
        public virtual NumberOfItemsCounter CreateNumberOfItemsCounter(string counterName, string counterDescription)
        {
            return (NumberOfItemsCounter)CreateCounter(CounterTypes.NumberOfItems, counterName, counterDescription);
        }

        /// <summary>
        /// Create counter (DeltaCounter)
        /// </summary>
        /// <param name="counterName">Existing counter name</param>
        /// <param name="counterDescription">Counter description</param>
        /// <returns>Counter</returns>
        public virtual DeltaCounter CreateDeltaCounter(string counterName, string counterDescription)
        {
            return (DeltaCounter)CreateCounter(CounterTypes.Delta, counterName, counterDescription);
        }

        /// <summary>
        /// Create counter (AverageTimeCounter)
        /// </summary>
        /// <param name="counterName">Existing counter name</param>
        /// <param name="counterDescription">Counter description</param>
        /// <returns>Counter</returns>
        public virtual AverageTimeCounter CreateAverageTimeCounter(string counterName, string counterDescription)
        {
            return (AverageTimeCounter)CreateCounter(CounterTypes.AverageTime, counterName, counterDescription);
        }

        /// <summary>
        /// Create counter (AverageCountCounter)
        /// </summary>
        /// <param name="counterName">Existing counter name</param>
        /// <param name="counterDescription">Counter description</param>
        /// <returns>Counter</returns>
        public virtual AverageCountCounter CreateAverageCountCounter(string counterName, string counterDescription)
        {
            return (AverageCountCounter)CreateCounter(CounterTypes.AverageCount, counterName, counterDescription);
        }

        /// <summary>
        /// Create counter (OperationsPerSecondCounter)
        /// </summary>
        /// <param name="counterName">Existing counter name</param>
        /// <param name="counterDescription">Counter description</param>
        /// <returns>Counter</returns>
        public virtual OperationsPerSecondCounter CreateOperationsPerSecondCounter(string counterName, string counterDescription)
        {
            return (OperationsPerSecondCounter)CreateCounter(CounterTypes.OperationsPerSecond, counterName, counterDescription);
        }

        /// <summary>
        /// Create counter (ElapsedTimeCounter)
        /// </summary>
        /// <param name="counterName">Existing counter name</param>
        /// <param name="counterDescription">Counter description</param>
        /// <returns>Counter</returns>
        public virtual ElapsedTimeCounter CreateElapsedTimeCounter(string counterName, string counterDescription)
        {
            return (ElapsedTimeCounter)CreateCounter(CounterTypes.ElapsedTime, counterName, counterDescription);
        }


        /// <summary>
        /// Create counter (MomentTimeCounter)
        /// </summary>
        /// <param name="counterName">Existing counter name</param>
        /// <param name="counterDescription">Counter description</param>
        /// <returns>Counter</returns>
        public virtual MomentTimeCounter CreateMomentTimeCounter(string counterName, string counterDescription)
        {
            return (MomentTimeCounter)CreateCounter(CounterTypes.MomentTime, counterName, counterDescription);
        }


        /// <summary>
        /// checks existence of counter with specific name
        /// </summary>
        /// <param name="counterName">Existing counter name</param>
        /// <returns>True if exists</returns>
        public abstract bool HasCounter(string counterName);

        /// <summary>
        /// Returns existing counter by name
        /// </summary>
        /// <param name="counterName">Existing counter name</param>
        /// <returns>Counter</returns>
        public abstract Counter GetCounter(string counterName);

        /// <summary>
        /// Returns existing counter by name and type
        /// </summary>
        /// <param name="counterName">Existing counter name</param>
        /// <param name="expectedCounterType">Counter type</param>
        /// <returns>Counter</returns>
        public abstract Counter GetCounter(string counterName, CounterTypes expectedCounterType);

        /// <summary>
        /// Returns existing NumberOfItemsCounter by name
        /// </summary>
        /// <param name="counterName">Existing counter name</param>
        /// <returns>Counter</returns>
        public virtual NumberOfItemsCounter GetNumberOfItemsCounter(string counterName)
        {
            return (NumberOfItemsCounter)GetCounter(counterName, CounterTypes.NumberOfItems);
        }

        /// <summary>
        /// Returns existing DeltaCounter by name
        /// </summary>
        /// <param name="counterName">Existing counter name</param>
        /// <returns>Counter</returns>
        public virtual DeltaCounter GetDeltaCounter(string counterName)
        {
            return (DeltaCounter)GetCounter(counterName, CounterTypes.Delta);
        }

        /// <summary>
        /// Returns existing AverageTimeCounter by name
        /// </summary>
        /// <param name="counterName">Existing counter name</param>
        /// <returns>Counter</returns>
        public virtual AverageTimeCounter GetAverageTimeCounter(string counterName)
        {
            return (AverageTimeCounter)GetCounter(counterName, CounterTypes.AverageTime);
        }


        /// <summary>
        /// Returns existing AverageCountCounter by name
        /// </summary>
        /// <param name="counterName">Existing counter name</param>
        /// <returns>Counter</returns>
        public virtual AverageCountCounter GetAverageCountCounter(string counterName)
        {
            return (AverageCountCounter)GetCounter(counterName, CounterTypes.AverageCount);
        }

        /// <summary>
        /// Returns existing OperationsPerSecondCounter by name
        /// </summary>
        /// <param name="counterName">Existing counter name</param>
        /// <returns>Counter</returns>
        public virtual OperationsPerSecondCounter GetOperationsPerSecondCounter(string counterName)
        {
            return (OperationsPerSecondCounter)GetCounter(counterName, CounterTypes.OperationsPerSecond);
        }


        /// <summary>
        /// Returns existing ElapsedTimeCounter by name
        /// </summary>
        /// <param name="counterName">Existing counter name</param>
        /// <returns>Counter</returns>
        public virtual ElapsedTimeCounter GetElapsedTimeCounter(string counterName)
        {
            return (ElapsedTimeCounter)GetCounter(counterName, CounterTypes.ElapsedTime);
        }

        /// <summary>
        /// Returns existing MomentTimeCounter by name
        /// </summary>
        /// <param name="counterName">Existing counter name</param>
        /// <returns>Counter</returns>
        public virtual MomentTimeCounter GetMomentTimeCounter(string counterName)
        {
            return (MomentTimeCounter)GetCounter(counterName, CounterTypes.MomentTime);
        }

    }
}
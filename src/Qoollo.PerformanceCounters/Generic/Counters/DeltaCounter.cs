using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Abstract Counter for the difference between the raw value at the beginning and the end of the measured time frame
    /// </summary>
    public abstract class DeltaCounter : Counter
    {
        /// <summary>
        /// Constructor of DeltaCounter
        /// </summary>
        /// <param name="name">Counter name</param>
        /// <param name="description">Counter description</param>
        protected DeltaCounter(string name, string description)
            : base(name, description, CounterTypes.Delta)
        {
        }

        /// <summary>
        /// Decrement counter value by 1
        /// </summary>
        /// <returns>Current value or Counter.FailureNum</returns>
        public abstract long Decrement();

        /// <summary>
        /// Decrement counter value by "value"
        /// </summary>
        /// <param name="value">Decrement value</param>
        /// <returns>Current value or Counter.FailureNum</returns>
        public abstract long DecrementBy(long value);

        /// <summary>
        /// Increment counter value by 1
        /// </summary>
        /// <returns>Current value or Counter.FailureNum</returns>
        public abstract long Increment();

        /// <summary>
        /// Increment counter value by "value"
        /// </summary>
        /// <param name="value">Increment value</param>
        /// <returns>Current value or Counter.FailureNum</returns>
        public abstract long IncrementBy(long value);

        /// <summary>
        /// Set current counter value
        /// </summary>
        /// <param name="value">New value</param>
        public abstract void SetValue(long value);

        /// <summary>
        /// Get current counter value
        /// </summary>
        public abstract long CurrentValue { get; }

        /// <summary>
        /// Reset the counter value
        /// </summary>
        public override void Reset()
        {
            SetValue(0);
        }


        /// <summary>
        /// Returns a string that represents the counter including value
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            return string.Format("[Value: {0}, Name: {1}, CounterType: {2}]", CurrentValue, Name, Type);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.CompositeCounters.Counters
{
    /// <summary>
    /// Счётчик числа операций в секунду для CompositeCounters
    /// </summary>
    public sealed class CompositeOperationsPerSecondCounter : OperationsPerSecondCounter
    {
        /// <summary>
        /// Дескриптор для счётчика CompositeOperationsPerSecondCounter
        /// </summary>
        private class Descriptor : CompositeCounterDescriptor
        {
            /// <summary>
            /// Конструктор Descriptor
            /// </summary>
            /// <param name="name">Имя счётчика</param>
            /// <param name="description">Описание счётчика</param>
            public Descriptor(string name, string description)
                : base(name, description, CounterTypes.OperationsPerSecond)
            {
            }

            /// <summary>
            /// Метод создания счётчика из дескриптора
            /// </summary>
            /// <param name="counters">Оборачиваемые счётчики</param>
            /// <returns>Созданный счётчик</returns>
            public override Counter CreateCounter(IEnumerable<Counter> counters)
            {
                return new CompositeOperationsPerSecondCounter(this.Name, this.Description, counters.Cast<OperationsPerSecondCounter>());
            }
        }

        /// <summary>
        /// Метод создания дескриптора для CompositeOperationsPerSecondCounter
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        /// <returns>Дескриптор</returns>
        internal static CompositeCounterDescriptor CreateDescriptor(string name, string description)
        {
            return new Descriptor(name, description);
        }

        // ===================

        /// <summary>
        /// Оборачиваемые счётчики
        /// </summary>
        private readonly OperationsPerSecondCounter[] _wrappedCounters;


        /// <summary>
        /// Конструктор CompositeOperationsPerSecondCounter
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        /// <param name="counters">Оборачиваемые счётчики</param>
        public CompositeOperationsPerSecondCounter(string name, string description, IEnumerable<OperationsPerSecondCounter> counters)
            : base(name, description)
        {
            if (counters == null)
                throw new ArgumentNullException("counters");

            _wrappedCounters = counters.ToArray();

            if (_wrappedCounters.Length == 0)
                _wrappedCounters = new OperationsPerSecondCounter[] { new NullCounters.Counters.NullOperationPerSecondCounter(name, description) };
        }

        /// <summary>
        /// Текущее значение
        /// </summary>
        public override double CurrentValue
        {
            get { return _wrappedCounters[0].CurrentValue; }
        }


        /// <summary>
        /// Закончилась 1 операция
        /// </summary>
        public override void OperationFinished()
        {
            for (int i = 0; i < _wrappedCounters.Length; i++)
                _wrappedCounters[i].OperationFinished();
        }

        /// <summary>
        /// Закончилось несколько операций
        /// </summary>
        /// <param name="operationCount">Число операций</param>
        public override void OperationFinished(int operationCount)
        {
            for (int i = 0; i < _wrappedCounters.Length; i++)
                _wrappedCounters[i].OperationFinished(operationCount);
        }


        /// <summary>
        /// Сброс значения счётчика
        /// </summary>
        public override void Reset()
        {
            for (int i = 0; i < _wrappedCounters.Length; i++)
                _wrappedCounters[i].Reset();
        } 
    }
}

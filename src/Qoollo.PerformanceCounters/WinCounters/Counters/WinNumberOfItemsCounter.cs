using Qoollo.PerformanceCounters.WinCounters.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.WinCounters.Counters
{
    /// <summary>
    /// Счетчик мгновенного значения, показывающий последнее наблюдавшееся значение, для WinCounters
    /// </summary>
    public class WinNumberOfItemsCounter : NumberOfItemsCounter, IWinCounterInitialization
    {
        /// <summary>
        /// Дескриптор для счётчика WinNumberOfItemsCounter
        /// </summary>
        private class Descriptor : WinCounterDescriptor
        {
            /// <summary>
            /// Конструктор Descriptor
            /// </summary>
            /// <param name="name">Имя счётчика</param>
            /// <param name="description">Описание счётчика</param>
            /// <param name="info">Информация о функционировании</param>
            public Descriptor(string name, string description, WinCountersWorkingInfo info)
                : base(name, description, CounterTypes.NumberOfItems, info)
            {
            }

            /// <summary>
            /// Создание счётчика
            /// </summary>
            /// <returns>Созданный счётчик</returns>
            public override Counter CreateCounter()
            {
                return new WinNumberOfItemsCounter(this.Name, this.Description, this.Info);
            }
            /// <summary>
            /// Занести данные о необходимых счётчиках Windows в коллекцию
            /// </summary>
            /// <param name="col">Коллекция</param>
            public override void FillCounterCreationData(CounterCreationDataCollection col)
            {
                CounterCreationData data = new CounterCreationData(this.Name, this.Description, CounterHelper.ConvertCounterType(this.Type, Info.Prefer64BitCounters));
                col.Add(data);
            }
        }

        /// <summary>
        /// Создание дескриптора для счётчика WinNumberOfItemsCounter
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        /// <param name="info">Информация о функционировании</param>
        /// <returns>Созданный дескриптор</returns>
        internal static WinCounterDescriptor CreateDescriptor(string name, string description, WinCountersWorkingInfo info)
        {
            return new Descriptor(name, description, info);
        }

        // ===================

        private readonly WinCountersWorkingInfo _info;
        private PerformanceCounter _winCounter;
        private volatile WinCounterState _state;

        /// <summary>
        /// Конструктор WinNumberOfItemsCounter
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        /// <param name="info">Информация о функционировании</param>
        internal WinNumberOfItemsCounter(string name, string description, WinCountersWorkingInfo info)
            : base(name, description)
        {
            _state = WinCounterState.Created;
            _info = info;
        }

        /// <summary>
        /// Уменьшить значение на 1
        /// </summary>
        /// <returns>Текущее значение или Counter.FailureNum</returns>
        public override long Decrement()
        {
            var counterCpy = _winCounter;
            if (counterCpy == null)
                return Counter.FailureNum;

            return counterCpy.Decrement();
        }

        /// <summary>
        /// Уменьшить значение на "value"
        /// </summary>
        /// <param name="value">Значение, на которое уменьшаем</param>
        /// <returns>Текущее значение или Counter.FailureNum</returns>
        public override long DecrementBy(long value)
        {
            var counterCpy = _winCounter;
            if (counterCpy == null)
                return Counter.FailureNum;

            return counterCpy.IncrementBy(-value);
        }

        /// <summary>
        /// Увеличить значение на 1
        /// </summary>
        /// <returns>Текущее значение или Counter.FailureNum</returns>
        public override long Increment()
        {
            var counterCpy = _winCounter;
            if (counterCpy == null)
                return Counter.FailureNum;

            return counterCpy.Increment();
        }

        /// <summary>
        /// Увеличить значение на "value"
        /// </summary>
        /// <param name="value">Значение, на которое увеличиваем</param>
        /// <returns>Текущее значение или Counter.FailureNum</returns>
        public override long IncrementBy(long value)
        {
            var counterCpy = _winCounter;
            if (counterCpy == null)
                return Counter.FailureNum;

            return counterCpy.IncrementBy(value);
        }

        /// <summary>
        /// Задать значение
        /// </summary>
        /// <param name="value">Новое значение</param>
        public override void SetValue(long value)
        {
            var counterCpy = _winCounter;
            if (counterCpy == null)
                return;

            counterCpy.RawValue = value;
        }

        /// <summary>
        /// Получает текущее значение счетчика производительности
        /// </summary>
        public override long CurrentValue
        {
            get 
            {
                var counterCpy = _winCounter;
                if (counterCpy == null)
                    return Counter.FailureNum;

                return counterCpy.RawValue;
            }
        }

        /// <summary>
        /// Занести данные о необходимых счётчиках Windows в коллекцию
        /// </summary>
        /// <param name="col">Коллекция</param>
        void IWinCounterInitialization.CounterFillCreationData(CounterCreationDataCollection col)
        {
            CounterCreationData data = new CounterCreationData(this.Name, this.Description, CounterHelper.ConvertCounterType(this.Type, _info.Prefer64BitCounters));
            col.Add(data);
        }

        /// <summary>
        /// Проинициализировать счётчик Windows
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="instanceName">Имя инстанса (null, если одноинстансовая категория)</param>
        void IWinCounterInitialization.CounterInit(string categoryName, string instanceName)
        {
            if (_state == WinCounterState.Disposed)
                throw new ObjectDisposedException(this.GetType().Name);
            if (_state == WinCounterState.Initialized)
                return;

            _state = WinCounterState.Initialized;

            if (_info.IsLocalMachine)
            {
                _winCounter = new PerformanceCounter(categoryName, this.Name, instanceName ?? "", _info.ReadOnlyCounters);
                if (!_info.ReadOnlyCounters)
                    _winCounter.RawValue = 0;
            }
            else
            {
                _winCounter = new PerformanceCounter(categoryName, this.Name, instanceName ?? "", _info.MachineName);
            }
        }


        /// <summary>
        /// Освободить счётчик
        /// </summary>
        /// <param name="removeInstance">Удалить ли его инстанс из Windows</param>
        void IWinCounterInitialization.CounterDispose(bool removeInstance)
        {
            _state = WinCounterState.Disposed;

            var oldVal = Interlocked.Exchange(ref _winCounter, null);
            if (oldVal != null)
            {
                if (removeInstance)
                    oldVal.RemoveInstance();
                oldVal.Dispose();
            }
        }
    }
}

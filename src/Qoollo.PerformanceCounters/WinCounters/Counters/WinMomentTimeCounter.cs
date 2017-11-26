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
    /// Счётчик моментального времени для WinCounters
    /// </summary> 
    public class WinMomentTimeCounter: MomentTimeCounter, IWinCounterInitialization
    {
        /// <summary>
        /// Дескриптор для счётчика WinMomentTimeCounter
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
                : base(name, description, CounterTypes.MomentTime, info)
            {
            }

            /// <summary>
            /// Создание счётчика
            /// </summary>
            /// <returns>Созданный счётчик</returns>
            public override Counter CreateCounter()
            {
                return new WinMomentTimeCounter(this.Name, this.Description, this.Info);
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
        /// Создание дескриптора для счётчика WinMomentTimeCounter
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
        /// Конструктор WinMomentTimeCounter
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        /// <param name="info">Информация о функционировании</param>
        internal WinMomentTimeCounter(string name, string description, WinCountersWorkingInfo info)
            : base(name, description)
        {
            _state = WinCounterState.Created;
            _info = info;
        }

        /// <summary>
        /// Сконвертировать TimeSpan в тики счётчика Windows
        /// </summary>
        /// <param name="period">Период времени</param>
        /// <returns>Тики</returns>
        private static long ConvertToTicks(TimeSpan period)
        {
            return (long)(((double)Stopwatch.Frequency / (double)TimeSpan.TicksPerSecond) * period.Ticks);
        }
        /// <summary>
        /// Скорнвертировать тики счётчика Windows в TimeSpan
        /// </summary>
        /// <param name="ticks">Тики</param>
        /// <returns>Сконвертированный период времени</returns>
        private static TimeSpan ConvertFromTicks(long ticks)
        {
            return new TimeSpan((long)(((double)TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency) * ticks));
        }


        /// <summary>
        /// Текущее значение
        /// </summary>
        public override TimeSpan CurrentValue
        {
            get 
            {
                var counterCpy = _winCounter;
                if (counterCpy == null)
                    return FailureTime;

                return TimeSpan.FromMilliseconds(counterCpy.RawValue);
            }
        }

        /// <summary>
        /// Зарегистрировать измерение
        /// </summary>
        /// <param name="period">Период</param>
        public override void RegisterMeasurement(TimeSpan period)
        {
            var counterCpy = _winCounter;
            if (counterCpy == null)
                return;

            counterCpy.RawValue = (long)period.TotalMilliseconds;
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
                {
                    _winCounter.RawValue = 0;
                }
            }
            else
            {
                _winCounter = new PerformanceCounter(categoryName, this.Name, instanceName ?? "", _info.MachineName);
            }
            _winCounter.NextValue();
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

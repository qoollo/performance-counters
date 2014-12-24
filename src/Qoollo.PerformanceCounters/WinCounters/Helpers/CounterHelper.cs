using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Qoollo.PerformanceCounters.WinCounters.Counters;

namespace Qoollo.PerformanceCounters.WinCounters.Helpers
{
    /// <summary>
    /// Помошник для работы со счётчиками
    /// </summary>
    internal static class CounterHelper
    {
        /// <summary>
        /// Преобразовать тип счётчика из внутреннего в Windows
        /// </summary>
        /// <param name="srcType">Исходный тип счётчика</param>
        /// <param name="prefer64BitCounter">Предпочитать ли 64-ёх битные счётчики</param>
        /// <returns>Преобразованный тип счётчика</returns>
        public static PerformanceCounterType ConvertCounterType(CounterTypes srcType, bool prefer64BitCounter)
        {
            switch (srcType)
            {
                case CounterTypes.NumberOfItems:
                    if (prefer64BitCounter)
                        return PerformanceCounterType.NumberOfItems64;
                    else
                        return PerformanceCounterType.NumberOfItems32;
                case CounterTypes.OperationsPerSecond:
                    if (prefer64BitCounter)
                        return PerformanceCounterType.RateOfCountsPerSecond64;
                    else
                        return PerformanceCounterType.RateOfCountsPerSecond32;
                //case CounterTypes.CountPerTimeInterval:
                //    break;
                case CounterTypes.AverageCount:
                    return PerformanceCounterType.AverageCount64;
                case CounterTypes.AverageTime:
                    return PerformanceCounterType.AverageTimer32;
                case CounterTypes.MomentTime:
                    if (prefer64BitCounter)
                        return PerformanceCounterType.NumberOfItems64;
                    else
                        return PerformanceCounterType.NumberOfItems32;
                //case CounterTypes.RawFraction:
                //    break;
                //case CounterTypes.CounterMultiTimer:
                //    break;
                //case CounterTypes.SampleCounter:
                //    break;
                //case CounterTypes.SampleFraction:
                //    break;
                //case CounterTypes.CounterDelta:
                //    break;
                case CounterTypes.ElapsedTime:
                    return PerformanceCounterType.ElapsedTime;
                default:
                    throw new InvalidOperationException("Unknown CounterTypes value: " + srcType.ToString());
            }
        }

        /// <summary>
        /// Попробовать сконвертировать тип счётчика Windows во внутренний тип счётчика
        /// </summary>
        /// <param name="srcWinType">Тип счётчика Windows</param>
        /// <param name="resLocType">Сконвертированный тип счётчика</param>
        /// <returns>Найдено ли соответствие</returns>
        public static bool TryConvertCounterType(PerformanceCounterType srcWinType, out CounterTypes resLocType)
        {
            switch (srcWinType)
            {
                //case PerformanceCounterType.AverageBase:
                //    break;
                case PerformanceCounterType.AverageCount64:
                    resLocType = CounterTypes.AverageCount;
                    return true;
                case PerformanceCounterType.AverageTimer32:
                    resLocType = CounterTypes.AverageTime;
                    return true;
                //case PerformanceCounterType.CountPerTimeInterval32:
                //    break;
                //case PerformanceCounterType.CountPerTimeInterval64:
                //    break;
                //case PerformanceCounterType.CounterDelta32:
                //    break;
                //case PerformanceCounterType.CounterDelta64:
                //    break;
                //case PerformanceCounterType.CounterMultiBase:
                //    break;
                //case PerformanceCounterType.CounterMultiTimer:
                //    break;
                //case PerformanceCounterType.CounterMultiTimer100Ns:
                //    break;
                //case PerformanceCounterType.CounterMultiTimer100NsInverse:
                //    break;
                //case PerformanceCounterType.CounterMultiTimerInverse:
                //    break;
                //case PerformanceCounterType.CounterTimer:
                //    break;
                //case PerformanceCounterType.CounterTimerInverse:
                //    break;
                case PerformanceCounterType.ElapsedTime:
                    resLocType = CounterTypes.ElapsedTime;
                    return true;
                case PerformanceCounterType.NumberOfItems32:
                    resLocType = CounterTypes.NumberOfItems;
                    return true;
                case PerformanceCounterType.NumberOfItems64:
                    resLocType = CounterTypes.NumberOfItems;
                    return true;
                //case PerformanceCounterType.NumberOfItemsHEX32:
                //    break;
                //case PerformanceCounterType.NumberOfItemsHEX64:
                //    break;
                case PerformanceCounterType.RateOfCountsPerSecond32:
                    resLocType = CounterTypes.OperationsPerSecond;
                    return true;
                case PerformanceCounterType.RateOfCountsPerSecond64:
                    resLocType = CounterTypes.OperationsPerSecond;
                    return true;
                //case PerformanceCounterType.RawBase:
                //    break;
                //case PerformanceCounterType.RawFraction:
                //    break;
                //case PerformanceCounterType.SampleBase:
                //    break;
                //case PerformanceCounterType.SampleCounter:
                //    break;
                //case PerformanceCounterType.SampleFraction:
                //    break;
                //case PerformanceCounterType.Timer100Ns:
                //    break;
                //case PerformanceCounterType.Timer100NsInverse:
                //    break;
            }

            resLocType = default(CounterTypes);
            return false;
        }


        /// <summary>
        /// Совместимы ли представления счётчиков в Windows
        /// </summary>
        /// <param name="a">Тип первого счётчика</param>
        /// <param name="b">Тип второго счётчика</param>
        /// <returns>Совместимы ли</returns>
        public static bool IsWinCompatible(CounterTypes a, CounterTypes b)
        {
            if (a == b)
                return true;

            switch (a)
            {
                case CounterTypes.NumberOfItems:
                    return b == CounterTypes.NumberOfItems || b == CounterTypes.MomentTime;
                case CounterTypes.MomentTime:
                    return b == CounterTypes.MomentTime || b == CounterTypes.NumberOfItems;
            }

            return false;
        }

        /// <summary>
        /// Выполнить преобразование типа счётчика Windows во внутренний тип счётчика
        /// </summary>
        /// <param name="srcWinType">Исходный тип счётчика Windows</param>
        /// <returns>Внутренний тип счётичка после преобразования</returns>
        /// <exception cref="ArgumentException">Неподдерживаемый тип счётчика</exception>
        public static CounterTypes ConvertCounterType(PerformanceCounterType srcWinType)
        {
            CounterTypes res;
            if (!TryConvertCounterType(srcWinType, out res))
                throw new ArgumentException("Unsupported PerformanceCounterType value: " + srcWinType.ToString());

            return res;
        }

        /// <summary>
        /// Создать внутренний счётчик на основе существующего счётчика Windows
        /// </summary>
        /// <param name="src">Счётчик Windows</param>
        /// <param name="info">Информацию по функционированию</param>
        /// <returns>Созданный внутренний счётчик</returns>
        public static Counter CreateByExistedCounter(PerformanceCounter src, WinCountersWorkingInfo info)
        {
            CounterTypes reqType;
            if (!TryConvertCounterType(src.CounterType, out reqType))
                return null;

            return CreateCounter(reqType, src.CounterName, src.CounterHelp, info);
        }

        /// <summary>
        /// Создать внутренний счётчик на основе существующего счётчика Windows и инициализировать его
        /// </summary>
        /// <param name="src">Счётчик Windows</param>
        /// <param name="info">Информацию по функционированию</param>
        /// <returns>Созданный и инициализированный внутренний счётчик</returns>
        public static Counter CreateByExistedCounterAndInit(PerformanceCounter src, WinCountersWorkingInfo info)
        {
            var res = CreateByExistedCounter(src, info);
            if (res == null)
                return null;

            ((IWinCounterInitialization)res).CounterInit(src.CategoryName, src.InstanceName);
            return res;
        }


        /// <summary>
        /// Создать дескриптор счётчика на основе существующего счётчика Windows
        /// </summary>
        /// <param name="src">Счётчик Windows</param>
        /// <param name="info">Информацию по функционированию</param>
        /// <returns>Созданный дескриптор счётчика</returns>
        public static WinCounterDescriptor CreateDescriptorByExistedCounter(PerformanceCounter src, WinCountersWorkingInfo info)
        {
            CounterTypes reqType;
            if (!TryConvertCounterType(src.CounterType, out reqType))
                return null;

            return CreateCounterDescriptor(reqType, src.CounterName, src.CounterHelp, info);
        }


        /// <summary>
        /// Создать внутренний счётчик
        /// </summary>
        /// <param name="type">Тип счётчика</param>
        /// <param name="counterName">Имя счётчика</param>
        /// <param name="counterDescription">Описание счётчика</param>
        /// <param name="info">Информацию по функционированию</param>
        /// <returns>Созданный счётчик</returns>
        public static Counter CreateCounter(CounterTypes type, string counterName, string counterDescription, WinCountersWorkingInfo info)
        {
            switch (type)
            {
                case CounterTypes.NumberOfItems:
                    return new WinNumberOfItemsCounter(counterName, counterDescription, info);
                case CounterTypes.AverageTime:
                    return new WinAverageTimeCounter(counterName, counterDescription, info);
                case CounterTypes.AverageCount:
                    return new WinAverageCountCounter(counterName, counterDescription, info);
                case CounterTypes.OperationsPerSecond:
                    return new WinOperationsPerSecondCounter(counterName, counterDescription, info);
                case CounterTypes.ElapsedTime:
                    return new WinElapsedTimeCounter(counterName, counterDescription, info);
                case CounterTypes.MomentTime:
                    return new WinMomentTimeCounter(counterName, counterDescription, info);
                default:
                    throw new InvalidOperationException("Unknown CounterTypes value: " + type.ToString());
            }
        }

        /// <summary>
        /// Создать дескриптор счётчика
        /// </summary>
        /// <param name="type">Тип счётчика</param>
        /// <param name="counterName">Имя счётчика</param>
        /// <param name="counterDescription">Описание счётчика</param>
        /// <param name="info">Информацию по функционированию</param>
        /// <returns>Созданный дескриптор</returns>
        public static WinCounterDescriptor CreateCounterDescriptor(CounterTypes type, string counterName, string counterDescription, WinCountersWorkingInfo info)
        {
            switch (type)
            {
                case CounterTypes.NumberOfItems:
                    return WinNumberOfItemsCounter.CreateDescriptor(counterName, counterDescription, info);
                case CounterTypes.AverageTime:
                    return WinAverageTimeCounter.CreateDescriptor(counterName, counterDescription, info);
                case CounterTypes.AverageCount:
                    return WinAverageCountCounter.CreateDescriptor(counterName, counterDescription, info);
                case CounterTypes.OperationsPerSecond:
                    return WinOperationsPerSecondCounter.CreateDescriptor(counterName, counterDescription, info);
                case CounterTypes.ElapsedTime:
                    return WinElapsedTimeCounter.CreateDescriptor(counterName, counterDescription, info);
                case CounterTypes.MomentTime:
                    return WinMomentTimeCounter.CreateDescriptor(counterName, counterDescription, info);
                default:
                    throw new InvalidOperationException("Unknown CounterTypes value: " + type.ToString());
            }
        }
    }
}

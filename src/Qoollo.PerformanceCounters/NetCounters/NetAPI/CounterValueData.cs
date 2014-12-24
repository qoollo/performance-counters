using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NetCounters.NetAPI
{
    /// <summary>
    /// Данные категории
    /// </summary>
    [DataContract]
    public class CategoryValueData
    {
        private CategoryValueData() { }

        /// <summary>
        /// Конструктор CategoryValueData
        /// </summary>
        /// <param name="namePath">Имена родительских категорий и данной</param>
        /// <param name="type">Тип категории</param>
        /// <param name="instances">Инстансы в данной категории</param>
        public CategoryValueData(string[] namePath, CategoryTypes type, InstanceValueData[] instances)
        {
            if (namePath == null)
                throw new ArgumentNullException("namePath");
            if (namePath.Length == 0)
                throw new ArgumentException("namePath is empty");
            if (instances == null)
                throw new ArgumentNullException("instances");

            NamePath = namePath;
            Type = type;
            Instances = instances;
        }

        /// <summary>
        /// Имя категории
        /// </summary>
        public string Name { get { return NamePath[NamePath.Length - 1]; } }

        /// <summary>
        /// Имена родительских категорий и данной
        /// </summary>
        [DataMember(Order = 1)]
        public string[] NamePath { get; private set; }

        /// <summary>
        /// Тип категории
        /// </summary>
        [DataMember(Order = 2)]
        public CategoryTypes Type { get; private set; }

        /// <summary>
        /// Инстансы
        /// </summary>
        [DataMember(Order = 3)]
        public InstanceValueData[] Instances { get; private set; }


        /// <summary>
        /// Преобразовать к строке
        /// </summary>
        /// <returns>Строка</returns>
        public override string ToString()
        {
            return string.Format("[{0}: {1}]", Type, string.Join(".", NamePath.SkipWhile(o => o == "")));
        }
    }

    /// <summary>
    /// Данные инстанса
    /// </summary>
    [DataContract]
    public class InstanceValueData
    {
        private InstanceValueData() { }

        /// <summary>
        /// Конструктор InstanceValueData
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <param name="counters">Счётчики</param>
        public InstanceValueData(string instanceName, CounterValueData[] counters)
        {
            if (instanceName == null)
                throw new ArgumentNullException("instanceName");
            if (counters == null)
                throw new ArgumentNullException("counters");

            InstanceName = instanceName;
            Counters = counters;
        }
        /// <summary>
        /// Конструктор InstanceValueData для SingleInstanceCategory
        /// </summary>
        /// <param name="counters">Счётчики</param>
        public InstanceValueData(CounterValueData[] counters)
            : this("", counters)
        {
        }


        /// <summary>
        /// Имя инстанса
        /// </summary>
        [DataMember(Order = 1)]
        public string InstanceName { get; private set; }

        /// <summary>
        /// Счётчики
        /// </summary>
        [DataMember(Order = 2)]
        public CounterValueData[] Counters { get; private set; }

        /// <summary>
        /// Преобразовать к строке
        /// </summary>
        /// <returns>Строка</returns>
        public override string ToString()
        {
            return string.Format("[Instance: {0}]", InstanceName);
        }
    }


    /// <summary>
    /// Хранение значений счётчика
    /// </summary>
    [DataContract]
    [KnownType(typeof(CounterValueData))]
    [KnownType(typeof(TimeCounterValueData))]
    [KnownType(typeof(ItemCounterValueData))]
    [KnownType(typeof(FractionCounterValueData))]
    public class CounterValueData
    {
        /// <summary>
        /// Конструктор CounterValueData
        /// </summary>
        protected CounterValueData() { }

        /// <summary>
        /// Конструктор CounterValueData
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="type">Тип счётчика</param>
        /// <param name="rawValue">Значение счётчика</param>
        public CounterValueData(string name, CounterTypes type, double rawValue)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            Name = name;
            Type = type;
            RawValue = rawValue;
        }

        /// <summary>
        /// Имя счётчика
        /// </summary>
        [DataMember(Order = 1)]
        public string Name { get; private set; }

        /// <summary>
        /// Тип счётчика
        /// </summary>
        [DataMember(Order = 2)]
        public CounterTypes Type { get; protected set; }

        /// <summary>
        /// Значение счётчика
        /// </summary>
        [DataMember(Order = 3)]
        public double RawValue { get; protected set; }

        /// <summary>
        /// Преобразовать к строке
        /// </summary>
        /// <returns>Строка</returns>
        public override string ToString()
        {
            return string.Format("[{0}: {1}]", Name, RawValue);
        }
    }


    /// <summary>
    /// Тип для счётчиков времени
    /// </summary>
    [DataContract]
    public class TimeCounterValueData: CounterValueData
    {
        private TimeCounterValueData() { }

        /// <summary>
        /// Конструктор TimeCounterValueData
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="type">Тип счётчика</param>
        /// <param name="value">Значение счётчика</param>
        public TimeCounterValueData(string name, CounterTypes type, TimeSpan value)
            : base(name, type, value.TotalMilliseconds)
        {
        }
        
        /// <summary>
        /// Значение счётчика
        /// </summary>
        public TimeSpan Value { get { return TimeSpan.FromMilliseconds(this.RawValue); } }

        /// <summary>
        /// Преобразовать к строке
        /// </summary>
        /// <returns>Строка</returns>
        public override string ToString()
        {
            return string.Format("[{0}: {1}]", Name, Value);
        }
    }


    /// <summary>
    /// Тип для целочисленных счётчиков
    /// </summary>
    [DataContract]
    public class ItemCounterValueData : CounterValueData
    {
        private ItemCounterValueData() { }

        /// <summary>
        /// Конструктор ItemCounterValueData
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="type">Тип счётчика</param>
        /// <param name="value">Значение счётчика</param>
        public ItemCounterValueData(string name, CounterTypes type, long value)
            : base(name, type, value)
        {
        }

        /// <summary>
        /// Значение счётчика
        /// </summary>
        public long Value { get { return (long)this.RawValue; } }

        /// <summary>
        /// Преобразовать к строке
        /// </summary>
        /// <returns>Строка</returns>
        public override string ToString()
        {
            return string.Format("[{0}: {1}]", Name, Value);
        }
    }

    /// <summary>
    /// Ти для дробных счётчиков
    /// </summary>
    [DataContract]
    public class FractionCounterValueData : CounterValueData
    {
        private FractionCounterValueData() { }

        /// <summary>
        /// Конструктор FractionCounterValueData
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="type">Тип счётчика</param>
        /// <param name="value">Значение счётчика</param>
        public FractionCounterValueData(string name, CounterTypes type, double value)
            : base(name, type, value)
        {
        }

        /// <summary>
        /// Значение счётчика
        /// </summary>
        public double Value { get { return this.RawValue; } }

        /// <summary>
        /// Преобразовать к строке
        /// </summary>
        /// <returns>Строка</returns>
        public override string ToString()
        {
            return string.Format("[{0}: {1}]", Name, Value);
        }
    }
}

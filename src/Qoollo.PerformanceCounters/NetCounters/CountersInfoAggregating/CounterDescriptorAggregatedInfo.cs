using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NetCounters.CountersInfoAggregating
{
    /// <summary>
    /// Описатель счётчика
    /// </summary>
    internal class CounterDescriptorAggregatedInfo
    {
        /// <summary>
        /// Конструктор CounterDescriptorAggregatedInfo
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        /// <param name="type">Тип счётчика</param>
        public CounterDescriptorAggregatedInfo(string name, string description, CounterTypes type)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (description == null)
                throw new ArgumentNullException("description");

            Name = name;
            Description = description;
            Type = type;
        }

        /// <summary>
        /// Имя счётчика
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Описание счётчика
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Тип счётчика
        /// </summary>
        public CounterTypes Type { get; protected set; }
    }
}

using Qoollo.PerformanceCounters.WinCounters.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.WinCounters.Counters
{
    /// <summary>
    /// Дескриптор счётчика Windows.
    /// Позволяет создать впоследствии сам счётчик.
    /// </summary>
    internal abstract class WinCounterDescriptor
    {
        /// <summary>
        /// Конструктор WinCounterDescriptor
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        /// <param name="type">Тип счётчика</param>
        /// <param name="info">Информация о функционировании</param>
        protected WinCounterDescriptor(string name, string description, CounterTypes type, WinCountersWorkingInfo info)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (description == null)
                throw new ArgumentNullException("description");
            if (info == null)
                throw new ArgumentNullException("info");

            Name = name;
            Description = description;
            Type = type;
            Info = info;
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

        /// <summary>
        /// Информация о функционировании
        /// </summary>
        public WinCountersWorkingInfo Info { get; private set; }

        /// <summary>
        /// Создание счётчика
        /// </summary>
        /// <returns>Созданный счётчик</returns>
        public abstract Counter CreateCounter();

        /// <summary>
        /// Занести данные о необходимых счётчиках Windows в коллекцию
        /// </summary>
        /// <param name="col">Коллекция</param>
        public abstract void FillCounterCreationData(CounterCreationDataCollection col);
    }
}

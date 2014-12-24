using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NetCounters.NetAPI
{
    /// <summary>
    /// Описатель категории
    /// </summary>
    [DataContract]
    public class CategoryDescriptionData
    {
        private CategoryDescriptionData() { }

        /// <summary>
        /// Конструктор CategoryDescriptionData
        /// </summary>
        /// <param name="namePath">Имена родительских категорий и данной в конце</param>
        /// <param name="description">Описание категории</param>
        /// <param name="type">Тип категории</param>
        /// <param name="counters">Счётчики в данной категории</param>
        public CategoryDescriptionData(string[] namePath, string description, CategoryTypes type, CounterDescriptionData[] counters)
        {
            if (namePath == null)
                throw new ArgumentNullException("namePath");
            if (namePath.Length == 0)
                throw new ArgumentException("namePath is empty");
            if (description == null)
                throw new ArgumentNullException("description");
            if (counters == null)
                throw new ArgumentNullException("counters");

            NamePath = namePath;
            Description = description;
            Type = type;
            Counters = counters;
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
        /// Описание категории
        /// </summary>
        [DataMember(Order = 2)]
        public string Description { get; private set; }

        /// <summary>
        /// Тип категории
        /// </summary>
        [DataMember(Order = 3)]
        public CategoryTypes Type { get; private set; }

        /// <summary>
        /// Счётчики в данной категории
        /// </summary>
        [DataMember(Order = 3)]
        public CounterDescriptionData[] Counters { get; private set; }

        /// <summary>
        /// Преобразовать к строке
        /// </summary>
        /// <returns>Строка</returns>
        public override string ToString()
        {
            return string.Format("[{0}: {1}]", string.Join(".", NamePath.SkipWhile(o => o == "")), Type);
        }
    }

    /// <summary>
    /// Описатель счётчика
    /// </summary>
    [DataContract]
    public class CounterDescriptionData
    {
        private CounterDescriptionData() { }

        /// <summary>
        /// Конструктор CounterDescriptionData
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        /// <param name="type">Тип счётчика</param>
        public CounterDescriptionData(string name, string description, CounterTypes type)
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
        [DataMember(Order = 1)]
        public string Name { get; private set; }

        /// <summary>
        /// Описание счётчика
        /// </summary>
        [DataMember(Order = 2)]
        public string Description { get; private set; }

        /// <summary>
        /// Тип счётчика
        /// </summary>
        [DataMember(Order = 3)]
        public CounterTypes Type { get; protected set; }

        /// <summary>
        /// Преобразовать к строке
        /// </summary>
        /// <returns>Строка</returns>
        public override string ToString()
        {
            return string.Format("[{0}: {1}]", Name, Type);
        }
    }
}

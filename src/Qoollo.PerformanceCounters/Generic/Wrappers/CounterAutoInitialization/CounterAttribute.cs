using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Атрибут для счетчика производительности.
    /// Содержит описание счетчика необходимое для его создания и использования.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class CounterAttribute : Attribute
    {
        private readonly string _name;
        private readonly string _description;

        /// <summary>
        /// Конструктор CounterAttribute. 
        /// Имя счётчика и описание будут соответствовать имени свойства
        /// </summary>
        public CounterAttribute() { }

        /// <summary>
        /// Конструктор CounterAttribute.
        /// Имя счётчика будет соответствовать имени свойства
        /// </summary>
        /// <param name="description">Описание счётчика</param>
        public CounterAttribute(string description)
        {
            _description = description;
        }

        /// <summary>
        /// Конструктор CounterAttribute
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        public CounterAttribute(string name, string description)
        {
            _name = name;
            _description = description;
        }

        /// <summary>
        /// Имя счётчика
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// Описание счетчика
        /// </summary>
        public string Description { get { return _description; } }
    }
    
}

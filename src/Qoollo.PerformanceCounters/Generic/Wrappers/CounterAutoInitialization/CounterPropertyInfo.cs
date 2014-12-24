using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.CounterAutoInitialization
{
    internal delegate void GenericSetter(object target, object value);
    internal delegate object GenericGetter(object target);

    /// <summary>
    /// Описатель свойства счётчика в обёртке
    /// </summary>
    internal class CounterPropertyInfo
    {
        private readonly GenericSetter _setter;
        private readonly GenericGetter _getter;

        /// <summary>
        /// Конструктор CounterPropertyInfo
        /// </summary>
        /// <param name="containerType">Тип контейнера</param>
        /// <param name="counterProp">Свойство</param>
        /// <param name="counterAttrib">Атрибут</param>
        /// <param name="counterType">Тип счётчика</param>
        /// <param name="counterSetter">Делегат установки значения счётчика</param>
        /// <param name="couterGetter">Делегат для получения счётчика</param>
        public CounterPropertyInfo(Type containerType, PropertyInfo counterProp, CounterAttribute counterAttrib, CounterTypes counterType, GenericSetter counterSetter, GenericGetter couterGetter)
        {
            if (containerType == null)
                throw new ArgumentNullException("containerType");
            if (counterProp == null)
                throw new ArgumentNullException("counterProp");
            if (counterAttrib == null)
                throw new ArgumentNullException("counterAttrib");
            if (counterSetter == null)
                throw new ArgumentNullException("counterSetter");
            if (couterGetter == null)
                throw new ArgumentNullException("couterGetter");

            CounterContainerType = containerType;
            Property = counterProp;
            Attribute = counterAttrib;
            CounterType = counterType;
            _setter = counterSetter;
            _getter = couterGetter;
        }

        /// <summary>
        /// Тип контейнера
        /// </summary>
        public Type CounterContainerType { get; private set; }
        /// <summary>
        /// Свойство
        /// </summary>
        public PropertyInfo Property { get; private set; }
        /// <summary>
        /// Атрибут счётчика на свойстве
        /// </summary>
        public CounterAttribute Attribute { get; private set; }
        /// <summary>
        /// Тип счётчика
        /// </summary>
        public CounterTypes CounterType { get; private set; }

        /// <summary>
        /// Получить имя счётчика (из атрибута или имени свойства)
        /// </summary>
        /// <returns>Имя счётчика</returns>
        public string GetCounterName()
        {
            return Attribute.Name ?? Property.Name;
        }

        /// <summary>
        /// Получить описание счётчика (из атрибута или имени свойства)
        /// </summary>
        /// <returns>Описание счётчика</returns>
        public string GetCounterDescription()
        {
            return Attribute.Description ?? Property.Name;
        }

        /// <summary>
        /// Задать значение счётчика в обёртке
        /// </summary>
        /// <param name="instance">Обёртка</param>
        /// <param name="counter">Объект счётчика</param>
        public void SetCounterValue(object instance, object counter)
        {
            _setter(instance, counter);
        }
        /// <summary>
        /// Получить счётчик в обёртке
        /// </summary>
        /// <param name="instance">Обёртка</param>
        /// <returns>Объект счётчика</returns>
        public object GetCounterValue(object instance)
        {
            return _getter(instance);
        }
    }
}

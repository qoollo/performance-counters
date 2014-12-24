using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;

namespace Qoollo.PerformanceCounters.CounterAutoInitialization
{
    /// <summary>
    /// Помощник для извлечения CounterPropertyInfo
    /// </summary>
    internal static class CounterPropertyInfoExtractor
    {
        /// <summary>
        /// Получить описатель свойства счётчика
        /// </summary>
        /// <param name="type">Тип обёртки</param>
        /// <param name="property">Свойство</param>
        /// <returns>Описатель или null, если не помечен атрибутом</returns>
        public static CounterPropertyInfo GetCounterPropertyInfo(Type type, PropertyInfo property)
        {
            if (type == null)
                throw new ArgumentNullException("type", "Тип класса категориги не божет быть null");
            if (property == null)
                throw new ArgumentNullException("property", "Свойство не может быть null");


            var attrArr = property.GetCustomAttributes(typeof(CounterAttribute), false);
            if (attrArr == null || attrArr.Length == 0)
                return null;

            var attr = attrArr[0] as CounterAttribute;
            if (attr == null)
                return null;

            CounterTypes counterType = CounterTypes.NumberOfItems;
            try
            {
                counterType = CounterTypeConverter.Convert(property.PropertyType);
            }
            catch (UnknownCounterTypeException ex)
            {
                throw new InvalidCounterDefinitionException(string.Format("Unknown counter type. WrapperType: {0}, Property: {1}", type.Name, property.Name), ex);
            }

            var setter = CreateSetMethod(property);
            if (setter == null)
                throw new InvalidCounterDefinitionException(
                    string.Format("Counter property in wrapper should have Set method. WrapperType: {0}, Property: {1}", type.Name, property.Name));

            var getter = CreateGetMethod(property);
            if (getter == null)
                throw new InvalidCounterDefinitionException(
                    string.Format("Counter property in wrapper should have Get method. WrapperType: {0}, Property: {1}", type.Name, property.Name));

            return new CounterPropertyInfo(type, property, attr, counterType, setter, getter);
        }

        /// <summary>
        /// Скомпилировать метод установки значения счётчика в обёртке
        /// </summary>
        /// <param name="propertyInfo">Свойство</param>
        /// <returns>Делегат на сеттер</returns>
        private static GenericSetter CreateSetMethod(PropertyInfo propertyInfo)
        {
            var setMethod = propertyInfo.GetSetMethod(true);
            if (setMethod == null)
                return null;

            var p1 = Expression.Parameter(typeof(object));
            var p2 = Expression.Parameter(typeof(object));

            var p1Conv = Expression.Convert(p1, propertyInfo.DeclaringType);
            var p2Conv = Expression.Convert(p2, propertyInfo.PropertyType);

            var propSet = Expression.Call(p1Conv, setMethod, p2Conv);

            return Expression.Lambda<GenericSetter>(propSet, p1, p2).Compile();
        }

        /// <summary>
        /// Скомпилировать метод получения значения счётчика в обёртке
        /// </summary>
        /// <param name="propertyInfo">Свойство</param>
        /// <returns>Делегат на геттер</returns>
        private static GenericGetter CreateGetMethod(PropertyInfo propertyInfo)
        {
            var getMethod = propertyInfo.GetGetMethod(true);
            if (getMethod == null)
                return null;

            var p1 = Expression.Parameter(typeof(object));

            var p1Conv = Expression.Convert(p1, propertyInfo.DeclaringType);
            var propGet = Expression.Call(p1Conv, getMethod);
            var resultConv = Expression.Convert(propGet, typeof(object));

            return Expression.Lambda<GenericGetter>(resultConv, p1).Compile();
        }


        /// <summary>
        /// Получить описатель для всех свойств счётчиков в обёртке
        /// </summary>
        /// <param name="type">Тип обёртки</param>
        /// <returns>Перечень описателей</returns>
        public static List<CounterPropertyInfo> GetContainerCounterProperties(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type", "Тип класса категориги не божет быть null");

            var res = new List<CounterPropertyInfo>();
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var prop = GetCounterPropertyInfo(type, property);

                if (prop != null)
                    res.Add(prop);
            }

            return res;
        }
    }
}

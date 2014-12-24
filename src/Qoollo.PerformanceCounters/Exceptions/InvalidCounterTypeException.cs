using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Исключение, когда тип счётчика невалидный
    /// </summary>
    [Serializable]
    public class InvalidCounterTypeException : PerformanceCountersException
    {
        /// <summary>
        /// Конструктор InvalidCounterTypeException
        /// </summary>
        public InvalidCounterTypeException() { }
        /// <summary>
        /// Конструктор InvalidCounterTypeException
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        public InvalidCounterTypeException(string message) : base(message) { }
        /// <summary>
        /// Конструктор InvalidCounterTypeException
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="ex">Внутреннее исключение</param>
        public InvalidCounterTypeException(string message, Exception ex) : base(message, ex) { }
        /// <summary>
        /// Конструктор InvalidCounterTypeException для десериализации
        /// </summary>
        /// <param name="info">info</param>
        /// <param name="context">context</param>
        protected InvalidCounterTypeException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

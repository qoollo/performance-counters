using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Исключение, когда тип счётчика неизвестен
    /// </summary>
    [Serializable]
    public class UnknownCounterTypeException : InvalidCounterTypeException
    {
        /// <summary>
        /// Конструктор UnknownCounterTypeException
        /// </summary>
        public UnknownCounterTypeException() { }
        /// <summary>
        /// Конструктор UnknownCounterTypeException
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        public UnknownCounterTypeException(string message) : base(message) { }
        /// <summary>
        /// Конструктор UnknownCounterTypeException
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="ex">Внутреннее исключение</param>
        public UnknownCounterTypeException(string message, Exception ex) : base(message, ex) { }
        /// <summary>
        /// Конструктор UnknownCounterTypeException для десериализации
        /// </summary>
        /// <param name="info">info</param>
        /// <param name="context">context</param>
        protected UnknownCounterTypeException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

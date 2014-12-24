using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Исключение при создании счётчика
    /// </summary>
    [Serializable]
    public class PerformanceCounterCreationException : PerformanceCountersException
    {
        /// <summary>
        /// Конструктор PerformanceCounterCreationException
        /// </summary>
        public PerformanceCounterCreationException() { }
        /// <summary>
        /// Конструктор PerformanceCounterCreationException
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        public PerformanceCounterCreationException(string message) : base(message) { }
        /// <summary>
        /// Конструктор PerformanceCounterCreationException
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="ex">Внутреннее исключение</param>
        public PerformanceCounterCreationException(string message, Exception ex) : base(message, ex) { }
        /// <summary>
        /// Конструктор PerformanceCounterCreationException для десериализации
        /// </summary>
        /// <param name="info">info</param>
        /// <param name="context">context</param>
        protected PerformanceCounterCreationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

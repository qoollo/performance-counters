using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Базовый класс исключения для библиотеки счётчиков
    /// </summary>
    [Serializable]
    public class PerformanceCountersException : ApplicationException
    {
        /// <summary>
        /// Конструктор PerformanceCountersException
        /// </summary>
        public PerformanceCountersException() { }
        /// <summary>
        /// Конструктор PerformanceCountersException
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        public PerformanceCountersException(string message) : base(message) { }
        /// <summary>
        /// Конструктор PerformanceCountersException
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="ex">Внутреннее исключение</param>
        public PerformanceCountersException(string message, Exception ex) : base(message, ex) { }
        /// <summary>
        /// Конструктор PerformanceCountersException для десериализации
        /// </summary>
        /// <param name="info">info</param>
        /// <param name="context">context</param>
        protected PerformanceCountersException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

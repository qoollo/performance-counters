using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Исключение для ситуации, когда указанный счётчик не существует
    /// </summary>
    [Serializable]
    public class CounterNotExistException: PerformanceCountersException
    {
        /// <summary>
        /// Конструктор CounterNotExistException
        /// </summary>
        public CounterNotExistException() { }
        /// <summary>
        /// Конструктор CounterNotExistException
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        public CounterNotExistException(string message) : base(message) { }
        /// <summary>
        /// Конструктор CounterNotExistException
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="ex">Внутреннее исключение</param>
        public CounterNotExistException(string message, Exception ex) : base(message, ex) { }
        /// <summary>
        /// Конструктор CounterNotExistException для десериализации
        /// </summary>
        /// <param name="info">info</param>
        /// <param name="context">context</param>
        protected CounterNotExistException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

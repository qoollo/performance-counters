using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Повторяющееся имя счётчика
    /// </summary>
    [Serializable]
    public class DuplicateCounterNameException : PerformanceCountersException
    {
        /// <summary>
        /// Конструктор DuplicateCounterNameException
        /// </summary>
        public DuplicateCounterNameException() { }
        /// <summary>
        /// Конструктор DuplicateCounterNameException
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        public DuplicateCounterNameException(string message) : base(message) { }
        /// <summary>
        /// Конструктор DuplicateCounterNameException
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="ex">Внутреннее исключение</param>
        public DuplicateCounterNameException(string message, Exception ex) : base(message, ex) { }
        /// <summary>
        /// Конструктор DuplicateCounterNameException для десериализации
        /// </summary>
        /// <param name="info">info</param>
        /// <param name="context">context</param>
        protected DuplicateCounterNameException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Исключение при неверном задании описания счётчика в обёртке
    /// </summary>
    [Serializable]
    public class InvalidCounterDefinitionException : PerformanceCountersException
    {
        /// <summary>
        /// Конструктор InvalidCounterDefinitionException
        /// </summary>
        public InvalidCounterDefinitionException() { }
        /// <summary>
        /// Конструктор InvalidCounterDefinitionException
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        public InvalidCounterDefinitionException(string message) : base(message) { }
        /// <summary>
        /// Конструктор InvalidCounterDefinitionException
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="ex">Внутреннее исключение</param>
        public InvalidCounterDefinitionException(string message, Exception ex) : base(message, ex) { }
        /// <summary>
        /// Конструктор InvalidCounterDefinitionException для десериализации
        /// </summary>
        /// <param name="info">info</param>
        /// <param name="context">context</param>
        protected InvalidCounterDefinitionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

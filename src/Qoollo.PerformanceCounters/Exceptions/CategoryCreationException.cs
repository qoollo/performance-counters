using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Исключение при создании категории
    /// </summary>
    [Serializable]
    public class CategoryCreationException : PerformanceCountersException
    {
        /// <summary>
        /// Конструктор CategoryCreationException
        /// </summary>
        public CategoryCreationException() { }
        /// <summary>
        /// Конструктор CategoryCreationException
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        public CategoryCreationException(string message) : base(message) { }
        /// <summary>
        /// Конструктор CategoryCreationException
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="ex">Внутреннее исключение</param>
        public CategoryCreationException(string message, Exception ex) : base(message, ex) { }
        /// <summary>
        /// Конструктор CategoryCreationException для десериализации
        /// </summary>
        /// <param name="info">info</param>
        /// <param name="context">context</param>
        protected CategoryCreationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

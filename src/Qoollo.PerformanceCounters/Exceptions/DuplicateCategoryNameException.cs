using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Повторяющееся имя категории
    /// </summary>
    [Serializable]
    public class DuplicateCategoryNameException : PerformanceCountersException
    {
        /// <summary>
        /// Конструктор DuplicateCategoryNameException
        /// </summary>
        public DuplicateCategoryNameException() { }
        /// <summary>
        /// Конструктор DuplicateCategoryNameException
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        public DuplicateCategoryNameException(string message) : base(message) { }
        /// <summary>
        /// Конструктор DuplicateCategoryNameException
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="ex">Внутреннее исключение</param>
        public DuplicateCategoryNameException(string message, Exception ex) : base(message, ex) { }
        /// <summary>
        /// Конструктор DuplicateCategoryNameException для десериализации
        /// </summary>
        /// <param name="info">info</param>
        /// <param name="context">context</param>
        protected DuplicateCategoryNameException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

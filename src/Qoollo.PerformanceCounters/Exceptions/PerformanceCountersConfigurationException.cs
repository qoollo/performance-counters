using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Ошибка загрузки конфигурации счётчиков
    /// </summary>
    [Serializable]
    public class PerformanceCountersConfigurationException: PerformanceCountersException
    {
        /// <summary>
        /// Конструктор PerformanceCountersConfigurationException
        /// </summary>
        public PerformanceCountersConfigurationException() { }
        /// <summary>
        /// Конструктор PerformanceCountersConfigurationException
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        public PerformanceCountersConfigurationException(string message) : base(message) { }
        /// <summary>
        /// Конструктор PerformanceCountersConfigurationException
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="ex">Внутреннее исключение</param>
        public PerformanceCountersConfigurationException(string message, Exception ex) : base(message, ex) { }
        /// <summary>
        /// Конструктор PerformanceCountersConfigurationException для десериализации
        /// </summary>
        /// <param name="info">info</param>
        /// <param name="context">context</param>
        protected PerformanceCountersConfigurationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

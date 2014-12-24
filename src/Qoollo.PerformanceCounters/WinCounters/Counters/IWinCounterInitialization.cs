using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.WinCounters.Counters
{
    /// <summary>
    /// Интерфейс для инициализации счётчиков WinCounters
    /// </summary>
    internal interface IWinCounterInitialization
    {
        /// <summary>
        /// Занести данные о необходимых счётчиках Windows в коллекцию
        /// </summary>
        /// <param name="col">Коллекция</param>
        void CounterFillCreationData(CounterCreationDataCollection col);
        /// <summary>
        /// Проинициализировать счётчик Windows
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="instanceName">Имя инстанса (null, если одноинстансовая категория)</param>
        void CounterInit(string categoryName, string instanceName);
        /// <summary>
        /// Освободить счётчик
        /// </summary>
        /// <param name="removeInstance">Удалить ли его инстанс из Windows</param>
        void CounterDispose(bool removeInstance);
    }
}

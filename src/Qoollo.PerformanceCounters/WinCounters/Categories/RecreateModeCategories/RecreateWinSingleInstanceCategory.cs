using Qoollo.PerformanceCounters.WinCounters.Counters;
using Qoollo.PerformanceCounters.WinCounters.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.WinCounters.Categories.RecreateModeCategories
{
    /// <summary>
    /// Категория с одним инстансом для WinCounters.
    /// Работает в режиме, когда счётчики и категории можно удалять/добавлять
    /// </summary>
    internal class RecreateWinSingleInstanceCategory : WinSingleInstanceCategory
    {
        private readonly ConcurrentDictionary<string, Counter> _counters;
        private PerformanceCounterCategory _winCategory;

        /// <summary>
        /// Конструктор RecreateWinSingleInstanceCategory
        /// </summary>
        /// <param name="name">Имя категории</param>
        /// <param name="description">Описание категории</param>
        /// <param name="rootName">Корневое имя</param>
        /// <param name="info">Информация о функционировании</param>
        internal RecreateWinSingleInstanceCategory(string name, string description, string rootName, WinCountersWorkingInfo info)
            : base(name, description, rootName, info)
        {
            if (info.InstantiationMode != WinCountersInstantiationMode.AlwaysCreateNew &&
                info.InstantiationMode != WinCountersInstantiationMode.UseExistedIfPossible)
            {
                throw new InvalidOperationException("Category RecreateWinSingleInstanceCategory can't be used with instantiation mode: " + info.InstantiationMode.ToString());
            }

            _counters = new ConcurrentDictionary<string, Counter>();
        }

        /// <summary>
        /// Перечень счётчиков
        /// </summary>
        public override IEnumerable<Counter> Counters { get { return _counters.Values; } }


        /// <summary>
        /// Создать счётчик определённого типа
        /// </summary>
        /// <param name="type">Тип счётчика</param>
        /// <param name="counterName">Имя счётчика</param>
        /// <param name="counterDescription">Описание счётчика</param>
        /// <returns>Счётчик</returns>
        public override Counter CreateCounter(CounterTypes type, string counterName, string counterDescription)
        {
            if (counterName == null)
                throw new ArgumentNullException("counterName");
            if (counterDescription == null)
                throw new ArgumentNullException("counterDescription");

            if (State != WinCategoryState.Created)
                throw new InvalidOperationException("Can't create counter inside initialized category. Category: " + this.ToString());

            if (_counters.ContainsKey(counterName))
                throw new PerformanceCounterCreationException("Counter with the same name is already existed. Name: " + counterName,
                            new DuplicateCounterNameException("Counter with the same name is already existed. Name: " + counterName));

            Counter res = CounterHelper.CreateCounter(type, counterName, counterDescription, Info);
            if (!_counters.TryAdd(counterName, res))
                throw new PerformanceCounterCreationException("Counter with the same name is already existed. Name: " + counterName,
                            new DuplicateCounterNameException("Counter with the same name is already existed. Name: " + counterName));

            return res;
        }

        /// <summary>
        /// Есть ли счётчик с указанным именем
        /// </summary>
        /// <param name="counterName">Имя счётчика</param>
        /// <returns>Есть ли он</returns>
        public override bool HasCounter(string counterName)
        {
            return _counters.ContainsKey(counterName);
        }


        /// <summary>
        /// Получение счетчика определенного типа
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <returns>Счётчик</returns>
        public override Counter GetCounter(string counterName)
        {
            if (counterName == null)
                throw new ArgumentNullException("counterName");

            Counter res = null;
            if (!_counters.TryGetValue(counterName, out res))
                throw new CounterNotExistException(string.Format("Counter ({0}) is not found in category {1}", counterName, this.ToString()));

            return res;
        }

        /// <summary>
        /// Получение счетчика определенного типа
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <param name="expectedCounterType">Тип счётчика</param>
        /// <returns>Счётчик</returns>
        public override Counter GetCounter(string counterName, CounterTypes expectedCounterType)
        {
            if (counterName == null)
                throw new ArgumentNullException("counterName");

            Counter res = null;
            if (!_counters.TryGetValue(counterName, out res))
                throw new CounterNotExistException(string.Format("Counter ({0}) is not found in category {1}", counterName, this.ToString()));

            if (res.Type != expectedCounterType)
                throw new InvalidCounterTypeException(
                    string.Format("Counter types are not equal. Expected: {0}, Returned: {1}", expectedCounterType, res.Type));

            return res;
        }

        /// <summary>
        /// Существует ли категория с тем же именем в Windows
        /// </summary>
        /// <returns>Существует ли</returns>
        private bool HasWinCategory()
        {
            return PerformanceCounterCategory.Exists(this.FullName, Info.MachineName);
        }
        /// <summary>
        /// Удалить категорию с моим именем из Windows
        /// </summary>
        private void DeleteWinCategory()
        {
            PerformanceCounterCategory.Delete(this.FullName);
        }
        /// <summary>
        /// Создание в Windows новой многоинстансовой категории
        /// </summary>
        /// <param name="counterData">Перечень счётчиков в категории</param>
        /// <returns>Созданная категория</returns>
        private PerformanceCounterCategory CreateWinCategory(CounterCreationDataCollection counterData)
        {
            return PerformanceCounterCategory.Create(this.FullName, this.Description, PerformanceCounterCategoryType.SingleInstance, counterData);
        }
        /// <summary>
        /// Получить существующую категорию из Windows с таким же именем
        /// </summary>
        /// <returns>Категория</returns>
        private PerformanceCounterCategory GetExistedWinCategory()
        {
            return new PerformanceCounterCategory(this.FullName, Info.MachineName);
        }

        /// <summary>
        /// Инициализация категории
        /// </summary>
        protected override void InitCategory()
        {
            CounterCreationDataCollection collection = new CounterCreationDataCollection();
            foreach (var counter in _counters)
                ((IWinCounterInitialization)counter.Value).CounterFillCreationData(collection);

            if (HasWinCategory())
            {
                if (Info.InstantiationMode == WinCountersInstantiationMode.AlwaysCreateNew)
                {
                    DeleteWinCategory();
                    _winCategory = CreateWinCategory(collection);
                }
                else if (Info.InstantiationMode == WinCountersInstantiationMode.UseExistedIfPossible)
                {
                    var wc = GetExistedWinCategory();
                    var existedCounters = wc.GetCounters();
                    if (wc.CategoryType != PerformanceCounterCategoryType.SingleInstance || 
                        !CategoryHelper.IsAllRequestedCountersExist(existedCounters, collection))
                    {
                        DeleteWinCategory();
                        _winCategory = CreateWinCategory(collection);
                    }
                    else
                    {
                        _winCategory = wc;
                    }

                    foreach (var cnt in existedCounters)
                        cnt.Dispose();
                }
                else
                {
                    throw new InvalidOperationException("Incorrect WinCountersInstantiationMode value: " + Info.InstantiationMode.ToString());
                }
            }
            else
            {
                _winCategory = CreateWinCategory(collection);
            }

            foreach (var counter in _counters)
                ((IWinCounterInitialization)counter.Value).CounterInit(this.FullName, null);
        }

        /// <summary>
        /// Удаление категории из Windows
        /// </summary>
        public override void RemoveCategory()
        {
            if (State == WinCategoryState.Disposed)
                throw new ObjectDisposedException(this.GetType().Name);

            this.Dispose();

            if (!Info.IsLocalMachine)
                throw new InvalidOperationException("Can't remove category on remote machine. Category: " + this.ToString());

            if (PerformanceCounterCategory.Exists(this.FullName))
                PerformanceCounterCategory.Delete(this.FullName);
        }


        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        /// <param name="isUserCall">Было ли инициировано пользователем</param>
        protected override void Dispose(bool isUserCall)
        {
            if (isUserCall)
            {
                if (State != WinCategoryState.Disposed)
                {
                    foreach (var counter in _counters)
                        ((IWinCounterInitialization)counter.Value).CounterDispose(false);
                }
            }

            base.Dispose(isUserCall);
        }
    }
}

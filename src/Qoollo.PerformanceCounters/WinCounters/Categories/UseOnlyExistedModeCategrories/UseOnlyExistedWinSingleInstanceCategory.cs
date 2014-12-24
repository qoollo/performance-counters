using Qoollo.PerformanceCounters.WinCounters.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;
using Qoollo.PerformanceCounters.WinCounters.Counters;

namespace Qoollo.PerformanceCounters.WinCounters.Categories.UseOnlyExistedModeCategrories
{
    /// <summary>
    /// Категория с одним инстансом для WinCounters.
    /// Работает в режиме, когда требуется использование только существующих счётчиков.
    /// Работает прозрачно Windows
    /// </summary>
    internal class UseOnlyExistedWinSingleInstanceCategory : WinSingleInstanceCategory
    {
        private readonly ConcurrentDictionary<string, Counter> _counters;
        private readonly PerformanceCounterCategory _winCategory;

        /// <summary>
        /// Конструктор UseOnlyExistedWinSingleInstanceCategory
        /// </summary>
        /// <param name="name">Имя категории</param>
        /// <param name="description">Описание категории</param>
        /// <param name="rootName">Корневое имя</param>
        /// <param name="info">Информация о функционировании</param>
        internal UseOnlyExistedWinSingleInstanceCategory(string name, string description, string rootName, WinCountersWorkingInfo info)
            : base(name, description, rootName, info)
        {
            if (info.InstantiationMode != WinCountersInstantiationMode.UseOnlyExisted)
                throw new InvalidOperationException("Category UseOnlyExistedWinSingleInstanceCategory can't be used with instantiation mode: " + info.InstantiationMode.ToString());

            if (!HasWinCategory())
                throw new CategoryCreationException(string.Format("Can't create category ({0}) cause it is not registerd in Windows (UseOnlyExisted mode)", this.ToString()));

            _counters = new ConcurrentDictionary<string, Counter>();

            _winCategory = GetExistedWinCategory();
            if (_winCategory.CategoryType != PerformanceCounterCategoryType.SingleInstance)
                throw new CategoryCreationException(string.Format("Can't create category ({0}) cause it's type is not equal to Windows category type ({1}) (UseOnlyExisted mode)", this.ToString(), _winCategory.CategoryType));

            var existedCntr = _winCategory.GetCounters();

            foreach (var cnt in existedCntr)
            {
                var newCounter = CounterHelper.CreateByExistedCounterAndInit(cnt, Info);
                if (newCounter != null)
                    _counters.TryAdd(newCounter.Name, newCounter);

                cnt.Dispose();
            }

            this.Init();
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
        /// Получить существующую категорию из Windows с таким же именем
        /// </summary>
        /// <returns>Категория</returns>
        private PerformanceCounterCategory GetExistedWinCategory()
        {
            return new PerformanceCounterCategory(this.FullName, Info.MachineName);
        }

        /// <summary>
        /// Перечень счётчиков
        /// </summary>
        public override IEnumerable<Counter> Counters { get { return _counters.Values; } }


        /// <summary>
        /// Создать счётчик определённого типа, если он уже есть в Windows
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

            if (State == WinCategoryState.Disposed)
                throw new ObjectDisposedException(this.GetType().Name);

            Counter res = null;
            if (!_counters.TryGetValue(counterName, out res))
                throw new PerformanceCounterCreationException("Can't create not existed counter in mode 'UseOnlyExisted'. Counter: " + counterName + ", Category: " + this.ToString());

            if (res.Type != type && CounterHelper.IsWinCompatible(res.Type, type))
            {
                var newCntr = CounterHelper.CreateCounter(type, counterName, res.Description, Info);
                if (_counters.TryUpdate(counterName, newCntr, res))
                {
                    (newCntr as IWinCounterInitialization).CounterInit(this.FullName, null);
                    res = newCntr;
                }
            }

            if (res.Type != type)
            {
                throw new PerformanceCounterCreationException("Can't create not existed counter in mode 'UseOnlyExisted'. Counter: " + counterName,
                    new InvalidCounterTypeException(string.Format("Counter types are not equal. Expected: {0}, Returned: {1}", type, res.Type)));
            }

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
        /// Удаление категории из Windows. Не поддерживается.
        /// </summary>
        /// <exception cref="InvalidOperationException">Всегда</exception>
        public override void RemoveCategory()
        {
            throw new InvalidOperationException("Can't remove category in UseOnlyExisted mode");
        }

        /// <summary>
        /// Инициализация категории
        /// </summary>
        protected override void InitCategory()
        {
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

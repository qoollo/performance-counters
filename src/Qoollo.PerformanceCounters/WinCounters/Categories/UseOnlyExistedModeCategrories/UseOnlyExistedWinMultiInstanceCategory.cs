using Qoollo.PerformanceCounters.WinCounters.Counters;
using Qoollo.PerformanceCounters.WinCounters.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.WinCounters.Categories.UseOnlyExistedModeCategrories
{
    /// <summary>
    /// Категория, поддерживающая многочисленные инстансы для WinCounters.
    /// Работает в режиме, когда требуется использование только существующих счётчиков.
    /// Работает прозрачно Windows
    /// </summary>
    internal class UseOnlyExistedWinMultiInstanceCategory : WinMultiInstanceCategory
    {
        private readonly ConcurrentDictionary<string, WinInstanceInMultiInstanceCategory> _instances;
        private readonly ConcurrentDictionary<string, WinCounterDescriptor> _counters;
        private PerformanceCounterCategory _winCategory;

        /// <summary>
        /// Конструктор UseOnlyExistedWinMultiInstanceCategory
        /// </summary>
        /// <param name="name">Имя категории</param>
        /// <param name="description">Описание категории</param>
        /// <param name="rootName">Корневое имя</param>
        /// <param name="info">Информация о функционировании</param>
        internal UseOnlyExistedWinMultiInstanceCategory(string name, string description, string rootName, WinCountersWorkingInfo info)
            : base(name, description, rootName, info)
        {
            if (info.InstantiationMode != WinCountersInstantiationMode.UseOnlyExisted)
                throw new InvalidOperationException("Category UseOnlyExistedWinSingleInstanceCategory can't be used with instantiation mode: " + info.InstantiationMode.ToString());

            if (!HasWinCategory())
                throw new CategoryCreationException(string.Format("Can't create category ({0}) cause it is not registerd in Windows (UseOnlyExisted mode)", this.ToString()));

            _instances = new ConcurrentDictionary<string, WinInstanceInMultiInstanceCategory>();
            _counters = new ConcurrentDictionary<string, WinCounterDescriptor>();

            _winCategory = GetExistedWinCategory();
            var existedCntr = _winCategory.GetCounters("");

            foreach (var cnt in existedCntr)
            {
                var newCounter = CounterHelper.CreateDescriptorByExistedCounter(cnt, Info);
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
        /// Есть ли в Windows инстанс с указанным именем
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Есть ли</returns>
        private bool HasWinInstance(string instanceName)
        {
            return PerformanceCounterCategory.InstanceExists(instanceName, this.FullName, Info.MachineName);
        }

        /// <summary>
        /// Перечень инстансов
        /// </summary>
        public override IEnumerable<InstanceInMultiInstanceCategory> Instances { get { return _instances.Values; } }
        /// <summary>
        /// Перечень описателей счётчиков для внутреннего использования
        /// </summary>
        internal override IEnumerable<WinCounterDescriptor> Counters { get { return _counters.Values; } }

        /// <summary>
        /// Получение инстанса
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Инстанс</returns>
        public override InstanceInMultiInstanceCategory this[string instanceName]
        {
            get
            {
                WinInstanceInMultiInstanceCategory result = null;
                if (!_instances.TryGetValue(instanceName, out result))
                {
                    if (!HasWinInstance(instanceName))
                        throw new CategoryCreationException("Can't get not existed instance in mode 'UseOnlyExisted'. Category: " + this.ToString() + ", Instance: " + instanceName);

                    result = new WinInstanceInMultiInstanceCategory(this, instanceName);
                }

                if (result.State == WinCategoryState.Created)
                    result.Init();

                return result;
            }
        }

        /// <summary>
        /// Существует ли инстанс счётчиков
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Существует ли</returns>
        public override bool HasInstance(string instanceName)
        {
            if (_instances.ContainsKey(instanceName))
                return true;

            return HasWinInstance(instanceName);
        }

        /// <summary>
        /// Удалить инстанс. Удаляется только из внутреннего списка. В Windows всё остаётся.
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Существал ли</returns>
        public override bool RemoveInstance(string instanceName)
        {
            WinInstanceInMultiInstanceCategory inst = null;
            if (_instances.TryRemove(instanceName, out inst))
            {
                inst.OnRemoveFromMultiInstanceCategory(false);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Создать счётчик определённого типа. 
        /// Создаётся только если есть такой в Windows и был найден при создании.
        /// </summary>
        /// <param name="type">Тип счётчика</param>
        /// <param name="counterName">Имя счетчика</param>
        /// <param name="counterDescription">Описание счетчика</param>
        public override void CreateCounter(CounterTypes type, string counterName, string counterDescription)
        {
            if (counterName == null)
                throw new ArgumentNullException("counterName");
            if (counterDescription == null)
                throw new ArgumentNullException("counterDescription");

            if (State == WinCategoryState.Disposed)
                throw new ObjectDisposedException(this.GetType().Name);


            WinCounterDescriptor res = null;
            if (!_counters.TryGetValue(counterName, out res))
                throw new PerformanceCounterCreationException("Can't create not existed counter in mode 'UseOnlyExisted'. Counter: " + counterName + ", Category: " + this.ToString());

            if (res.Type != type && CounterHelper.IsWinCompatible(res.Type, type))
            {
                var newCntr = CounterHelper.CreateCounterDescriptor(type, counterName, res.Description, Info);
                if (_counters.TryUpdate(counterName, newCntr, res))
                    res = newCntr;
            }

            if (res.Type != type)
            {
                throw new PerformanceCounterCreationException("Can't create not existed counter in mode 'UseOnlyExisted'. Counter: " + counterName,
                    new InvalidCounterTypeException(string.Format("Counter types are not equal. Expected: {0}, Returned: {1}", type, res.Type)));
            }
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
        /// Удалить все инстансы в многоинстансовой категории (в том числе и из Windows).  Не поддерживается.
        /// </summary>
        /// <exception cref="InvalidOperationException">Всегда</exception>
        public override void RemoveInstances()
        {
            throw new InvalidOperationException("Can't remove instances from MultiInstanceCategory in UseOnlyExisted mode");
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
                    var instances = _instances.ToArray();
                    _instances.Clear();
                    foreach (var inst in instances)
                        inst.Value.OnRemoveFromMultiInstanceCategory(false);
                }
            }
            base.Dispose(isUserCall);
        }
    }
}

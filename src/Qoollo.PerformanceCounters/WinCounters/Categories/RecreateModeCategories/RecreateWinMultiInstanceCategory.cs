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
    /// Категория, поддерживающая многочисленные инстансы для WinCounters.
    /// Работает в режиме, когда счётчики и категории можно удалять/добавлять
    /// </summary>
    internal class RecreateWinMultiInstanceCategory : WinMultiInstanceCategory
    {
        private readonly ConcurrentDictionary<string, WinInstanceInMultiInstanceCategory> _instances;
        private readonly ConcurrentDictionary<string, WinCounterDescriptor> _counters;
        private PerformanceCounterCategory _winCategory;

        /// <summary>
        /// Конструктор RecreateWinMultiInstanceCategory
        /// </summary>
        /// <param name="name">Имя категории</param>
        /// <param name="description">Описание категории</param>
        /// <param name="rootName">Корневое имя</param>
        /// <param name="info">Информация о функционировании</param>
        internal RecreateWinMultiInstanceCategory(string name, string description, string rootName, WinCountersWorkingInfo info)
            : base(name, description, rootName, info)
        {
            if (info.InstantiationMode != WinCountersInstantiationMode.AlwaysCreateNew &&
                info.InstantiationMode != WinCountersInstantiationMode.UseExistedIfPossible)
            {
                throw new InvalidOperationException("Category RecreateWinMultiInstanceCategory can't be used with instantiation mode: " + info.InstantiationMode.ToString());
            }

            _instances = new ConcurrentDictionary<string, WinInstanceInMultiInstanceCategory>();
            _counters = new ConcurrentDictionary<string, WinCounterDescriptor>();
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
        /// Получение или создание инстанса
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Инстанс</returns>
        public override InstanceInMultiInstanceCategory this[string instanceName]
        {
            get
            {
                var result = _instances.GetOrAdd(instanceName, (name) => new WinInstanceInMultiInstanceCategory(this, name));
                if (result.State == WinCategoryState.Created && State == WinCategoryState.Initialized)
                    result.Init();

                return result;
            }
        }

        /// <summary>
        /// Добавление нового инстанса, если его нет
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Был ли добавлен (false, если уже существовал)</returns>
        private bool TryAddInstance(string instanceName)
        {
            if (_instances.ContainsKey(instanceName))
                return false;

            var inst = new WinInstanceInMultiInstanceCategory(this, instanceName);
            if (!_instances.TryAdd(instanceName, inst))
                return false;

            inst.Init();
            return true;
        }

        /// <summary>
        /// Существует ли инстанс счётчиков
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Существует ли</returns>
        public override bool HasInstance(string instanceName)
        {
            return _instances.ContainsKey(instanceName);
        }

        /// <summary>
        /// Удалить инстанс
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Существал ли</returns>
        public override bool RemoveInstance(string instanceName)
        {
            WinInstanceInMultiInstanceCategory inst = null;
            if (_instances.TryRemove(instanceName, out inst))
            {
                inst.OnRemoveFromMultiInstanceCategory(true);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Создать счётчик определённого типа
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

            if (State != WinCategoryState.Created)
                throw new InvalidOperationException("Can't create counter inside initialized category. Category: " + this.ToString());

            if (_instances.Count != 0)
            {
                throw new PerformanceCounterCreationException(
                    string.Format("Can't create counter in MultiInstanceCategory ({0}) when it has created instances", this.ToString()));
            }

            if (_counters.ContainsKey(counterName))
                throw new PerformanceCounterCreationException("Counter with the same name is already existed. Name: " + counterName,
                            new DuplicateCounterNameException("Counter with the same name is already existed. Name: " + counterName));


            WinCounterDescriptor res = CounterHelper.CreateCounterDescriptor(type, counterName, counterDescription, Info);
            if (!_counters.TryAdd(counterName, res))
                throw new PerformanceCounterCreationException("Counter with the same name is already existed. Name: " + counterName,
                            new DuplicateCounterNameException("Counter with the same name is already existed. Name: " + counterName));
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
            return PerformanceCounterCategory.Create(this.FullName, this.Description, PerformanceCounterCategoryType.MultiInstance, counterData);
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
                counter.Value.FillCounterCreationData(collection);

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
                    var existedCounters = wc.GetCounters("");
                    if (wc.CategoryType != PerformanceCounterCategoryType.MultiInstance ||
                        !CategoryHelper.IsAllRequestedCountersExist(existedCounters, collection))
                    {
                        DeleteWinCategory();
                        _winCategory = CreateWinCategory(collection);
                    }
                    else
                    {
                        _winCategory = wc;

                        if (Info.ExistedInstancesTreatment == WinCountersExistedInstancesTreatment.LoadExisted)
                        {
                            foreach (var instName in wc.GetInstanceNames())
                                TryAddInstance(instName);
                        }
                        else if (Info.ExistedInstancesTreatment == WinCountersExistedInstancesTreatment.RemoveExisted)
                        {
                            foreach (var instName in wc.GetInstanceNames())
                            {
                                if (!HasInstance(instName))
                                {
                                    WinInstanceInMultiInstanceCategory inst = new WinInstanceInMultiInstanceCategory(this, instName);
                                    inst.Init();
                                    inst.OnRemoveFromMultiInstanceCategory(true);
                                }
                            }
                        }
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

            foreach (var inst in _instances)
                inst.Value.Init();
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
        /// Удалить все инстансы в многоинстансовой категории (в том числе и из Windows)
        /// </summary>
        public override void RemoveInstances()
        {
            if (State == WinCategoryState.Disposed)
                throw new ObjectDisposedException(this.GetType().Name);

            var categoryCpy = _winCategory;
            if (categoryCpy == null)
            {
                if (!HasWinCategory())
                    return;
                categoryCpy = GetExistedWinCategory();
            }

            var instances = _instances.ToArray();
            _instances.Clear();
            foreach (var inst in instances)
                inst.Value.OnRemoveFromMultiInstanceCategory(true);


            var allInst = categoryCpy.GetInstanceNames();
            for (int i = 0; i < allInst.Length; i++)
            {
                var allCounters = categoryCpy.GetCounters(allInst[i]);
                foreach (var counter in allCounters)
                    counter.RemoveInstance();
            }
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

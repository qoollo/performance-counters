using Qoollo.PerformanceCounters.WinCounters.Counters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.WinCounters.Categories
{
    /// <summary>
    /// Инстанс в многоинстансовой категории для WinCounters
    /// </summary> 
    public class WinInstanceInMultiInstanceCategory : InstanceInMultiInstanceCategory
    {
        private volatile WinCategoryState _state;
        private readonly Dictionary<string, Counter> _counters;

        /// <summary>
        /// Конструктор для создания инстанса WinInstanceInMultiInstanceCategory
        /// </summary>
        /// <param name="parent">Родительская многоинстовая категория</param>
        /// <param name="instanceName">Имя инстанса</param>
        internal WinInstanceInMultiInstanceCategory(WinMultiInstanceCategory parent, string instanceName)
            : base(parent, instanceName)
        {
            _state = WinCategoryState.Created;
            _counters = parent.Counters.ToDictionary(key => key.Name, val => val.CreateCounter());
        }

        /// <summary>
        /// Состояние
        /// </summary>
        public WinCategoryState State { get { return _state; } }
        /// <summary>
        /// Перечень счётчиков
        /// </summary>
        public IEnumerable<Counter> Counters { get { return _counters.Values; } }


        /// <summary>
        /// Внутренний метод для инициализации
        /// </summary>
        internal void Init()
        {
            if (_state == WinCategoryState.Disposed)
                throw new ObjectDisposedException(this.GetType().Name);
            if (_state == WinCategoryState.Initialized)
                return;

            _state = WinCategoryState.Initialized;

            foreach (var counter in _counters)
                ((IWinCounterInitialization)counter.Value).CounterInit(((WinMultiInstanceCategory)this.Parent).FullName, this.InstanceName);
        }

        /// <summary>
        /// Вызывается из родительской MultiInstanceCategory при удалении данного инстанса
        /// </summary>
        /// <param name="removeInstance">Удалять ли данный инстанс в Windows</param>
        internal void OnRemoveFromMultiInstanceCategory(bool removeInstance)
        {
            _state = WinCategoryState.Disposed;
            foreach (var counter in _counters)
                ((IWinCounterInitialization)counter.Value).CounterDispose(removeInstance);

  
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
        /// Получение счетчика по имени
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <returns>Счётчик</returns>
        public override Counter GetCounter(string counterName)
        {
            if (_state == WinCategoryState.Disposed)
                throw new ObjectDisposedException(this.GetType().Name);

            Counter res = null;
            if (!_counters.TryGetValue(counterName, out res))
                throw new CounterNotExistException(string.Format("Counter ({0}) is not found in instance {1} of category {2}", counterName, this.InstanceName, this.Parent.Name));

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
            if (_state == WinCategoryState.Disposed)
                throw new ObjectDisposedException(this.GetType().Name);

            Counter res = null;
            if (!_counters.TryGetValue(counterName, out res))
                throw new CounterNotExistException(string.Format("Counter ({0}) is not found in instance {1} of category {2}", counterName, this.InstanceName, this.Parent.Name));

            if (res.Type != expectedCounterType)
                throw new InvalidCounterTypeException(
                    string.Format("Counter types are not equal. Expected: {0}, Returned: {1}", expectedCounterType, res.Type));

            return res;
        }

        /// <summary>
        /// Удаление инстанса
        /// </summary>
        public override void Remove()
        {
            var par = this.Parent as WinMultiInstanceCategory;
            if (par != null)
                par.RemoveInstance(this.InstanceName);
            else
                OnRemoveFromMultiInstanceCategory(false);
        }

        /// <summary>
        /// Живой ли инстанс (не был ли удалён)
        /// </summary>
        public override bool IsAlive
        {
            get { return _state != WinCategoryState.Disposed; }
        }
    }
}

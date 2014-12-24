using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.CompositeCounters.Categories
{
    /// <summary>
    /// Инстанс в многоинстансовой категории для CompositeCounters
    /// </summary> 
    public class CompositeInstanceInMultiInstanceCategory : InstanceInMultiInstanceCategory
    {
        private bool _isAlive = true;
        private readonly List<InstanceInMultiInstanceCategory> _wrappedInstances;
        private readonly Dictionary<string, Counter> _counters;

        /// <summary>
        /// Конструктор для создания инстанса CompositeInstanceInMultiInstanceCategory
        /// </summary>
        /// <param name="parent">Родительская многоинстовая категория</param>
        /// <param name="instanceName">Имя инстанса</param>
        /// <param name="wrappedInstances">Оборачиваемые инстансы</param>
        public CompositeInstanceInMultiInstanceCategory(CompositeMultiInstanceCategory parent, string instanceName, IEnumerable<InstanceInMultiInstanceCategory> wrappedInstances)
            : base(parent, instanceName)
        {
            if (wrappedInstances == null)
                throw new ArgumentNullException("wrappedInstances");

            _wrappedInstances = new List<InstanceInMultiInstanceCategory>(wrappedInstances);
            _counters = parent.Counters.ToDictionary(key => key.Key, val => val.Value.CreateCounter(_wrappedInstances.Select(o => o.GetCounter(val.Value.Name, val.Value.Type))));
        }

        /// <summary>
        /// Перечень счётчиков
        /// </summary>
        public IEnumerable<Counter> Counters { get { return _counters.Values; } }
        /// <summary>
        /// Обёрнутые инстансы
        /// </summary>
        public IEnumerable<InstanceInMultiInstanceCategory> WrappedInstances { get { return _wrappedInstances; } }

        /// <summary>
        /// Вызывается из родительской MultiInstanceCategory при удалении данного инстанса
        /// </summary>
        internal void OnRemoveFromMultiInstanceCategory()
        {
            _isAlive = false;
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
            var par = this.Parent as CompositeMultiInstanceCategory;
            if (par != null)
                par.RemoveInstance(this.InstanceName);

            _isAlive = false;
        }


        /// <summary>
        /// Живой ли инстанс (не был ли удалён)
        /// </summary>
        public override bool IsAlive
        {
            get { return _isAlive; }
        }
    }
}

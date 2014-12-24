using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.InternalCounters.Categories
{
    /// <summary>
    /// Инстанс в многоинстансовой категории для InternalCounters
    /// </summary> 
    public class InternalInstanceInMultiInstanceCategory : InstanceInMultiInstanceCategory
    {
        private bool _isAlive = true;
        private readonly Dictionary<string, Counter> _counters;

        /// <summary>
        /// Конструктор для создания инстанса InternalInstanceInMultiInstanceCategory
        /// </summary>
        /// <param name="parent">Родительская многоинстовая категория</param>
        /// <param name="instanceName">Имя инстанса</param>
        public InternalInstanceInMultiInstanceCategory(InternalMultiInstanceCategory parent, string instanceName)
            : base(parent, instanceName)
        {
            _counters = parent.Counters.ToDictionary(key => key.Key, val => val.Value.CreateCounter());
        }

        /// <summary>
        /// Перечень счётчиков
        /// </summary>
        public IEnumerable<Counter> Counters { get { return _counters.Values; } }

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
            var par = this.Parent as InternalMultiInstanceCategory;
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

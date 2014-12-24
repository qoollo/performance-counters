using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NullCounters.Categories
{
    /// <summary>
    /// Создание категории поддерживающей многочисленные инстансы для NullCounters
    /// </summary>
    public class NullMultiInstanceCategory: MultiInstanceCategory
    {
        /// <summary>
        /// Конструктор категории NullMultiInstanceCategory
        /// </summary>
        /// <param name="name">Имя категории</param>
        /// <param name="description">Описание категории</param>
        public NullMultiInstanceCategory(string name, string description)
            : base(name, description)
        {
        }

        /// <summary>
        /// Создание инстанса
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Инстанс</returns>
        public override InstanceInMultiInstanceCategory this[string instanceName]
        {
            get { return new NullInstanceInMultiInstanceCategory(this, instanceName); }
        }


        /// <summary>
        /// Существует ли инстанс счётчиков. Перманентно true.
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Существует ли</returns>
        public override bool HasInstance(string instanceName)
        {
            return true;
        }

        /// <summary>
        /// Удалить инстанс. Ничего не делает.
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Существал ли</returns>
        public override bool RemoveInstance(string instanceName)
        {
            return true;
        }

        /// <summary>
        /// Создать счётчик определённого типа
        /// </summary>
        /// <param name="type">Тип счётчика</param>
        /// <param name="counterName">Имя счетчика</param>
        /// <param name="counterDescription">Описание счетчика</param>
        public override void CreateCounter(CounterTypes type, string counterName, string counterDescription)
        {
        }


        /// <summary>
        /// Создание пустой подкатегории
        /// Для добавления элемента в названии категории - Name.SubName.SubSubName
        /// </summary>
        /// <param name="categoryName">Имя подкатегории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Созданная подкатегория</returns>
        public override EmptyCategory CreateEmptySubCategory(string categoryName, string categoryDescription)
        {
            return new NullEmptyCategory(categoryName, categoryDescription);
        }

        /// <summary>
        /// Создание подкатегории c многими инстансами
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Созданная подкатегория</returns>
        public override MultiInstanceCategory CreateMultiInstanceSubCategory(string categoryName, string categoryDescription)
        {
            return new NullMultiInstanceCategory(categoryName, categoryDescription);
        }

        /// <summary>
        /// Создание простой подкатегории
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Созданная подкатегория</returns>
        public override SingleInstanceCategory CreateSingleInstanceSubCategory(string categoryName, string categoryDescription)
        {
            return new NullSingleInstanceCategory(categoryName, categoryDescription);
        }
    }
}

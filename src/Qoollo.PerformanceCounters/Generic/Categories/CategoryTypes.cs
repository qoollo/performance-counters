namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Типы категорий
    /// </summary>
    public enum CategoryTypes
    {
        /// <summary>
        /// Категория без счётчиков
        /// </summary>
        Empty,
        /// <summary>
        /// Простая категория
        /// </summary>
        SingleInstance,
        /// <summary>
        /// Категория с множественными инстансами
        /// </summary>
        MultiInstance
    }
}
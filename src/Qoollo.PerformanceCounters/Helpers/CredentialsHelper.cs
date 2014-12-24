using System;
using System.Security.Principal;

namespace Qoollo.PerformanceCounters.Helpers
{
    /// <summary>
    /// Класс который может проветить является ли пользователь, запустивший приложение Администратором
    /// Не уверен что это нужно
    /// так как при создании счетчиков и без того кинется говорящий Exception
    /// Просто это есть
    /// </summary>
    public static class CredentialsHelper
    {
        /// <summary>
        /// Проверяем является ли пользователь, запустивший приложение Администратором
        /// </summary>
        /// <returns></returns>
        public static bool IsUserAdministrator()
        {
            bool isAdmin;

            try
            {
                // получаем сведения о текущем пользователем
                var user = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException)
            {
                isAdmin = false;
            }
            catch (Exception)
            {
                isAdmin = false;
            }

            return isAdmin;
        }
    }
}

using System.Collections.Generic;

namespace GameDashboardProject.Application.Abstractions
{
    public interface IUserSessionService
    {
        void InitializeSession(string userId);
        void AddUserAction(string userId, string action);
        List<string> GetUserActions(string userId);
        void ClearSession(string userId);
    }
}

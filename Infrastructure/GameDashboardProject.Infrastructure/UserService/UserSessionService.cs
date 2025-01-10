using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using GameDashboardProject.Application.Abstractions;

namespace GameDashboardProject.Application.Services
{
    public class UserSessionService : IUserSessionService
    {
        private readonly IMemoryCache _cache;
        private const string CacheKeyPrefix = "UserSession_";

        public UserSessionService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void InitializeSession(string userId)
        {
            var key = $"{CacheKeyPrefix}{userId}";
            _cache.Set(key, new List<string>(), TimeSpan.FromHours(1));
        }

        public void AddUserAction(string userId, string action)
        {
            var key = $"{CacheKeyPrefix}{userId}";
            if (_cache.TryGetValue(key, out List<string> actions))
            {
                actions.Add(action);
                _cache.Set(key, actions);
            }
        }

        public List<string> GetUserActions(string userId)
        {
            var key = $"{CacheKeyPrefix}{userId}";
            return _cache.TryGetValue(key, out List<string> actions) ? actions : new List<string>();
        }

        public void ClearSession(string userId)
        {
            var key = $"{CacheKeyPrefix}{userId}";
            _cache.Remove(key);
        }
    }
}

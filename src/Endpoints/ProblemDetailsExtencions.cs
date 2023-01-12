using Flunt.Notifications;
using System.Collections.Generic;

namespace OrderingPlatform.Endpoints;

public static class ProblemDetailsExtencions
{
    public static Dictionary<string, string[]> ConvertToProblemDetails(this IReadOnlyCollection<Notification> notifications)
    {
        return notifications
            .GroupBy(erro => erro.Key)
            .ToDictionary(erro => erro.Key, erro => erro.Select(_erro => _erro.Message).ToArray());
    }
}

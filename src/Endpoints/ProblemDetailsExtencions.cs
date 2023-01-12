using Flunt.Notifications;
using Microsoft.AspNetCore.Identity;
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

    public static Dictionary<string, string[]> ConvertToProblemDetails(this IEnumerable<IdentityError> error)
    {
        return error
            .GroupBy(e => e.Code)
            .ToDictionary(e => e.Key, e => e.Select(x => x.Description).ToArray());
    }
}

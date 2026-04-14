using LMS.Shared.DTOs.Activity;
using LMS.Shared.DTOs.Module;

namespace LMS.Blazor.Client.Shared.Presentation;

public static class CourseDisplayHelper
{
    private static readonly (string BgClass, string TextClass)[] ActivityColorVariants =
    [
        ("bg-primary-subtle", "text-primary"),
        ("bg-success-subtle", "text-success"),
        ("bg-warning-subtle", "text-warning"),
        ("bg-info-subtle", "text-info"),
        ("bg-danger-subtle", "text-danger"),
        ("bg-secondary-subtle", "text-secondary")
    ];

    public static int GetModuleSortOrder(ModuleDto module)
    {
        var today = DateTime.Today;

        if (module.StartDate.Date <= today && module.EndDate.Date >= today)
            return 0;

        if (module.StartDate.Date > today)
            return 1;

        return 2;
    }

    public static string GetModuleStatus(ModuleDto module)
    {
        var today = DateTime.Today;

        if (module.EndDate.Date < today)
            return "Completed";

        if (module.StartDate.Date > today)
            return "Upcoming";

        return "Ongoing";
    }

    public static string GetModuleStatusBadgeClass(ModuleDto module)
    {
        var today = DateTime.Today;

        if (module.EndDate.Date < today)
            return "bg-success-subtle text-success";

        if (module.StartDate.Date > today)
            return "bg-secondary-subtle text-secondary";

        return "bg-primary-subtle text-primary";
    }

    public static string GetModuleCircleStyle(ModuleDto module)
    {
        var today = DateTime.Today;

        if (module.EndDate.Date < today)
            return "background-color:#d1e7dd; color:#0f5132;";

        if (module.StartDate.Date > today)
            return "background-color:#e2e3e5; color:#41464b;";

        return "background-color:#cfe2ff; color:#084298;";
    }

    public static Dictionary<string, (string BgClass, string TextClass)> BuildActivityTypeColorMap(
        IEnumerable<ActivityDto> activities)
    {
        var allTypes = activities
            .Select(a => a.TypeName?.Trim())
            .Where(type => !string.IsNullOrWhiteSpace(type))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(type => type)
            .ToList();

        var map = new Dictionary<string, (string BgClass, string TextClass)>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < allTypes.Count; i++)
        {
            map[allTypes[i]!] = ActivityColorVariants[i % ActivityColorVariants.Length];
        }

        return map;
    }

    public static (string BgClass, string TextClass) GetActivityColorClass(
        ActivityDto activity,
        Dictionary<string, (string BgClass, string TextClass)> activityTypeColorMap)
    {
        if (!string.IsNullOrWhiteSpace(activity.TypeName) &&
            activityTypeColorMap.TryGetValue(activity.TypeName, out var color))
        {
            return color;
        }

        return ("bg-secondary-subtle", "text-secondary");
    }
}

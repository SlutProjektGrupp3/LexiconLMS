using Microsoft.AspNetCore.Components;
using System.Threading;
using System.Linq;
using LMS.Shared.DTOs.Course;
using LMS.Shared.DTOs.Module;
using LMS.Blazor.Client.Services;

namespace LMS.Blazor.Client.Pages.Teacher
{
    public partial class Dashboard : ComponentBase
    {
        [Inject] protected IApiService ApiService { get; set; } = default!;
        [Inject] protected NavigationManager Navigation { get; set; } = default!;

        protected bool isLoading = true;
        protected string? errorMessage;

        protected bool _showCreateForm;
        protected bool _showSuccessMessage;
        protected string? successMessage;
        protected int TotalStudents { get; set; }

        protected List<CourseDetailsDto> Courses { get; set; } = new();
        protected List<ModuleDto> modulesList { get; set; } = new();

        protected int currentPage = 1;
        protected int pageSize = 12;
        protected int totalCount = 0;
        protected string? searchText;
        protected string activeFilter = "all";

        protected bool showParticipantsModal;
        protected bool showModulesModal;

        protected override async Task OnInitializedAsync()
        {
            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var q = uri.Query;
            if (!string.IsNullOrWhiteSpace(q))
            {
                var dict = new Dictionary<string, string>();
                var trimmed = q.TrimStart('?');
                foreach (var part in trimmed.Split('&', StringSplitOptions.RemoveEmptyEntries))
                {
                    var kv = part.Split('=', 2);
                    var key = Uri.UnescapeDataString(kv[0]);
                    var val = kv.Length > 1 ? Uri.UnescapeDataString(kv[1]) : string.Empty;
                    dict[key] = val;
                }

                if (dict.TryGetValue("search", out var s))
                    searchText = s;

                if (dict.TryGetValue("active", out var a))
                    activeFilter = a == "true" ? "active" : a == "false" ? "inactive" : "all";

                if (dict.TryGetValue("page", out var p) && int.TryParse(p, out var parsedPage))
                    currentPage = parsedPage;
            }

            await LoadCourses();
            await LoadStudentCount();
        }

        protected async Task LoadStudentCount()
        {
            try
            {
                TotalStudents = await ApiService.GetAsync<int>($"api/users/count-by-role/{Uri.EscapeDataString("Student")} ");
            }
            catch
            {
                TotalStudents = 0;
            }
        }

        protected async Task LoadCourses(int? page = null)
        {
            isLoading = true;
            errorMessage = null;

            if (page.HasValue)
                currentPage = page.Value;

            try
            {
                var activeQuery = activeFilter == "active" ? "true" : activeFilter == "inactive" ? "false" : (string?)null;
                var url = $"api/courses/summary?page={currentPage}&pageSize={pageSize}";
                if (!string.IsNullOrWhiteSpace(searchText))
                    url += $"&search={Uri.EscapeDataString(searchText)}";
                if (activeQuery is not null)
                    url += $"&active={activeQuery}";

                var qs = new List<string> { $"page={currentPage}" };
                if (!string.IsNullOrWhiteSpace(searchText)) qs.Add($"search={Uri.EscapeDataString(searchText)}");
                if (activeQuery is not null) qs.Add($"active={activeQuery}");
                var navUri = "/teacher/dashboard" + (qs.Count > 0 ? "?" + string.Join("&", qs) : string.Empty);
                // Only navigate if the current URI differs from the target to avoid redirect loops
                var currentRelative = Navigation.ToBaseRelativePath(Navigation.Uri);
                var navUriRelative = navUri.TrimStart('/');
                if (!string.Equals(currentRelative, navUriRelative, StringComparison.OrdinalIgnoreCase))
                {
                    Navigation.NavigateTo(navUri, forceLoad: false);
                }

                var dto = await ApiService.GetAsync<CourseSummaryPagedDto>(url);
                // Map returned CourseSummaryDto items (from controller) to CourseDetailsDto used in UI
                Courses = dto?.Items.Select(i => new CourseDetailsDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    StartDate = i.StartDate,
                    EndDate = i.EndDate,
                    ParticipantsCount = i.ParticipantsCount,
                    ModulesCount = i.ModulesCount,
                    Active = i.Active
                }).ToList() ?? new();
                totalCount = dto?.Total ?? 0;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                isLoading = false;
            }
        }

        protected Task OnActiveChanged(ChangeEventArgs e)
        {
            activeFilter = e.Value?.ToString() ?? "all";
            return Task.CompletedTask;
        }

        private CancellationTokenSource? _searchCts;
        protected async Task OnSearchDebounced(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString();

            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            try
            {
                await Task.Delay(400, _searchCts.Token);
                await LoadCourses(1);
            }
            catch (TaskCanceledException) { }
        }

        protected Task ToggleCreateForm()
        {
            successMessage = null;
            _showCreateForm = !_showCreateForm;
            return Task.CompletedTask;
        }

        protected Task HandleCourseCreated()
        {
            _showCreateForm = false;
            successMessage = "Course added.";
            return Task.CompletedTask;
        }

        protected Task HandleCancelCreate()
        {
            _showCreateForm = false;
            return Task.CompletedTask;
        }

        protected int GetActiveCoursesNumber(IEnumerable<CourseDetailsDto>? courses = null)
        {
            var list = courses ?? Courses;
            return list?.Count(c => c.Active ?? false) ?? 0;
        }
    }
}

using NextStopAPIs.DTOs;

namespace NextStopAPIs.Services
{
    public interface IAdminDashboardService
    {
        Task<bool> AssignRole(AssignRoleDTO assignRoleDto);
        Task<ReportDTO> GenerateReports(GenerateReportsDTO generateReportsDto);
    }
}

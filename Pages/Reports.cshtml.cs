using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Coursework.Pages;

[Authorize]
public class ReportsModel : LoginModel
{
    public ReportsModel(UserManager<ApplicationUser> user, AppDbContext context) : base(user, context)
    {

    }

    public async Task<List<Report>> GetReports()
    {
        var user = await this.GetUser();
        if (user == null) return [];
        var userId = user.Id;
        return _context.Reports.Where(p => p.UserId == userId).ToList();
    }
}
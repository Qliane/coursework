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

    public async Task<ActionResult> OnGetDelete(int Id)
    {
        var e = _context.Reports.FirstOrDefault(p => p.Id == Id);
        if (e == null) return new NotFoundResult();
        _context.Reports.Remove(e);
        _context.SaveChanges();
        return new RedirectResult("/Reports");
    }
    public async Task<ActionResult> OnGetDownload(int Id)
    {
        var e = _context.Reports.FirstOrDefault(p => p.Id == Id);
        if (e == null) return new NotFoundResult();
        return Content(e.HTML, "text/html");
    }
    public async Task<List<Report>> GetReports()
    {
        var user = await this.GetUser();
        if (user == null) return [];
        var userId = user.Id;
        return _context.Reports.Where(p => p.UserId == userId)
                .OrderByDescending(p => p.createdAt)   // Сначала новые (по убыванию даты);
                .ToList();
    }
}
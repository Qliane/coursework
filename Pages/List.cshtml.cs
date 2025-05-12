
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Coursework.Pages;

[Authorize]
public class ListModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;

    public ListModel(UserManager<ApplicationUser> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<ActionResult> OnGet(int ListID) // Автоматическая привязка
    {
        var user = await _userManager.GetUserAsync(User);
        ViewData["UserEmail"] = user.Email;
        var userId = user.Id;
        var lists = this._context.Lists.Where(p => p.Id == ListID && p.UserId == userId);
        if(lists.Count() == 0) return new RedirectResult("/404");

        var list = lists.First();
        this.ViewData["List"] = list;
        return new PageResult();
    }

}
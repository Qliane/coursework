using System.Threading.Tasks;
using Coursework.Areas.Identity.Pages.Account;
using Coursework.Pages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Coursework.Pages;
public class NotFoundModel(UserManager<ApplicationUser> userManager, AppDbContext context) : LoginModel(userManager, context)
{
    public async Task OnGet()
    {
        this.ViewData["UserEmail"] = await this.GetUserEmail();
    }
}
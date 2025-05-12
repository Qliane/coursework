using Coursework.Pages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Coursework.Pages;

public class LoginModel : PageModel{

    protected readonly UserManager<ApplicationUser> _userManager;
    protected readonly AppDbContext _context;
    public LoginModel(UserManager<ApplicationUser> userManager, AppDbContext context){
        _userManager = userManager;
        _context = context;
    }
    protected async Task<ApplicationUser?> GetUser(){
        return await _userManager.GetUserAsync(User);
    }

    protected async Task<string> GetUserEmail(){
        var user = await _userManager.GetUserAsync(User);
        if(user != null){
            return user.Email;
        }else{
            return "Гость";
        }
    }
}
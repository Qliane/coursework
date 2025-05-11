using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Coursework.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;

    public IndexModel(ILogger<IndexModel> logger, UserManager<ApplicationUser> userManager, AppDbContext context)
    {
        _logger = logger;
        _userManager = userManager;
        _context = context;
    }

    public async Task OnGet()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user != null)
        {
            if(_context.Lists.Count() <= 0){
                List l = new List
                {
                    NextItemOrder = 0,
                    UserId = user.Id,
                    Text = "Хотелочки",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Lists.Add(l);
                _context.SaveChanges();
                Console.WriteLine(user.Id);
            }

            Console.WriteLine(user.Id);
        }
        else
        {
            Console.WriteLine("User is null");
        }
    }
}

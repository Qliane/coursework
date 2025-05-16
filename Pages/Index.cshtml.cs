using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Coursework.Pages;

[Authorize] 
public class IndexModel : LoginModel
{
    private readonly ILogger<IndexModel> _logger;
    public IndexModel(ILogger<IndexModel> logger, UserManager<ApplicationUser> userManager, AppDbContext context) : base(userManager, context)
    {
        _logger = logger;
    }


    public static List<Item> GetUncompletedItems(List list){
        var items = list.Items;
        var outList = new List<Item>();
        foreach (var item in items){
            if(!item.IsCompleted){
                outList.Add(item);
            }
        }
        return outList;
    }

    public List<Item> GetCompletedItems(List list){
        var items = list.Items;
        var outList = new List<Item>();
        foreach (var item in items){
            if(item.IsCompleted){
                outList.Add(item);
            }
        }
        return outList;
    }


    [BindProperty]  // Для привязки данных из запроса
    public string Name { get; set; }
    public async Task<ActionResult> OnPostAddListAsync(){
        var user = await this.GetUser();
        if (string.IsNullOrEmpty(Name))
            return BadRequest("Name is required");
        if(user == null) return new NotFoundResult();
        var list = new List(){
            Text = Name,
            UserId = user.Id,
            User = user,
            CreatedAt = DateTime.UtcNow
        };

        _context.Lists.Add(list);
        _context.SaveChanges();
        
        return new JsonResult(new List{
            Id = list.Id,
            Text = list.Text,
            CreatedAt = list.CreatedAt,
            NextItemOrder = list.NextItemOrder
        });
    }

    public async Task<ActionResult> OnGetLogout()
    {
        await HttpContext.SignOutAsync();
        return new RedirectResult("/Identity/Account/Login");
    }
    public async Task<ActionResult> OnGetGenerateUserReport()
    {
        var rg = new ReportGenerator(this._context);
        var user = await this.GetUser();
        if (user == null) return new NotFoundResult();
        var report = rg.CreateReport(user);
        return Content(report, "text/html");
    }
    public async Task<ActionResult> OnGet()
    {

        ViewData["IsShowButton"] = "1";
        var user = await _userManager.GetUserAsync(User);
        this.ViewData["UserEmail"] = await GetUserEmail();
        if (user != null)
        {
            var lists = _context.Lists.Where(p => p.UserId == user.Id)
                .OrderByDescending(p => p.CreatedAt);  // Сначала новые (по убыванию даты);
            this.ViewData["IsEmpty"] = lists.Count() <= 0;
            foreach (var list in lists)
            {
                _context.Entry(list)
                    .Collection(l => l.Items)
                    .Load();

                list.Items.OrderBy(i => i.Order);
            }
            this.ViewData["Lists"] = lists.ToList();
            return new PageResult();
        }
        else
        {
            return Redirect("/Identity/Account/Login");
        }
    }
}

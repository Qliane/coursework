
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Coursework.Pages;

[Authorize]
public class ListModel : LoginModel
{

    public ListModel(UserManager<ApplicationUser> userManager, AppDbContext context) : base(userManager, context)
    {
    }

    private class ResponseItem
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Color { get; set; }
        public int Selected { get; set; }
        public string Text { get; set; }
    }

    private async Task<List?> GetList(int ListId)
    {
        var user = await this.GetUser();
        var lists = _context.Lists.Where(p => p.Id == ListId && p.UserId == user.Id);
        if (lists.Count() == 0) return null;
        return lists.First();
    }


    public async Task<ActionResult> OnGetDelete(int ListID)
    {
        var list = await this.GetList(ListID);
        if (list == null) new NotFoundResult();
        _context.Lists.Remove(list);
        _context.SaveChanges();
        return new RedirectResult("/Index");
    }

    public async Task<ActionResult> OnGetGetReport(int ListID)
    {
        var rg = new ReportGenerator(this._context);
        var user = await this.GetUser();
        if (user == null) return new NotFoundResult();
        var report = rg.CreateListReport(ListID, user);
        return Content(report, "text/html");
    }
    public async Task<ActionResult> OnGetAddTask(int ListID, string Text)
    {
        var list = await GetList(ListID);
        if (list == null) return new BadRequestResult();
        Item item = new Item()
        {
            ListId = list.Id,
            List = list,
            Text = Text,
            Order = list.NextItemOrder++,
            CreatedAt = DateTime.UtcNow
        };
        list.Items.Add(item);
        _context.SaveChanges();
        return new JsonResult(new
        {
            color = item.Color,
            id = item.Id,
            text = item.Text,
            order = item.Order,
            selected = item.IsCompleted,
        });
    }

    [BindProperty]
    public int Id { get; set; }
    [BindProperty]
    public int Order { get; set; }
    [BindProperty]
    public string Color { get; set; }
    [BindProperty]
    public int Selected { get; set; }
    [BindProperty]
    public string Text { get; set; }
    public async Task<ActionResult> OnPostRemove()
    {
        var items = _context.Items.Where(p=>p.Id == Id);
        var ok = false;
        if(items.Count() > 0){
            ok = true;
            var item = items.First();
            _context.Items.Remove(item);
            _context.SaveChanges();
        }
        return new JsonResult(new {
            ok = ok,
            this.Id
        });
    }

    
    public async Task<ActionResult> OnPostUpdate()
    {
        var items = _context.Items.Where(p=>p.Id == Id);
        if(items.Count() > 0){
            var item = items.First();
            if(item.Order != Order){
                var sameOrders = _context.Items.Where(p=>p.Order == Order);
                if (sameOrders.Count() > 0)
                {
                    var sameOrderItem = sameOrders.First();
                    sameOrderItem.Order = item.Order;
                    item.Order = this.Order;
                }
            }
            item.Color = Color;
            item.IsCompleted = Selected == 1;
            if(Selected == 1){
                item.CompletedAt = DateTime.UtcNow;
            }
            Console.WriteLine("SELECTED: ", Selected);
            item.Text = Text;
            await _context.SaveChangesAsync();
        }
        return new JsonResult(new {
            ok = 1
        });
    }

    public async Task<ActionResult> OnGetGetTasks(int ListID)
    {
        var list = await GetList(ListID);
        if (list == null) return new BadRequestResult();
        var responseItems = new List<ResponseItem>();

        foreach (var item in _context.Items.Where(p => p.ListId == list.Id))
        {
            responseItems.Add(new ResponseItem()
            {
                Id = item.Id,
                Order = item.Order,
                Color = item.Color,
                Selected = item.IsCompleted ? 1 : 0,
                Text = item.Text,
            });
        }

        return new JsonResult(responseItems);
    }
    public async Task<ActionResult> OnGet(int ListID) // РђРІС‚РѕРјР°С‚РёС‡РµСЃРєР°СЏ РїСЂРёРІСЏР·РєР°
    {
        var user = await _userManager.GetUserAsync(User);
        ViewData["UserEmail"] = user.Email;
        var userId = user.Id;
        var lists = this._context.Lists.Where(p => p.Id == ListID && p.UserId == userId);
        if (lists.Count() == 0) return new RedirectResult("/404");

        var list = lists.First();
        this.ViewData["List"] = list;
        return new PageResult();
    }

}
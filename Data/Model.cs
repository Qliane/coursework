using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Coursework.Pages;

public class ApplicationUser : IdentityUser
{
    // Коллекция списков пользователя
    public ICollection<List> Lists { get; set; } = new List<List>();

}

public class List
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }  // Внешний ключ для пользователя
    
    [Required]
    public ApplicationUser User { get; set; }  // Навигационное свойство к пользователю

    public int NextItemOrder { get; set; } = 1;  // Счетчик для порядка следующих элементов

    // Коллекция элементов списка
    public ICollection<Item> Items { get; set; } = new List<Item>();

    [Required]
    [StringLength(50)]
    public string Text { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
}

public class Item
{
    public int Id { get; set; }

    [Required]
    public int ListId { get; set; }  // Внешний ключ для списка

    [Required]
    public List List { get; set; }  // Навигационное свойство к списку

    [StringLength(7)]
    public string Color { get; set; } = "#000000";  // Значение по умолчанию - черный цвет

    [Required]
    [StringLength(50)]
    public string Text { get; set; }

    public bool IsCompleted { get; set; } = false;

    public int Order { get; set; }  // Поле для порядка в списке

    [Required]
    public DateTime CreatedAt { get; set; }


    [Required]
    public DateTime CompletedAt { get; set; }
}

public class Report
{
    public int Id { get; set; }
    public DateTime createdAt { get; set; }
    public string UserId { get; set; }
    public string Title { get; set; }
    public string HTML { get; set; }
    public int Type { get; set; }
}
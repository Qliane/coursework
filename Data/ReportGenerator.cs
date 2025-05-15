using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;

namespace Coursework.Pages
{
    public class ReportGenerator
    {
        private readonly AppDbContext _context;

        public ReportGenerator(AppDbContext context)
        {
            _context = context;
        }

        public string CreateListReport(int listId, ApplicationUser user)
        {

            var list = _context.Lists
                .Include(l => l.User)
                .Include(l => l.Items)
                .FirstOrDefault(l => l.Id == listId && l.User.Email == user.Email);

            if (list == null)
            {
                return "<html><body><h1>Список не найден</h1></body></html>";
            }

            var html = this.GenerateListReport(list);
            // добавить проверку на владение
            var report = new Report()
            {
                Title = "Отчёт о списке \"" + list.Text + "\"",
                createdAt = DateTime.UtcNow,
                Type = 1,
                UserId = user.Id,
                HTML = html
            };
            _context.Reports.Add(report);
            _context.SaveChanges();
            return html;
        }


        public string CreateReport(ApplicationUser user)
        {

            var html = this.GenerateReport(user);
            // добавить проверку на владение
            var report = new Report()
            {
                Title = "Отчёт о пользователе",
                createdAt = DateTime.UtcNow,
                Type = 0,
                UserId = user.Id,
                HTML = html
            };
            _context.Reports.Add(report);
            _context.SaveChanges();
            return html;
        }

        // Существующий метод для отчёта по пользователю
        private string GenerateReport(ApplicationUser user)
        {
            if (user == null)
            {
                return "<html><body><h1>Пользователь не найден</h1></body></html>";
            }

            _context.Entry(user)
                .Collection(u => u.Lists)
                .Load();

            foreach (var list in user.Lists)
            {
                _context.Entry(list)
                    .Collection(l => l.Items)
                    .Load();
            }

            var allItems = user.Lists.SelectMany(l => l.Items).ToList();
            var completedItems = allItems.Where(i => i.IsCompleted).ToList();

            int completedCount = completedItems.Count;
            int pendingCount = allItems.Count - completedCount;
            TimeSpan averageTime = completedCount > 0
                ? TimeSpan.FromTicks((long)completedItems.Average(i => (i.CompletedAt - i.CreatedAt).Ticks))
                : TimeSpan.Zero;

            var colorGroups = completedItems
                .GroupBy(i => i.Color?.ToLower() ?? "null")
                .OrderBy(g => g.Key)
                .ToList();

            string cssStyles = GetCssStyles();
            string cardsSection = GenerateCardsSection(completedCount, pendingCount);
            string tablesSection = GenerateTablesSection(colorGroups);

            string html = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Отчёт</title>
    <style>
        {cssStyles}
    </style>
</head>
<body>
    <main>
        <div id='content'>
            <div class='head'>
                <h1>Отчёт о пользователе</h1>
                <div class='text'>Подготовлен для {user.Email}</div>
                <div class='text'>Создан {DateTime.Now:dd.MM.yyyy HH:mm}</div>
            </div>
            <div class='body'>
                {cardsSection}
                {(completedCount > 0 ? $"<div class='averageTime'>Среднее время выполнения задачи: {FormatTimeSpan(averageTime)}</div>" : "")}
                {tablesSection}
            </div>
        </div>
    </main>
</body>
</html>";

            return html;
        }

        // Новый метод для отчёта по конкретному списку
        private string GenerateListReport(List list)
        {
            var completedItems = list.Items.Where(i => i.IsCompleted).ToList();
            int completedCount = completedItems.Count;
            int pendingCount = list.Items.Count - completedCount;

            TimeSpan averageTime = completedCount > 0
                ? TimeSpan.FromTicks((long)completedItems.Average(i => (i.CompletedAt - i.CreatedAt).Ticks))
                : TimeSpan.Zero;

            var colorGroups = completedItems
                .GroupBy(i => i.Color?.ToLower() ?? "null")
                .OrderBy(g => g.Key)
                .ToList();

            string cssStyles = GetCssStyles();
            string cardsSection = GenerateCardsSection(completedCount, pendingCount);
            string tablesSection = GenerateTablesSection(colorGroups);

            string html = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Отчёт по списку</title>
    <style>
        {cssStyles}
        .list-info {{ margin-bottom: 20px; }}
    </style>
</head>
<body>
    <main>
        <div id='content'>
            <div class='head'>
                <h1>Отчёт по списку</h1>
                <div class='text'>Подготовлен для {list.User.Email}</div>
                <div class='text'>Создан {DateTime.Now:dd.MM.yyyy HH:mm}</div>
            </div>
            <div class='body'>
                <div class='list-info'>
                    <div>Название списка: {list.Text}</div>
                    <div>Дата создания: {list.CreatedAt:dd.MM.yyyy HH:mm}</div>
                    <div>Всего задач: {list.Items.Count}</div>
                </div>
                {cardsSection}
                {(completedCount > 0 ? $"<div class='averageTime'>Среднее время выполнения: {FormatTimeSpan(averageTime)}</div>" : "")}
                {tablesSection}
            </div>
        </div>
    </main>
</body>
</html>";

            return html;
        }

        // Общие методы (без изменений)

        private string GetCssStyles()
        {
            return @"* {
                font-family: sans-serif;
                word-break: break-all;
            }

            body {
                margin: 0;
                background-color: #D4DCE3;
            }

            h1 {
                margin: 0;
            }

            #content {
                width: 840px;
                margin: 20px auto;
                padding: 45px;
                background-color: #DEE6E7;
            }

            .table {
                border: 1px solid #eee;
                table-layout: fixed;
                width: 100%;
                margin-bottom: 20px;
            }

            .table th {
                font-weight: bold;
                padding: 5px;
                background: #efefef;
                border: 1px solid #dddddd;
            }

            .table td {
                padding: 5px 10px;
                border: 1px solid #eee;
                text-align: left;
            }

            .table tbody tr:nth-child(odd) {
                background: #fff;
            }

            .table tbody tr:nth-child(even) {
                background: #F7F7F7;
            }

            .head .text {
                margin-top: 5px;
            }

            .cards {
                display: flex;
                gap: 20px;
                margin-top: 20px;
            }

            .card.green>.value {
                color: #00BD3A;
            }

            .card.blue>.value {
                color: #4276FF;
            }

            .card>.value {
                font-size: 20px;
                font-weight: 600;
            }

            .card {
                padding: 36px 40px;
                display: flex;
                flex-direction: column;
                align-items: center;
                gap: 6px;
                background-color: #D4DCE3;
            }

            .averageTime {
                margin-top: 22px;
            }

            .tables {
                margin-top: 25px;
                display: flex;
                flex-direction: column;
                gap: 15px;
            }
            .table{
                margin-top: 15px;
            }
            .color>.title{
                padding: 7px;
                display: inline;
            }
            .color.green>.title{
                background-color: #C6E191;
            }
            .color.red>.title{
                background-color: #E19191;
            }
            .color.orange>.title{
                background-color: #E1C191;
            }
            .color.blue>.title{
                background-color: #91A1E1;
            }
            .color.purple>.title{
                background-color: #B991E1;
            }
            .color.pink>.title{
                background-color: #E191D5;
            }
            .color.null>.title{
                border: 1px solid black;
            }";
        }

        private string GetColorClass(string colorHex)
        {
            return colorHex switch
            {
                "#c6e191" => "green",
                "#e19191" => "red",
                "#e1d091" => "orange",
                "#91d8e1" => "blue",
                "#9991e1" => "purple",
                "#e191bc" => "pink",
                _ => "null"
            };
        }

        private string FormatLocalDateTime(DateTime utcTime)
        {
            var localTime = utcTime.ToLocalTime(); // Конвертация UTC в локальное время сервера
            return localTime.ToString("g"); // Короткий формат даты+времени (например, "01.12.2025 14:30")
        }

        private string GetColorName(string colorHex)
        {
            return colorHex switch
            {
                "#c6e191" => "Зелёные задачи",
                "#e19191" => "Красные задачи",
                "#e1d091" => "Оранжевые задачи",
                "#91d8e1" => "Синие задачи",
                "#9991e1" => "Фиолетовые задачи",
                "#e191bc" => "Розовые задачи",
                _ => "Без цвета"
            };
        }

        private string GenerateCardsSection(int completedCount, int pendingCount) => $@"
            <div class='cards'>
                <div class='card green'>
                    <div class='value'>{completedCount}</div>
                    <div class='text'>Выполнено</div>
                </div>
                <div class='card blue'>
                    <div class='value'>{pendingCount}</div>
                    <div class='text'>Ожидают</div>
                </div>
            </div>";

        private string GenerateTablesSection(List<IGrouping<string, Item>> colorGroups)
        {
            if (colorGroups.Count == 0) return "";

            var tables = new StringBuilder();
            foreach (var group in colorGroups)
            {
                string colorClass = GetColorClass(group.Key);
                string colorName = GetColorName(group.Key);

                var rows = new StringBuilder();
                foreach (var item in group)
                {
                    TimeSpan duration = item.CompletedAt - item.CreatedAt;
                    rows.AppendLine($@"
                        <tr>
                            <td>{item.Text}</td>
                            <td>{FormatLocalDateTime(item.CreatedAt)}</td>
                            <td>{FormatLocalDateTime(item.CompletedAt)}</td>
                            <td>{FormatTimeSpan(item.CompletedAt - item.CreatedAt)}</td>
                        </tr>");
                }

                tables.Append($@"
                <div class='color {colorClass}'>
                    <div class='title'>{colorName}</div>
                    <table class='table'>
                        <thead>
                            <tr>
                                <th>Текст задачи</th>
                                <th>Создана</th>
                                <th>Завершена</th>
                                <th>Затрачено</th>
                            </tr>
                        </thead>
                        <tbody>
                            {rows}
                        </tbody>
                    </table>
                </div>");
            }

            return $@"<div class='tables'>{tables}</div>";
        }


        private string FormatTimeSpan(TimeSpan ts) =>
            $"{ts.Days} д. {ts.Hours} ч. {ts.Minutes} мин. {ts.Seconds} сек.";
    }
}
# Сборка
Выполните строку ниже, чтобы запустить приложение
```cmd
dotnet run
```

# Создание инсталятора
Опубликуем приложение и его зависимости в папку
```cmd
dotnet publish -c Release --self-contained -r win-x64 -o ./publish
```

Запустить сборку установщика при помощи Inno setup 6
```cmd
iscc Coursework.iss
```

# Доступные настройки
Для настройки конечных точек в файле kestrel.ini можно изменить URL для прослушивания:
```ini
[Kestrel]
Url={your ip}:{your port}
```
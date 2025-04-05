Если ты изменил API или любые контроллеры то действуешь по этому алгоритму: 

```shell

$ dotnet run --project flsurf 

# в другой консоли
$ cd Flsurf/Client
$ nswag run 
$ npm version patch

```

Потом делаешь коммит и пушишь измнения ВМЕСТЕ с измнениями в кодовой базе! 
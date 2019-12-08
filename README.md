# CVRPTW

**Capacitated Vehicle Routing Problem with Time Windows** — розв'язок проблеми оптимізації руху із врахуванням вмістимості транспортних засобів та часовими обмеженнями за допомогою OR-Tools

## Технології:

### Backend:
- [.NET Core](https://dotnet.microsoft.com/download)
- [SignalR](https://dotnet.microsoft.com/apps/aspnet/real-time)
- [RabbitMQ](https://www.rabbitmq.com/download.html)
- [OR-Tools](https://developers.google.com/optimization)

### Frontend:
- [Node.js](https://nodejs.org/en/)
- [Angular](https://angular.io)
- [Angular CLI](https://angular.io/cli)
- [Angular Material](https://material.angular.io)

## Запуск

### RabbitMQ

* відкрийте діалогове вікно виконання команд ``win + R`` 
* відкрийте термінал, введіть ``cmd`` 
* перейдіть в папку з RabbitMQ ``cd C:\Program Files\RabbitMQ Server\rabbitmq_server-***\sbin``
* запускаємо сервіс ``rabbitmq-service.bat start``
* при потребі переходимо на management сторінку http://localhost:15672, логін та пароль — **guest**

### OR-Tools

* відкрийте діалогове вікно виконання команд ``win + R`` 
* відкрийте термінал, введіть ``cmd`` 
* перейдіть в папку з OR-Tools ``cd **\CVRPTW\backend\OR-Tools``
* будуємо проект ``dotnet build``
* запускаємо сервіс ``dotnet run --project OR-Tools``

### API

* відкрийте діалогове вікно виконання команд ``win + R`` 
* відкрийте термінал, введіть ``cmd`` 
* перейдіть в папку з API ``cd **\CVRPTW\backend\API``
* будуємо проект ``dotnet build``
* запускаємо сервіс ``dotnet run --project API``
* документація — http://localhost:5000/swagger/index.html

### Angular

* відкрийте діалогове вікно виконання команд ``win + R`` 
* відкрийте термінал, введіть ``cmd`` 
* перейдіть в папку з клієнтом ``cd **\CVRPTW\frontend\Angular``
* встановлюємо пакети ``npm install``
* запускаємо сервіс ``ng serve``
* відкриваємо аплікацію — http://localhost:4200

## Запуск за допомогою Docker

* відкривайте Docker Terminal
* перейдіть в папку проекту ``cd **\CVRPTW``
* будуємо проект ``docker-compose build``
* запускаємо сервіси ``docker-compose up -d``

> **На Windows сервіси можуть бути доступні за іншою адресою. Щоб її дізнатись введіть ``docker-machine ip``**

## Архітектура сервісів

<p align="center">
  <img src="/docs/images/architecture.png">
</p>

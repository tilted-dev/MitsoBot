# MitsoBot - бот для поиска расписания студентов и преподавателей.

[VPS/VDS для развертывания (1 CPU, 512 RAM, 5 GB HDD)](https://firstbyte.ru/vps-vds/)
---------------
MITSO API
---------------
- Для доступа к API требуется ключ: `3A15-6C84-334C-5944` (он указан во всех проектах в appsettings.json)
- Эндпоинты, с которыми можно взаимодействовать можно посмотреть [на этой странице](https://student.mitso.by/api/)

Деплоймент
---------------
Чтобы запустить бота вконтакте необходимо выполнить следующую команду:
- `docker run -d -p 3000:80 -v /root/vk-db:/app/db -v /root/vk-logs:/app/Logs -i -t <название образа>`

Чтобы запустить бота телеграмм необходимо выполнить следующую команду:
- `docker run -d -p 3005:80 -v /root/tg-db:/app/db -v /root/tg-logs:/app/Logs -i -t <название образа>`

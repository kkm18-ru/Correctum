# Changelog

## [2.0.0](https://github.com/kkm18-ru/Correctum/compare/v1.4.1...v2.0.0) (2026-09-17)


### ⚠ BREAKING CHANGES

* Добавлены действия над чеками
* Переработано хранилище

### ✨ Нововведения

* **report:** Добавлен отчет "Наименования позиций" ([#47](https://github.com/kkm18-ru/Correctum/issues/47)) ([fb29832](https://github.com/kkm18-ru/Correctum/commit/fb29832b74798b470965acd9e71bdbd6f3cde8af))
* **ui:** Добавлена работа с диапазонами дат ([#52](https://github.com/kkm18-ru/Correctum/issues/52)) ([8ac7104](https://github.com/kkm18-ru/Correctum/commit/8ac7104566e730c705ea25d0a28d8600dadb3db6))
* **Действия:** Добавлено новое действие "Смена кассира" ([098001b](https://github.com/kkm18-ru/Correctum/commit/098001bed8f8d3e073d102057481619bc6a1e322))
* **Действия:** Добавлено новое действие "Смена метода оплаты" ([912f55b](https://github.com/kkm18-ru/Correctum/commit/912f55b71b3ec0567d1e21e02b4cea1fccd36a10))
* **Действия:** Добавлено новое действие "Смена способа расчета" ([d0d420f](https://github.com/kkm18-ru/Correctum/commit/d0d420f033a898f885aa7ee001b5f222c3e19d8e))
* Добавлен TLV-десериализатор из OFDru ([f9dc650](https://github.com/kkm18-ru/Correctum/commit/f9dc6504667e360e88c16d6f3afdcdf75fdd0983))
* Добавлена интеграция с "ККТ Онлайн" (!80) ([4930c33](https://github.com/kkm18-ru/Correctum/commit/4930c33c012ba059ffb2029f564b36a101f8ae0d))
* Добавлена кнопка обратной связи ([be0a1da](https://github.com/kkm18-ru/Correctum/commit/be0a1da8877d965185486e05bb3db926876f6a6e))
* Добавлена проверка подключения к ККТ ([db92730](https://github.com/kkm18-ru/Correctum/commit/db927301f880329907efabdd9bbb8d32acf0158c))
* Добавлено Drag&Drop для SQLite-импорта ([ac9ca88](https://github.com/kkm18-ru/Correctum/commit/ac9ca88b186e5e43e07530135d0dafd77f88be7c))
* Добавлено действие "Аннулировать" ([#138](https://github.com/kkm18-ru/Correctum/issues/138)) ([a3e16ce](https://github.com/kkm18-ru/Correctum/commit/a3e16ce565549077642869997fc1546bdedb3901))
* Добавлено действие "Удаление связи с оригинальным чеком" ([#137](https://github.com/kkm18-ru/Correctum/issues/137)) ([1c73f02](https://github.com/kkm18-ru/Correctum/commit/1c73f025a8c59948846c21fc1818ab1512608a2e))
* Добавлено логирование в Seq ([#110](https://github.com/kkm18-ru/Correctum/issues/110)) ([05cc17f](https://github.com/kkm18-ru/Correctum/commit/05cc17f8101ce09b1cd502a67afb268488469c53))
* Добавлено оповещение об авто-определении ККТ (!84) ([50cd016](https://github.com/kkm18-ru/Correctum/commit/50cd0160db8620becc5bb6c3f19c6057150eea47))
* Добавлено централизированное логирование ([#22](https://github.com/kkm18-ru/Correctum/issues/22)) ([611d1e6](https://github.com/kkm18-ru/Correctum/commit/611d1e678f3dcf9b563056079571755400684ccd))
* Добавлены действия над чеками ([094b5b4](https://github.com/kkm18-ru/Correctum/commit/094b5b43a5dd05119b70d656e3e44aebb3b1ecc6))
* Добавлены иконки для элементов меню ([ac1d0e4](https://github.com/kkm18-ru/Correctum/commit/ac1d0e478eb8b7b45bb830db2d02d1b04af83553))
* Добавлены реквизиты 1117, 1203, 1009, 1021 и 1187 к чеку ([5ea4520](https://github.com/kkm18-ru/Correctum/commit/5ea45208adb556703e87a10c04c129586772b2a6))
* Добавлены ставки НДС 18%, 18/118, 22%, 22/122 (!87) ([4ad54ae](https://github.com/kkm18-ru/Correctum/commit/4ad54ae221f3811b776ea9ef9dd97405e3b48abe))
* **Импорт:** Добавлен импорт из OFD.ru через ЛК ([#95](https://github.com/kkm18-ru/Correctum/issues/95)) ([3d38f98](https://github.com/kkm18-ru/Correctum/commit/3d38f98b2cd3834e2713961f4eec75efe8daa242))
* **Импорт:** Реализован импорт из Астрал ОФД ([#92](https://github.com/kkm18-ru/Correctum/issues/92)) ([d55f279](https://github.com/kkm18-ru/Correctum/commit/d55f279abc1189a88ebf7b3fa319d17be56e381c))
* **Импорт:** Упрощена авторизация для импорта из Астрал ОФД ([2201033](https://github.com/kkm18-ru/Correctum/commit/220103322cb14c27f2bf4e69f5adc43cd53420b3))
* **Импорт:** Упрощена авторизация для импорта из Платформа ОФД ([2c07d68](https://github.com/kkm18-ru/Correctum/commit/2c07d68cc5eaad60d23474dbb25add721cb80801))
* Обновлен формат заголовка группы позиций ([af2ab00](https://github.com/kkm18-ru/Correctum/commit/af2ab007346c38f73f860bcd1e624146aa739ffb)), closes [#69](https://github.com/kkm18-ru/Correctum/issues/69)
* **Отчеты:** Добавлен отчет "Количество чеков по часам" ([688e614](https://github.com/kkm18-ru/Correctum/commit/688e614614a2a0e9ee34266a0132990aec1dcd2f))
* **Отчеты:** Добавлен экспорт отчета в XLSX-таблицу ([#85](https://github.com/kkm18-ru/Correctum/issues/85)) ([62f8cf1](https://github.com/kkm18-ru/Correctum/commit/62f8cf1fc9dcbf4095f7c1302e1af905ba9ac782))
* **Отчеты:** Добавлено отображение графика в отчете ([#98](https://github.com/kkm18-ru/Correctum/issues/98)) ([860ab90](https://github.com/kkm18-ru/Correctum/commit/860ab9017df9fa7731d037b1ba94dc580918aef3))
* Переработано окно проведения ЧК (!86) ([0dae620](https://github.com/kkm18-ru/Correctum/commit/0dae62091c7bcec12e79e9175445c7363d276d64))
* Переработано хранилище ([2b0ed80](https://github.com/kkm18-ru/Correctum/commit/2b0ed80283a510365f5de876880b0ae3721b3102))
* Реализован MultiProgressBar ([#104](https://github.com/kkm18-ru/Correctum/issues/104)) ([a73599a](https://github.com/kkm18-ru/Correctum/commit/a73599a5fca904f7598d31d72357a63da01d4a73))
* Реализован импорт из "Первого ОФД" ([#16](https://github.com/kkm18-ru/Correctum/issues/16)) ([580b2ae](https://github.com/kkm18-ru/Correctum/commit/580b2ae8d7b9772a4cbe855f75eea2641d3f9d6e))
* Реализована валидация чеков и их позиций ([fe55c2b](https://github.com/kkm18-ru/Correctum/commit/fe55c2bde34ce31a2f9bd6ee139d15eb3e53763e))
* Реализовано меню для главного окна (!83) ([8f3dca4](https://github.com/kkm18-ru/Correctum/commit/8f3dca41b3433f9932f759bbf14bd81fed73d4ae))
* Реализовано отдельное окно для экспорта чеков (!82) ([9bf672f](https://github.com/kkm18-ru/Correctum/commit/9bf672f8ff7dc0d6e7eab4ecc08220818416a1ff))
* Реализовано создание, окрытие и сохранение хранилища ([2807bb5](https://github.com/kkm18-ru/Correctum/commit/2807bb5c3daae6a2ee13f072b0a4878d8e6ab2a8))
* Улучшена безопасность чувствительных данных ([#29](https://github.com/kkm18-ru/Correctum/issues/29)) ([e450a82](https://github.com/kkm18-ru/Correctum/commit/e450a821b6c9eff376a6a17f72df93bcb5c0bcff))
* **Хранилище:** Добавлено Drag&Drop-открытие хранилища ([878f1e6](https://github.com/kkm18-ru/Correctum/commit/878f1e6b5509a13d64c370abe24ad2a05acbc7b1))
* **Хранилище:** Добавлено резервное копирование хранилища ([#96](https://github.com/kkm18-ru/Correctum/issues/96)) ([c7a4a86](https://github.com/kkm18-ru/Correctum/commit/c7a4a86de979cea93f0f02badc20e663b3de003b))


### 🐞 Исправления

* **client-taxcom:** Исправлен доступ к константе SessionHeaderName ([fa8e0e1](https://github.com/kkm18-ru/Correctum/commit/fa8e0e141f660263a6744d08999fc34bf1d6f782))
* **client-taxcom:** Исправлен доступ к свойству SessionToken ([#48](https://github.com/kkm18-ru/Correctum/issues/48)) ([f772cd7](https://github.com/kkm18-ru/Correctum/commit/f772cd7833f3a34ddd0561b02900bd6d15565f1d))
* **lint:** S3881 ([b635c05](https://github.com/kkm18-ru/Correctum/commit/b635c057b62a31aac905a505c906d1e29243e18a))
* **OfdRuJsonDeserializer:** Добавлен парсинг свойств ([ea93d84](https://github.com/kkm18-ru/Correctum/commit/ea93d840c0c33a2031f06f6ec56581d2e474f637))
* **OfdRuTlvDeserializer:** Добавлен парсинг свойств ([128df14](https://github.com/kkm18-ru/Correctum/commit/128df1473075303cc0e3e30bfd77f5df1789ab0b))
* **sqlite:** Добавлено сохранение и загрузка СНО в/из SQLite ([#38](https://github.com/kkm18-ru/Correctum/issues/38)) ([da52dce](https://github.com/kkm18-ru/Correctum/commit/da52dce8235f10031ff8e0ae97fcbb3c58322c99))
* Восстановлен импорт из Такском-Касса ([#19](https://github.com/kkm18-ru/Correctum/issues/19)) ([b00dd19](https://github.com/kkm18-ru/Correctum/commit/b00dd19cf992668d09f68bf8709430abe78b03f5))
* Восстановлен показ прогресса импорта ([f63eea7](https://github.com/kkm18-ru/Correctum/commit/f63eea7ae22a586611e9bda93b4a0ac80d162d56))
* Добавлен Toml-конвертер LogLevel ([7ef0bda](https://github.com/kkm18-ru/Correctum/commit/7ef0bda728bd40a390e494c8202b3002cb1a4c74))
* Добавлен импорт ЧК из Платформа ОФД ([f7362c0](https://github.com/kkm18-ru/Correctum/commit/f7362c03e73c2c6ca17f1c5d71ec70284ae01443))
* Добавлен манифест Windows-приложения ([e7dc78d](https://github.com/kkm18-ru/Correctum/commit/e7dc78d07a34b57b7ab67e60741854047da9a7fd)), closes [#103](https://github.com/kkm18-ru/Correctum/issues/103)
* Добавлена обработка отсутствия значения в конвертере enum в локаль ([e873fb5](https://github.com/kkm18-ru/Correctum/commit/e873fb5be38585a92758060fd44398302b2f2f07)), closes [#150](https://github.com/kkm18-ru/Correctum/issues/150)
* Добавлена обработка отсутствия реквизитов в импортерах ([25c1ff7](https://github.com/kkm18-ru/Correctum/commit/25c1ff7c7cd32e8a6f0ffc93125c0ca9ac06c720))
* Добавлена очистка пула SQLite-подключений замене пути файла ([0c936df](https://github.com/kkm18-ru/Correctum/commit/0c936df58e368932951cf4a6f7a67d31e878dddf))
* Добавлена поддержка архитектуры x86 ([#42](https://github.com/kkm18-ru/Correctum/issues/42)) ([9e302a6](https://github.com/kkm18-ru/Correctum/commit/9e302a67e19216431ae765fc9c8276c320b3b1b5))
* Добавлена подсказка сочитания клавиш для вставки чека ([4afe55a](https://github.com/kkm18-ru/Correctum/commit/4afe55a8e0aeecf395e372e2c7a440a9abeebabf))
* Добавлена проверка значения перечислений ([d34441a](https://github.com/kkm18-ru/Correctum/commit/d34441a740903bb0fdb6060807b142928e62a5b0))
* Добавлено отображение ФН в импорте из SabyOFD ([6aa6bbf](https://github.com/kkm18-ru/Correctum/commit/6aa6bbf25b777ca219c51cde774f0c0e03793a29)), closes [#149](https://github.com/kkm18-ru/Correctum/issues/149)
* Добавлены исполнении ФН в конвертер ([e8ec72a](https://github.com/kkm18-ru/Correctum/commit/e8ec72af5f812ae4a0ba194f50a039d846fef9e6))
* Добавлены ограницчения размеров окна отчета ([18d0207](https://github.com/kkm18-ru/Correctum/commit/18d0207ed49fb3726f99dd8669b11d968f92b578))
* Загрузка таблиц позиций заменена на Include ([9d8d909](https://github.com/kkm18-ru/Correctum/commit/9d8d909f61cd3d225aafd752b4427913b3a84255))
* **Импорт:** Исправлена инициализация логирования импорта ([263f735](https://github.com/kkm18-ru/Correctum/commit/263f7354135fb136bf752da62ce5048148b01e35))
* **Импорт:** Чеки записываются в хранилище в отсортированном виде ([eadd7a1](https://github.com/kkm18-ru/Correctum/commit/eadd7a17d6f6e1a12f9425729e36411776dbd3d6))
* Исправлен Sqlite-экспорт с пустыми реквизитами ([e53ccad](https://github.com/kkm18-ru/Correctum/commit/e53ccadc267a2a9690578c14f43292349fb9bdc3))
* Исправлен парсинг количества в парсере PlatformOfd ([#40](https://github.com/kkm18-ru/Correctum/issues/40)) ([16cd4ad](https://github.com/kkm18-ru/Correctum/commit/16cd4adfaf3b99a5c0f13bb1fef3244adecd3ee8))
* Исправлен цикл импорта из ПлатформаОФД ([#41](https://github.com/kkm18-ru/Correctum/issues/41)) ([af6a505](https://github.com/kkm18-ru/Correctum/commit/af6a505a1e0a7e85b46d0a328c4a9a535bfe0f54))
* Исправлена авторизация в Платформа ОФД ([#131](https://github.com/kkm18-ru/Correctum/issues/131)) ([06d6bcc](https://github.com/kkm18-ru/Correctum/commit/06d6bcc98815cfc52bbf45613e22063af833ad14))
* Исправлена кнопка меню Файл&gt;Сохранить как... ([e391dde](https://github.com/kkm18-ru/Correctum/commit/e391dde6f64f8d3daee1940240057753d11513ed))
* Исправлена обрезка строк в парсере PlatformOfd ([#39](https://github.com/kkm18-ru/Correctum/issues/39)) ([14c62fd](https://github.com/kkm18-ru/Correctum/commit/14c62fd40c71315b5fc103d29649b3ef5073a2b4))
* Исправлена ошибка привязки MenuItem верт./гориз. выравнивания контента ([63e5b09](https://github.com/kkm18-ru/Correctum/commit/63e5b09aab11313fd9181c84308437849b2e008a))
* Исправлена ошибка с null в Fptr120ExportViewModel.PaymentAsync ([af89c02](https://github.com/kkm18-ru/Correctum/commit/af89c0271a36550234f238f8070b66a3aefe8563))
* Исправлена работа DateRange ([a1c4572](https://github.com/kkm18-ru/Correctum/commit/a1c4572af82b7f062d096c8997a24626b1e6abfb))
* Исправление варнингов Correctum.Importers.Acquirer ([16ece5c](https://github.com/kkm18-ru/Correctum/commit/16ece5c3c59260463dde79f5a95bc4ba76c65dc6))
* Исправлено CS8604 в отчете "Наименования позиций" ([ad6c18a](https://github.com/kkm18-ru/Correctum/commit/ad6c18af6db968d260ea4fe4c545bdc4ba3d7d0b))
* Исправлено действие при неудачной проверке пробития эл. чеков ([2000782](https://github.com/kkm18-ru/Correctum/commit/20007823eaefd2f1d4994ac6cb54c331401b5ab7))
* Исправлено отображение таймаута API-клиента ([5d7ab00](https://github.com/kkm18-ru/Correctum/commit/5d7ab00b6f8b58376e46b961d0a7f0eab51f0447))
* Исправлено получение ЕИ в клиенте Астрал ОФД ([d12fa86](https://github.com/kkm18-ru/Correctum/commit/d12fa863e89b99c8987729874454bf0a84328f10))
* Исправлено получение ставки НДС в клиенте Астрал ОФД ([27c6778](https://github.com/kkm18-ru/Correctum/commit/27c677802e4c1f628278f53651a98df96ffdf5fa))
* Исправлено получение ставки НДС в клиенте Первый ОФД ([d94ca07](https://github.com/kkm18-ru/Correctum/commit/d94ca072d4403fa33fe67aabb6d7bdc60fad0b91))
* Исправлено получение ставки НДС в клиенте Такском-Касса ([df7a109](https://github.com/kkm18-ru/Correctum/commit/df7a109baefcfeb16f73a7209b27ea1d08988ac0))
* Исправлено проведение электронных чеков при версии конфигурации 5.17.0 и выше (!85) ([e731155](https://github.com/kkm18-ru/Correctum/commit/e7311557d2dde1ddc7079f6abbd78b71c6a2a8ea))
* Исправлены варнинги S3459 и S1144 в импортере Sqlite ([8b5519d](https://github.com/kkm18-ru/Correctum/commit/8b5519d39dd428ed7777574a99593da3ed6906d0))
* Исправлены жизненные циклы API-клиентов ([#25](https://github.com/kkm18-ru/Correctum/issues/25)) ([08f1005](https://github.com/kkm18-ru/Correctum/commit/08f10056cbb8d244a24511978a90903f3266a536))
* Исправлены конструкторы DisplayReceipt и DisplayPosition ([0fb2a7c](https://github.com/kkm18-ru/Correctum/commit/0fb2a7c96d6d73438b172f6f3c6e657587abe5d1))
* **Логирование:** Добавлена лог-запись при необработанном исключении ([4fd1955](https://github.com/kkm18-ru/Correctum/commit/4fd1955ecc631e21c7fd939d59b94c6167d30732))
* **Логирование:** Подкорректирован шаблон лог-записи в файл ([d5d5705](https://github.com/kkm18-ru/Correctum/commit/d5d570592cc7cc4f8d19c5af826d938676c503bf))
* **Логирование:** Удалено отображения номера сборки приложения ([c08427f](https://github.com/kkm18-ru/Correctum/commit/c08427f27ce5df63af7fc4794562dde45387f38f))
* **Модели данных:** Удалены лишние nullable ([420a88b](https://github.com/kkm18-ru/Correctum/commit/420a88bc60fc69c18c4a66ed75770f4e53fd331f))
* **Отчеты:** Удалена излишняя кнопка обновления отчета ([858bee4](https://github.com/kkm18-ru/Correctum/commit/858bee4ce3e767d5d8482d34836e8567e967f69a))
* Параметр ИНН переведен в параметр для авторизации ([ef274e4](https://github.com/kkm18-ru/Correctum/commit/ef274e40ce9fff9f8a82634bc852616cd61429bc))
* Перевод главного меню в MVVM ([4136b52](https://github.com/kkm18-ru/Correctum/commit/4136b525c764128198ec337c9f4c92249f74a915))
* Перевод окна ExportWindow в XAML ([748518a](https://github.com/kkm18-ru/Correctum/commit/748518af58b65e808d00c98dab4f2848a7279c06))
* Подкорректирована работа EnumLocales.GetLocale ([8ae3dff](https://github.com/kkm18-ru/Correctum/commit/8ae3dffc421ee5205bff91e351c23a33ea5c6b80))
* Понижен уровень доступа к приложению ([b147326](https://github.com/kkm18-ru/Correctum/commit/b147326b24184b9ddb9d54acc0954a1d7011f45c)), closes [#151](https://github.com/kkm18-ru/Correctum/issues/151)
* **Ресурсы:** Шрифт перенесен в ресурсы ([73b3ed2](https://github.com/kkm18-ru/Correctum/commit/73b3ed2de67555f7da193dec34fce7f5d78a3e90))
* Сеттеры построителей принимают null ([093ce3e](https://github.com/kkm18-ru/Correctum/commit/093ce3e1f54a3b967a1f0adf9c2c2bdc8a161a13))
* Стабилизирована работа проведения чеков ([514afcb](https://github.com/kkm18-ru/Correctum/commit/514afcb60a42c89c3796e7b2c00a2a395259e5b8))
* Тип  данных количество предмета расчета заменен на decimal ([922ce85](https://github.com/kkm18-ru/Correctum/commit/922ce85f917721b36381a324cfc3a78cb835ac9c))
* Удалена дублирование чеков в импорте Платформа ОФД ([b5347ab](https://github.com/kkm18-ru/Correctum/commit/b5347abfd2c649aebf89e5d76ab9ab93c50f4bae))
* Удалена излишняя структура оплаты ([20a7aa9](https://github.com/kkm18-ru/Correctum/commit/20a7aa984318dfb6e93a698cebb333d4e4bebd30))
* Удалено излишнее условие десериализации чека из реквизитов Платформа ОФД ([7fe9381](https://github.com/kkm18-ru/Correctum/commit/7fe9381bb0cc4af148f36097c342236b34c4b65b))
* Удалены лишние пустые символы в исполнении ФН ([b586bb2](https://github.com/kkm18-ru/Correctum/commit/b586bb217162a09bda2282e570126ae7e3645f4c))
* Упрощена работа с сочетанием клавиш в базовом классе диалоговых окон ([70c7719](https://github.com/kkm18-ru/Correctum/commit/70c7719bc1baf1f9231062e73b38e5ed85a5a596))
* Установлен абсолютный путь к explorer.exe ([#55](https://github.com/kkm18-ru/Correctum/issues/55)) ([555b6e7](https://github.com/kkm18-ru/Correctum/commit/555b6e7aea8de0091a7a0adfd607c11215636b56))
* Установлена целевая платформа x86 ([9c1da0a](https://github.com/kkm18-ru/Correctum/commit/9c1da0ac176e6b0928daaad0c15b87d889251c4a))
* Установлено максимальное время работы регулярных выражений ([#54](https://github.com/kkm18-ru/Correctum/issues/54)) ([e73397d](https://github.com/kkm18-ru/Correctum/commit/e73397dfc5f992c691a545aec8ac649bf3e12e99))
* Ширина столбцов таблиц изменяемая ([a8ec190](https://github.com/kkm18-ru/Correctum/commit/a8ec190601fb8ed8914b6bc12748ebe7a8e3717f)), closes [#70](https://github.com/kkm18-ru/Correctum/issues/70)
* Шифрование секретов в конфигурации перенесена в Toml-конвертер ([4f7b84c](https://github.com/kkm18-ru/Correctum/commit/4f7b84c59aacabfe153ddb642b0013b306971007))


### ⚡ Улучшение производительности

* Добавлены значительные объекты ([#60](https://github.com/kkm18-ru/Correctum/issues/60)) ([b7c654f](https://github.com/kkm18-ru/Correctum/commit/b7c654f8920b60f2ee5885d68dd14d77e92fc964))
* Оптимизирован парсер Платформа ОФД (!88) ([69b27ec](https://github.com/kkm18-ru/Correctum/commit/69b27ecaff5caa5a2e1ec28c9de329375135e36b))


### ⏪ Отмена изменений

* Вырезана система действий над чеками ([#61](https://github.com/kkm18-ru/Correctum/issues/61)) ([08b6f20](https://github.com/kkm18-ru/Correctum/commit/08b6f20d5008a61d8decbb20ca960338a4ae5ac8))
* Удален Drop&Drag-импорт SQLite ([c4c6c31](https://github.com/kkm18-ru/Correctum/commit/c4c6c314afb0a10e43070fbec334f7c4bb04459f))
* Удален mock-импорт ([90de07d](https://github.com/kkm18-ru/Correctum/commit/90de07de7083e204c10ec901885f77ffbf04f12d))
* Удален импорт из Sqlite ([665840e](https://github.com/kkm18-ru/Correctum/commit/665840e668e20fc1eb75d726c403d84b2fada733))
* Удален импорт из эквайринга ([9dfcc0c](https://github.com/kkm18-ru/Correctum/commit/9dfcc0c8997acb6daaf04f39d6847ecb6b02de8e))
* Удалён импортер FxPOS ([736e48e](https://github.com/kkm18-ru/Correctum/commit/736e48e108c92b08123e8b7e862baaf05f3d5146))
* Удален экспорт в Sqlite ([a467aa6](https://github.com/kkm18-ru/Correctum/commit/a467aa6c57999e81be1d4c2466a0a8c554f03322))
* Удалена излишнее шифрование ключа Seq ([aac2f25](https://github.com/kkm18-ru/Correctum/commit/aac2f25b1527d7152d7a2c2a72931112127a88ff))
* Удалена настройка EoU ([5c9cda5](https://github.com/kkm18-ru/Correctum/commit/5c9cda5751c69bdf6b031bf8487ce3bcfe6c7b8c))
* Удалено логирование в Grafana Loki ([9cc4fc6](https://github.com/kkm18-ru/Correctum/commit/9cc4fc66a4b7ce070778ef522b66485232218f7f))
* Удалено окно обратной связи ([f61ba8a](https://github.com/kkm18-ru/Correctum/commit/f61ba8a7181a35aff879dce9a76d54ac89b5b0ce))
* Удалено перемешение по индексу ([b079c66](https://github.com/kkm18-ru/Correctum/commit/b079c66fd8ab01f9021e76aa73883f87f6bcfdb8))


### ⬆️ Обновления зависимостей

* **coverlet.collector:** 10.0.1 ([6d3a120](https://github.com/kkm18-ru/Correctum/commit/6d3a120919dbbd1af0900c235c67d85b4f40a0c2))
* **coverlet.collector:** 10.0.1 ([2b4f2a8](https://github.com/kkm18-ru/Correctum/commit/2b4f2a86614069d616b68c2d36e09c2f719493fc))
* **Dapper:** 2.1.79 ([bfe75e4](https://github.com/kkm18-ru/Correctum/commit/bfe75e434d4115a8256f61e6de9f7972cc8de2a0))
* **Extended.Wpf.Toolkit:** 5.1.2 ([ca569a2](https://github.com/kkm18-ru/Correctum/commit/ca569a2376206ee5b5afbdb20eb799179119859b))
* **Microsoft.CodeAnalysis.*:** 5.3.0 ([2324172](https://github.com/kkm18-ru/Correctum/commit/2324172023953b8b7fc6fa8226bb48ebe4137ada))
* **Microsoft.CodeAnalysis.*:** 5.9.0 ([a791b1b](https://github.com/kkm18-ru/Correctum/commit/a791b1ba4b83530a5215f9804c84693eb775f555))
* **Microsoft.CodeAnalysis.CSharp.SourceGenerators.Testing.XUnit:** 1.1.2 ([9a95958](https://github.com/kkm18-ru/Correctum/commit/9a959582ed77e4c7a6800fc06ae562009f4d5851))
* **Microsoft.Data.Sqlite.Core:** 10.0.9 ([4c17382](https://github.com/kkm18-ru/Correctum/commit/4c17382958db666838821ec686909f5564250bdd))
* **Microsoft.EntityFrameworkCore.Sqlite:** 10.0.12 ([8c8ad09](https://github.com/kkm18-ru/Correctum/commit/8c8ad093f40b9943209326bc6c346156f4e365b4))
* **Microsoft.EntityFrameworkCore.Sqlite:** 10.0.9 ([3c7db38](https://github.com/kkm18-ru/Correctum/commit/3c7db3887abd63a33b4205455b525f49ccbe09ae))
* **Microsoft.Extensions.Http.Resilience:** 10.10.0 ([6c3164a](https://github.com/kkm18-ru/Correctum/commit/6c3164aa012882c315f2fecb9d5b78e1e631bf0d))
* **Microsoft.Extensions.Http.Resilience:** 10.7.0 ([d8c324c](https://github.com/kkm18-ru/Correctum/commit/d8c324c1c23cb37ed15d7e9d5395010dca325d24))
* **Microsoft.NET.Test.Sdk:** 18.10.0 ([709d429](https://github.com/kkm18-ru/Correctum/commit/709d429138c53965e4fd37291450ca794110792f))
* **Microsoft.NET.Test.Sdk:** 18.5.1 ([3b0cf8b](https://github.com/kkm18-ru/Correctum/commit/3b0cf8be0f6558ef22ae424fce6f1d82532963c1))
* **Microsoft.NET.Test.Sdk:** 18.7.0 ([f432eb2](https://github.com/kkm18-ru/Correctum/commit/f432eb244a1f56639ac2c90eb65f8841724bd4bf))
* **Microsoft.Xaml.Behaviors.Wpf:** 1.1.158 ([e15d751](https://github.com/kkm18-ru/Correctum/commit/e15d751683c0d8d57e0847b50ad593fe465aaa24))
* **NanoXLSX.Writer:** 3.2.1 ([d366d64](https://github.com/kkm18-ru/Correctum/commit/d366d64def02bc5e5028c815a285e849e943b267))
* **NLog:** 6.1.3 ([cd971e6](https://github.com/kkm18-ru/Correctum/commit/cd971e60c2943c438ae84de7162664ea8a99e7a7))
* **NLog:** 6.2.0 ([e55f739](https://github.com/kkm18-ru/Correctum/commit/e55f7394075fd7bdb03e077a1618d9eec1c44b05))
* **NUnit.Analyzers:** 4.14.0 ([72eabf8](https://github.com/kkm18-ru/Correctum/commit/72eabf81e07fb5ff7bf24fa46ebbbc32db0ef0dd))
* **NUnit.Analyzers:** 4.15.0 ([d685163](https://github.com/kkm18-ru/Correctum/commit/d685163b1177e45336d42b4a0f7afc9b13381ee3))
* **NUnit3TestAdapter:** 6.3.0 ([d8d35b4](https://github.com/kkm18-ru/Correctum/commit/d8d35b410a9e0684578e75339d1c630a450cef69))
* **NUnit:** 4.6.1 ([ef63445](https://github.com/kkm18-ru/Correctum/commit/ef634453d66a08fe88ab809abb3aecbac93c881c))
* **NUnit:** 4.6.1 ([b3edacc](https://github.com/kkm18-ru/Correctum/commit/b3edaccabf0ff3a91c18b1cf15b050e293084544))
* **Polly:** 8.7.0 ([2dd8409](https://github.com/kkm18-ru/Correctum/commit/2dd8409ddaf8b93d0dd08bdbdae870ddcedca1f5))
* **PuppeteerSharp:** 25.10.0 ([860003b](https://github.com/kkm18-ru/Correctum/commit/860003bc9a9bbf6a8e67464694dfdbc780d0d845))
* **PuppeteerSharp:** 25.2.1 ([47384ae](https://github.com/kkm18-ru/Correctum/commit/47384ae72174469719725054a772d8679a88f7a3))
* **RetailCorrector.Tests:** WireMock.Net.Minimal 1.25.0 ==&gt; 2.3.0 ([e579240](https://github.com/kkm18-ru/Correctum/commit/e579240a5174e063caaca33ff766e0acff8c9de2))
* **RetailCorrector:** Bogus 35.6.4 ==&gt; 35.6.5 ([4636beb](https://github.com/kkm18-ru/Correctum/commit/4636beb5aa15b5b5b9367b05413ba03786337c3b))
* **RetailCorrector:** Dapper 2.1.66 ==&gt; 2.1.72 ([9f9edf8](https://github.com/kkm18-ru/Correctum/commit/9f9edf892b34534dfe393fed02f4353380638211))
* **RetailCorrector:** Microsoft.Data.Sqlite.Core 9.0.9 ==&gt; 10.0.7 ([563f726](https://github.com/kkm18-ru/Correctum/commit/563f726944011a7417f5dc743510b8a0607a4b35))
* **RetailCorrector:** Microsoft.Extensions.Http.Resilience 10.0.0 ==&gt; 10.5.0 ([b42f124](https://github.com/kkm18-ru/Correctum/commit/b42f12417820188ef380b5f976d32a8ef63d4365))
* **RetailCorrector:** NLog 6.1.0 ==&gt; 6.1.2 ([e87eaea](https://github.com/kkm18-ru/Correctum/commit/e87eaea2bb7e2c8393ee76f40419df114d428de3))
* **RetailCorrector:** NLog.Targets.Loki 2.4.0 ==&gt; 2.5.0 ([e2cbb8a](https://github.com/kkm18-ru/Correctum/commit/e2cbb8a145ceb992ae5e16633b0624800b0a6489))
* **RetailCorrector:** Polly 8.6.5 ==&gt; 8.6.6 ([d7c1a8d](https://github.com/kkm18-ru/Correctum/commit/d7c1a8d065f7c2c0b9101ea893d59d14e0ddd717))
* **RetailCorrector:** System.ServiceProcess.ServiceController 9.0.10 ==&gt; 10.0.7 ([1c1bfe6](https://github.com/kkm18-ru/Correctum/commit/1c1bfe66660d3ba8a99764699829538ff38954f9))
* **RetailCorrector:** Tomlyn 0.19.0 ==&gt; 2.3.1 ([0be5c45](https://github.com/kkm18-ru/Correctum/commit/0be5c45943eaeb267e806357f7de3158e95aa9e8))
* **System.ServiceProcess.ServiceController:** 10.0.12 ([cea8245](https://github.com/kkm18-ru/Correctum/commit/cea82456ad2850e3743f66715d0b17d41bcb6865))
* **System.ServiceProcess.ServiceController:** 10.0.9 ([e0e7235](https://github.com/kkm18-ru/Correctum/commit/e0e723503157674e5d8ff6a9608a9b03aab859a9))
* **tests:** coverlet.collector 6.0.4 ==&gt; 10.0.0 ([232fda0](https://github.com/kkm18-ru/Correctum/commit/232fda0f0ba2647a289623a4b9edb9be245351da))
* **tests:** Microsoft.NET.Test.Sdk ==&gt; 18.5.0 ([0cabf9b](https://github.com/kkm18-ru/Correctum/commit/0cabf9bf9499d6e59d06e686fc7346309f7034bd))
* **tests:** NUnit ==&gt; 4.5.1 ([18832cb](https://github.com/kkm18-ru/Correctum/commit/18832cb4ff4c71a527b283aa49dd8054a560a357))
* **tests:** NUnit.Analyzers ==&gt; 4.12.0 ([c3bc9c9](https://github.com/kkm18-ru/Correctum/commit/c3bc9c9de48c1edd41e6112d7e8713d2ab68b98b))
* **tests:** NUnit3TestAdapter ==&gt; 6.2.0 ([86377e8](https://github.com/kkm18-ru/Correctum/commit/86377e8a6884045eedb3bc0251eb9fda2ca747eb))
* **Tomlyn:** 2.10.1 ([c60c2bc](https://github.com/kkm18-ru/Correctum/commit/c60c2bcdb0e0fbf12b938519583aca5a083ca328))
* **Tomlyn:** 2.3.2 ([1087252](https://github.com/kkm18-ru/Correctum/commit/1087252e5ea82b3ce99a7a1a0675975ff726b0c3))
* **Tomlyn:** 2.9.0 ([30eb821](https://github.com/kkm18-ru/Correctum/commit/30eb8210da545c5e0d314b0120daa469eba3d4e5))
* **WireMock.Net.Minimal:** 2.11.0 ([8a93c53](https://github.com/kkm18-ru/Correctum/commit/8a93c53a954cb21ff74a95368c319cc837708212))
* **WireMock.Net.Minimal:** 2.4.0 ([a274474](https://github.com/kkm18-ru/Correctum/commit/a274474a05638e6475db0e85467cd2ebba52742c))
* **xunit.runner.visualstudio:** 3.1.5 ([6588cdf](https://github.com/kkm18-ru/Correctum/commit/6588cdf7e6216dbbc054ea100a8db880f2e36db5))
* **xunit:** 2.9.3 ([6a75efa](https://github.com/kkm18-ru/Correctum/commit/6a75efa9baefa394a538bf13c7bb30aae5ea823f))

## [1.4.1](https://github.com/kkm18-ru/Correctum/compare/v1.4.0...v1.4.1) (2026-09-15)


### 🐞 Исправления

* Переработана структура Inn ([#143](https://github.com/kkm18-ru/Correctum/issues/143)) ([0ca32b6](https://github.com/kkm18-ru/Correctum/commit/0ca32b67b8071703eec357cd593bffc9aca2e6aa))


### ⏪ Отмена изменений

* Отказ от "чистой архитектуры" ([#141](https://github.com/kkm18-ru/Correctum/issues/141)) ([c0616ef](https://github.com/kkm18-ru/Correctum/commit/c0616efa60f9ae2fb8d8e26aab613bada004daf0))
* Удален импорт из Sqlite ([8eb6c7f](https://github.com/kkm18-ru/Correctum/commit/8eb6c7fdf1e0efb902212bf4e864e42e45e54aa7))
* Удален экспорт в Sqlite ([45a96a8](https://github.com/kkm18-ru/Correctum/commit/45a96a8e6f53e421e13917f59f38628d76e6d636))


### ⬆️ Обновления зависимостей

* **coverlet.collector:** 10.0.1 ([43f7888](https://github.com/kkm18-ru/Correctum/commit/43f7888e7db650f5ea9db29a8a8c0c1b36048937))
* **Microsoft.CodeAnalysis.*:** 5.9.0 ([106f886](https://github.com/kkm18-ru/Correctum/commit/106f886b00f4f370f67663b9feb27b6ab7580007))
* **Microsoft.EntityFrameworkCore.Sqlite:** 10.0.12 ([42e2e7f](https://github.com/kkm18-ru/Correctum/commit/42e2e7f22cb7edf2c956ff2a222477a63a6e543c))
* **Microsoft.Extensions.Http.Resilience:** 10.10.0 ([86cdfed](https://github.com/kkm18-ru/Correctum/commit/86cdfedd8337aa51f2bb4a096d4836b42524a621))
* **Microsoft.NET.Test.Sdk:** 18.10.0 ([57c4e1a](https://github.com/kkm18-ru/Correctum/commit/57c4e1aca738ded626514ad7a36ef27b50928942))
* **Microsoft.Xaml.Behaviors.Wpf:** 1.1.158 ([72bdbaf](https://github.com/kkm18-ru/Correctum/commit/72bdbaffc056c9c96af13307ebe2fb883a6b8302))
* **NanoXLSX.Writer:** 3.2.1 ([03d7761](https://github.com/kkm18-ru/Correctum/commit/03d776185909509b75c52efad36b83c767bca828))
* **NLog:** 6.2.0 ([59547f2](https://github.com/kkm18-ru/Correctum/commit/59547f26937a863eab59c9b9325b8e09ba4e8437))
* **NUnit.Analyzers:** 4.15.0 ([696e92f](https://github.com/kkm18-ru/Correctum/commit/696e92fe1add554ef92fe23fb65142f8b62aa11a))
* **NUnit3TestAdapter:** 6.3.0 ([9ced1c2](https://github.com/kkm18-ru/Correctum/commit/9ced1c28940704f592b89e85b555c9c3e6670b5c))
* **NUnit:** 4.6.1 ([a868c8b](https://github.com/kkm18-ru/Correctum/commit/a868c8b62673e4833ee832df5fe29d73ff03db8a))
* **PuppeteerSharp:** 25.10.0 ([07e4511](https://github.com/kkm18-ru/Correctum/commit/07e4511ea2da07e91c961fef0f584ce4b760e924))
* **System.ServiceProcess.ServiceController:** 10.0.12 ([064ad1b](https://github.com/kkm18-ru/Correctum/commit/064ad1b2bd8e3d7bfb677d123b327d2273984a27))
* **Tomlyn:** 2.10.1 ([4c218f0](https://github.com/kkm18-ru/Correctum/commit/4c218f0825a9d6263d384795dbf1237c0769590e))

## [1.4.0](https://github.com/kkm18-ru/Correctum/compare/v1.3.0...v1.4.0) (2026-07-21)


### ✨ Нововведения

* Добавлено действие "Аннулировать" ([#138](https://github.com/kkm18-ru/Correctum/issues/138)) ([e0556af](https://github.com/kkm18-ru/Correctum/commit/e0556aff188211d8221704c6fbda5bd4ad24a369))
* Добавлено действие "Удаление связи с оригинальным чеком" ([#137](https://github.com/kkm18-ru/Correctum/issues/137)) ([a9e6b63](https://github.com/kkm18-ru/Correctum/commit/a9e6b63969f4f2ec222a1080eacc74dbaa2f92d2))

## [1.3.0](https://github.com/kkm18-ru/Correctum/compare/v1.2.0...v1.3.0) (2026-07-13)


### ✨ Нововведения

* **Действия:** Реализовано аннулирование по таблице ([#132](https://github.com/kkm18-ru/Correctum/issues/132)) ([ea69fd3](https://github.com/kkm18-ru/Correctum/commit/ea69fd36d7a58b6cb09d0720fb0ed80e36b5229c))


### 🐞 Исправления

* Исправлена авторизация в Платформа ОФД ([#131](https://github.com/kkm18-ru/Correctum/issues/131)) ([3bf2182](https://github.com/kkm18-ru/Correctum/commit/3bf2182dfa046a98071c30ed7090d069675915b6))


### ⬆️ Обновления зависимостей

* **coverlet.collector:** 10.0.1 ([380cc45](https://github.com/kkm18-ru/Correctum/commit/380cc45c91d4a0ee24fe044a22b122b923c8212e))
* **Dapper:** 2.1.79 ([32926c5](https://github.com/kkm18-ru/Correctum/commit/32926c564598cd9c0e9dc9d9b04fd376fc850a59))
* **Extended.Wpf.Toolkit:** 5.1.2 ([a8a6550](https://github.com/kkm18-ru/Correctum/commit/a8a655048b55cbc2da54ab559758bf07a7ea6f2d))
* **Microsoft.CodeAnalysis.*:** 5.3.0 ([15a9d20](https://github.com/kkm18-ru/Correctum/commit/15a9d20d10343b5f1afbde71dad28bdf280728e7))
* **Microsoft.CodeAnalysis.CSharp.SourceGenerators.Testing.XUnit:** 1.1.2 ([9608f9c](https://github.com/kkm18-ru/Correctum/commit/9608f9ca7f8a92d94f97cb56f1b04d5152f9b15e))
* **Microsoft.Data.Sqlite.Core:** 10.0.9 ([426817f](https://github.com/kkm18-ru/Correctum/commit/426817fb265eddd7a94bec15fb9a984b367d15de))
* **Microsoft.EntityFrameworkCore.Sqlite:** 10.0.9 ([a2ffe40](https://github.com/kkm18-ru/Correctum/commit/a2ffe40c8c0c32ba4787c6ef1d1414f812c05be7))
* **Microsoft.Extensions.Http.Resilience:** 10.7.0 ([e37a246](https://github.com/kkm18-ru/Correctum/commit/e37a2469f1877a18e6aee8d62bf4d92a09c86c7f))
* **Microsoft.NET.Test.Sdk:** 18.7.0 ([5e9ece7](https://github.com/kkm18-ru/Correctum/commit/5e9ece7dad11f963e74f77fcde0e548f1467fae7))
* **NLog:** 6.1.3 ([148dc85](https://github.com/kkm18-ru/Correctum/commit/148dc853a7eb19d9711e5552ed3329f0218855cb))
* **NUnit.Analyzers:** 4.14.0 ([de50233](https://github.com/kkm18-ru/Correctum/commit/de5023399b4e075594e0d4fa5e8cb3a400d59449))
* **NUnit:** 4.6.1 ([8470536](https://github.com/kkm18-ru/Correctum/commit/8470536ff7c8041c63c2ec0ba764849d80c42e70))
* **Polly:** 8.7.0 ([2334efd](https://github.com/kkm18-ru/Correctum/commit/2334efd6d944b4f52bf6e1c290008d8967b51d85))
* **PuppeteerSharp:** 25.2.1 ([449b667](https://github.com/kkm18-ru/Correctum/commit/449b667c8394865d6013bf1c448e6ab9a2c888b5))
* **System.ServiceProcess.ServiceController:** 10.0.9 ([0e47f96](https://github.com/kkm18-ru/Correctum/commit/0e47f96cd8d5738edb4dbf3fd0f1f865c46bb29b))
* **Tomlyn:** 2.9.0 ([62668a7](https://github.com/kkm18-ru/Correctum/commit/62668a7e05f544f621eae84b218e348bb68b1bc1))
* **WireMock.Net.Minimal:** 2.11.0 ([a2078d8](https://github.com/kkm18-ru/Correctum/commit/a2078d8b5df9d6eb853826a6bc885af9e9f0d690))
* **xunit.runner.visualstudio:** 3.1.5 ([b2bc4a4](https://github.com/kkm18-ru/Correctum/commit/b2bc4a473890aec4447a19f5c5254d62f7518b64))
* **xunit:** 2.9.3 ([63a17d6](https://github.com/kkm18-ru/Correctum/commit/63a17d6c24e34d44f786ca1b35799428b2f1e7ac))

## [1.2.0](https://github.com/kkm18-ru/Correctum/compare/v1.1.0...v1.2.0) (2026-06-02)


### ✨ Нововведения

* Добавлено логирование в Seq ([#110](https://github.com/kkm18-ru/Correctum/issues/110)) ([05cc17f](https://github.com/kkm18-ru/Correctum/commit/05cc17f8101ce09b1cd502a67afb268488469c53))
* Реализован MultiProgressBar ([#104](https://github.com/kkm18-ru/Correctum/issues/104)) ([a73599a](https://github.com/kkm18-ru/Correctum/commit/a73599a5fca904f7598d31d72357a63da01d4a73))


### 🐞 Исправления

* Добавлен манифест Windows-приложения ([e7dc78d](https://github.com/kkm18-ru/Correctum/commit/e7dc78d07a34b57b7ab67e60741854047da9a7fd)), closes [#103](https://github.com/kkm18-ru/Correctum/issues/103)
* Добавлены ограницчения размеров окна отчета ([18d0207](https://github.com/kkm18-ru/Correctum/commit/18d0207ed49fb3726f99dd8669b11d968f92b578))
* Исправлена работа DateRange ([a1c4572](https://github.com/kkm18-ru/Correctum/commit/a1c4572af82b7f062d096c8997a24626b1e6abfb))
* Исправлено отображение таймаута API-клиента ([5d7ab00](https://github.com/kkm18-ru/Correctum/commit/5d7ab00b6f8b58376e46b961d0a7f0eab51f0447))
* Исправлено получение ЕИ в клиенте Астрал ОФД ([d12fa86](https://github.com/kkm18-ru/Correctum/commit/d12fa863e89b99c8987729874454bf0a84328f10))
* Исправлено получение ставки НДС в клиенте Астрал ОФД ([27c6778](https://github.com/kkm18-ru/Correctum/commit/27c677802e4c1f628278f53651a98df96ffdf5fa))
* Исправлено получение ставки НДС в клиенте Первый ОФД ([d94ca07](https://github.com/kkm18-ru/Correctum/commit/d94ca072d4403fa33fe67aabb6d7bdc60fad0b91))
* Исправлено получение ставки НДС в клиенте Такском-Касса ([df7a109](https://github.com/kkm18-ru/Correctum/commit/df7a109baefcfeb16f73a7209b27ea1d08988ac0))


### ⏪ Отмена изменений

* Удалено логирование в Grafana Loki ([9cc4fc6](https://github.com/kkm18-ru/Correctum/commit/9cc4fc66a4b7ce070778ef522b66485232218f7f))

## [1.1.0](https://github.com/kkm18-ru/Correctum/compare/v1.0.0...v1.1.0) (2026-05-22)


### ✨ Нововведения

* **Действия:** Добавлено новое действие "Смена кассира" ([098001b](https://github.com/kkm18-ru/Correctum/commit/098001bed8f8d3e073d102057481619bc6a1e322))
* **Действия:** Добавлено новое действие "Смена метода оплаты" ([912f55b](https://github.com/kkm18-ru/Correctum/commit/912f55b71b3ec0567d1e21e02b4cea1fccd36a10))
* **Действия:** Добавлено новое действие "Смена способа расчета" ([d0d420f](https://github.com/kkm18-ru/Correctum/commit/d0d420f033a898f885aa7ee001b5f222c3e19d8e))
* Добавлена кнопка обратной связи ([be0a1da](https://github.com/kkm18-ru/Correctum/commit/be0a1da8877d965185486e05bb3db926876f6a6e))
* Добавлены иконки для элементов меню ([ac1d0e4](https://github.com/kkm18-ru/Correctum/commit/ac1d0e478eb8b7b45bb830db2d02d1b04af83553))
* **Импорт:** Добавлен импорт из OFD.ru через ЛК ([#95](https://github.com/kkm18-ru/Correctum/issues/95)) ([3d38f98](https://github.com/kkm18-ru/Correctum/commit/3d38f98b2cd3834e2713961f4eec75efe8daa242))
* **Импорт:** Упрощена авторизация для импорта из Астрал ОФД ([2201033](https://github.com/kkm18-ru/Correctum/commit/220103322cb14c27f2bf4e69f5adc43cd53420b3))
* **Импорт:** Упрощена авторизация для импорта из Платформа ОФД ([2c07d68](https://github.com/kkm18-ru/Correctum/commit/2c07d68cc5eaad60d23474dbb25add721cb80801))
* **Отчеты:** Добавлен отчет "Количество чеков по часам" ([688e614](https://github.com/kkm18-ru/Correctum/commit/688e614614a2a0e9ee34266a0132990aec1dcd2f))
* **Отчеты:** Добавлено отображение графика в отчете ([#98](https://github.com/kkm18-ru/Correctum/issues/98)) ([860ab90](https://github.com/kkm18-ru/Correctum/commit/860ab9017df9fa7731d037b1ba94dc580918aef3))
* **Хранилище:** Добавлено Drag&Drop-открытие хранилища ([878f1e6](https://github.com/kkm18-ru/Correctum/commit/878f1e6b5509a13d64c370abe24ad2a05acbc7b1))
* **Хранилище:** Добавлено резервное копирование хранилища ([#96](https://github.com/kkm18-ru/Correctum/issues/96)) ([c7a4a86](https://github.com/kkm18-ru/Correctum/commit/c7a4a86de979cea93f0f02badc20e663b3de003b))


### 🐞 Исправления

* **OfdRuJsonDeserializer:** Добавлен парсинг свойств ([ea93d84](https://github.com/kkm18-ru/Correctum/commit/ea93d840c0c33a2031f06f6ec56581d2e474f637))
* **OfdRuTlvDeserializer:** Добавлен парсинг свойств ([128df14](https://github.com/kkm18-ru/Correctum/commit/128df1473075303cc0e3e30bfd77f5df1789ab0b))
* **Импорт:** Исправлена инициализация логирования импорта ([263f735](https://github.com/kkm18-ru/Correctum/commit/263f7354135fb136bf752da62ce5048148b01e35))
* **Импорт:** Чеки записываются в хранилище в отсортированном виде ([eadd7a1](https://github.com/kkm18-ru/Correctum/commit/eadd7a17d6f6e1a12f9425729e36411776dbd3d6))
* Исправлена кнопка меню Файл&gt;Сохранить как... ([e391dde](https://github.com/kkm18-ru/Correctum/commit/e391dde6f64f8d3daee1940240057753d11513ed))
* **Отчеты:** Удалена излишняя кнопка обновления отчета ([858bee4](https://github.com/kkm18-ru/Correctum/commit/858bee4ce3e767d5d8482d34836e8567e967f69a))


### ⏪ Отмена изменений

* Удален Drop&Drag-импорт SQLite ([c4c6c31](https://github.com/kkm18-ru/Correctum/commit/c4c6c314afb0a10e43070fbec334f7c4bb04459f))

## [1.0.0](https://github.com/kkm18-ru/Correctum/compare/v0.11.0...v1.0.0) (2026-05-19)


### ⚠ BREAKING CHANGES

* Добавлены действия над чеками
* Переработано хранилище

### ✨ Нововведения

* Добавлено Drag&Drop для SQLite-импорта ([ac9ca88](https://github.com/kkm18-ru/Correctum/commit/ac9ca88b186e5e43e07530135d0dafd77f88be7c))
* Добавлены действия над чеками ([094b5b4](https://github.com/kkm18-ru/Correctum/commit/094b5b43a5dd05119b70d656e3e44aebb3b1ecc6))
* Добавлены реквизиты 1117, 1203, 1009, 1021 и 1187 к чеку ([5ea4520](https://github.com/kkm18-ru/Correctum/commit/5ea45208adb556703e87a10c04c129586772b2a6))
* **Импорт:** Реализован импорт из Астрал ОФД ([#92](https://github.com/kkm18-ru/Correctum/issues/92)) ([d55f279](https://github.com/kkm18-ru/Correctum/commit/d55f279abc1189a88ebf7b3fa319d17be56e381c))
* Переработано хранилище ([2b0ed80](https://github.com/kkm18-ru/Correctum/commit/2b0ed80283a510365f5de876880b0ae3721b3102))
* Реализована валидация чеков и их позиций ([fe55c2b](https://github.com/kkm18-ru/Correctum/commit/fe55c2bde34ce31a2f9bd6ee139d15eb3e53763e))
* Реализовано создание, окрытие и сохранение хранилища ([2807bb5](https://github.com/kkm18-ru/Correctum/commit/2807bb5c3daae6a2ee13f072b0a4878d8e6ab2a8))


### 🐞 Исправления

* Восстановлен показ прогресса импорта ([f63eea7](https://github.com/kkm18-ru/Correctum/commit/f63eea7ae22a586611e9bda93b4a0ac80d162d56))
* Исправлен Sqlite-экспорт с пустыми реквизитами ([e53ccad](https://github.com/kkm18-ru/Correctum/commit/e53ccadc267a2a9690578c14f43292349fb9bdc3))
* Стабилизирована работа проведения чеков ([514afcb](https://github.com/kkm18-ru/Correctum/commit/514afcb60a42c89c3796e7b2c00a2a395259e5b8))
* Удалена излишняя структура оплаты ([20a7aa9](https://github.com/kkm18-ru/Correctum/commit/20a7aa984318dfb6e93a698cebb333d4e4bebd30))
* Упрощена работа с сочетанием клавиш в базовом классе диалоговых окон ([70c7719](https://github.com/kkm18-ru/Correctum/commit/70c7719bc1baf1f9231062e73b38e5ed85a5a596))


### ⏪ Отмена изменений

* Удалено перемешение по индексу ([b079c66](https://github.com/kkm18-ru/Correctum/commit/b079c66fd8ab01f9021e76aa73883f87f6bcfdb8))

## [0.11.0](https://github.com/kkm18-ru/Correctum/compare/v0.10.2...v0.11.0) (2026-05-13)


### ✨ Нововведения

* **Отчеты:** Добавлен экспорт отчета в XLSX-таблицу ([#85](https://github.com/kkm18-ru/Correctum/issues/85)) ([62f8cf1](https://github.com/kkm18-ru/Correctum/commit/62f8cf1fc9dcbf4095f7c1302e1af905ba9ac782))


### 🐞 Исправления

* Добавлена обработка отсутствия реквизитов в импортерах ([25c1ff7](https://github.com/kkm18-ru/Correctum/commit/25c1ff7c7cd32e8a6f0ffc93125c0ca9ac06c720))
* Добавлена подсказка сочитания клавиш для вставки чека ([4afe55a](https://github.com/kkm18-ru/Correctum/commit/4afe55a8e0aeecf395e372e2c7a440a9abeebabf))
* **Модели данных:** Удалены лишние nullable ([420a88b](https://github.com/kkm18-ru/Correctum/commit/420a88bc60fc69c18c4a66ed75770f4e53fd331f))
* Подкорректирована работа EnumLocales.GetLocale ([8ae3dff](https://github.com/kkm18-ru/Correctum/commit/8ae3dffc421ee5205bff91e351c23a33ea5c6b80))
* **Ресурсы:** Шрифт перенесен в ресурсы ([73b3ed2](https://github.com/kkm18-ru/Correctum/commit/73b3ed2de67555f7da193dec34fce7f5d78a3e90))
* Тип  данных количество предмета расчета заменен на decimal ([922ce85](https://github.com/kkm18-ru/Correctum/commit/922ce85f917721b36381a324cfc3a78cb835ac9c))


### ⏪ Отмена изменений

* Удален mock-импорт ([90de07d](https://github.com/kkm18-ru/Correctum/commit/90de07de7083e204c10ec901885f77ffbf04f12d))
* Удален импорт из эквайринга ([9dfcc0c](https://github.com/kkm18-ru/Correctum/commit/9dfcc0c8997acb6daaf04f39d6847ecb6b02de8e))

## [0.10.2](https://github.com/kkm18-ru/Correctum/compare/v0.10.1...v0.10.2) (2026-05-06)


### 🐞 Исправления

* **lint:** S3881 ([b635c05](https://github.com/kkm18-ru/Correctum/commit/b635c057b62a31aac905a505c906d1e29243e18a))
* Добавлена проверка значения перечислений ([d34441a](https://github.com/kkm18-ru/Correctum/commit/d34441a740903bb0fdb6060807b142928e62a5b0))
* **Логирование:** Добавлена лог-запись при необработанном исключении ([4fd1955](https://github.com/kkm18-ru/Correctum/commit/4fd1955ecc631e21c7fd939d59b94c6167d30732))
* **Логирование:** Подкорректирован шаблон лог-записи в файл ([d5d5705](https://github.com/kkm18-ru/Correctum/commit/d5d570592cc7cc4f8d19c5af826d938676c503bf))
* **Логирование:** Удалено отображения номера сборки приложения ([c08427f](https://github.com/kkm18-ru/Correctum/commit/c08427f27ce5df63af7fc4794562dde45387f38f))
* Сеттеры построителей принимают null ([093ce3e](https://github.com/kkm18-ru/Correctum/commit/093ce3e1f54a3b967a1f0adf9c2c2bdc8a161a13))


### ⏪ Отмена изменений

* Удалена настройка EoU ([5c9cda5](https://github.com/kkm18-ru/Correctum/commit/5c9cda5751c69bdf6b031bf8487ce3bcfe6c7b8c))

## [0.10.1](https://github.com/kkm18-ru/Correctum/compare/v0.10.0...v0.10.1) (2026-05-05)


### 🐞 Исправления

* Исправлена ошибка привязки MenuItem верт./гориз. выравнивания контента ([63e5b09](https://github.com/kkm18-ru/Correctum/commit/63e5b09aab11313fd9181c84308437849b2e008a))
* Исправление варнингов Correctum.Importers.Acquirer ([16ece5c](https://github.com/kkm18-ru/Correctum/commit/16ece5c3c59260463dde79f5a95bc4ba76c65dc6))
* Исправлены конструкторы DisplayReceipt и DisplayPosition ([0fb2a7c](https://github.com/kkm18-ru/Correctum/commit/0fb2a7c96d6d73438b172f6f3c6e657587abe5d1))
* Перевод главного меню в MVVM ([4136b52](https://github.com/kkm18-ru/Correctum/commit/4136b525c764128198ec337c9f4c92249f74a915))
* Перевод окна ExportWindow в XAML ([748518a](https://github.com/kkm18-ru/Correctum/commit/748518af58b65e808d00c98dab4f2848a7279c06))


### ⬆️ Обновления зависимостей

* **Microsoft.NET.Test.Sdk:** 18.5.1 ([3b0cf8b](https://github.com/kkm18-ru/Correctum/commit/3b0cf8be0f6558ef22ae424fce6f1d82532963c1))
* **Tomlyn:** 2.3.2 ([1087252](https://github.com/kkm18-ru/Correctum/commit/1087252e5ea82b3ce99a7a1a0675975ff726b0c3))
* **WireMock.Net.Minimal:** 2.4.0 ([a274474](https://github.com/kkm18-ru/Correctum/commit/a274474a05638e6475db0e85467cd2ebba52742c))

## [0.10.0](https://github.com/kkm18-ru/Correctum/compare/v0.9.1...v0.10.0) (2026-04-30)


### ✨ Нововведения

* **ui:** Добавлена работа с диапазонами дат ([#52](https://github.com/kkm18-ru/Correctum/issues/52)) ([8ac7104](https://github.com/kkm18-ru/Correctum/commit/8ac7104566e730c705ea25d0a28d8600dadb3db6))
* Реализован импорт из "Первого ОФД" ([#16](https://github.com/kkm18-ru/Correctum/issues/16)) ([580b2ae](https://github.com/kkm18-ru/Correctum/commit/580b2ae8d7b9772a4cbe855f75eea2641d3f9d6e))


### 🐞 Исправления

* Исправлена ошибка с null в Fptr120ExportViewModel.PaymentAsync ([af89c02](https://github.com/kkm18-ru/Correctum/commit/af89c0271a36550234f238f8070b66a3aefe8563))
* Исправлено CS8604 в отчете "Наименования позиций" ([ad6c18a](https://github.com/kkm18-ru/Correctum/commit/ad6c18af6db968d260ea4fe4c545bdc4ba3d7d0b))
* Исправлены варнинги S3459 и S1144 в импортере Sqlite ([8b5519d](https://github.com/kkm18-ru/Correctum/commit/8b5519d39dd428ed7777574a99593da3ed6906d0))
* Установлен абсолютный путь к explorer.exe ([#55](https://github.com/kkm18-ru/Correctum/issues/55)) ([555b6e7](https://github.com/kkm18-ru/Correctum/commit/555b6e7aea8de0091a7a0adfd607c11215636b56))
* Установлена целевая платформа x86 ([9c1da0a](https://github.com/kkm18-ru/Correctum/commit/9c1da0ac176e6b0928daaad0c15b87d889251c4a))
* Установлено максимальное время работы регулярных выражений ([#54](https://github.com/kkm18-ru/Correctum/issues/54)) ([e73397d](https://github.com/kkm18-ru/Correctum/commit/e73397dfc5f992c691a545aec8ac649bf3e12e99))


### ⚡ Улучшение производительности

* Добавлены значительные объекты ([#60](https://github.com/kkm18-ru/Correctum/issues/60)) ([b7c654f](https://github.com/kkm18-ru/Correctum/commit/b7c654f8920b60f2ee5885d68dd14d77e92fc964))


### ⏪ Отмена изменений

* Вырезана система действий над чеками ([#61](https://github.com/kkm18-ru/Correctum/issues/61)) ([08b6f20](https://github.com/kkm18-ru/Correctum/commit/08b6f20d5008a61d8decbb20ca960338a4ae5ac8))
* Удалён импортер FxPOS ([736e48e](https://github.com/kkm18-ru/Correctum/commit/736e48e108c92b08123e8b7e862baaf05f3d5146))


### ⬆️ Обновления зависимостей

* **RetailCorrector.Tests:** WireMock.Net.Minimal 1.25.0 ==&gt; 2.3.0 ([e579240](https://github.com/kkm18-ru/Correctum/commit/e579240a5174e063caaca33ff766e0acff8c9de2))
* **RetailCorrector:** Bogus 35.6.4 ==&gt; 35.6.5 ([4636beb](https://github.com/kkm18-ru/Correctum/commit/4636beb5aa15b5b5b9367b05413ba03786337c3b))
* **RetailCorrector:** Dapper 2.1.66 ==&gt; 2.1.72 ([9f9edf8](https://github.com/kkm18-ru/Correctum/commit/9f9edf892b34534dfe393fed02f4353380638211))
* **RetailCorrector:** Microsoft.Data.Sqlite.Core 9.0.9 ==&gt; 10.0.7 ([563f726](https://github.com/kkm18-ru/Correctum/commit/563f726944011a7417f5dc743510b8a0607a4b35))
* **RetailCorrector:** Microsoft.Extensions.Http.Resilience 10.0.0 ==&gt; 10.5.0 ([b42f124](https://github.com/kkm18-ru/Correctum/commit/b42f12417820188ef380b5f976d32a8ef63d4365))
* **RetailCorrector:** NLog 6.1.0 ==&gt; 6.1.2 ([e87eaea](https://github.com/kkm18-ru/Correctum/commit/e87eaea2bb7e2c8393ee76f40419df114d428de3))
* **RetailCorrector:** NLog.Targets.Loki 2.4.0 ==&gt; 2.5.0 ([e2cbb8a](https://github.com/kkm18-ru/Correctum/commit/e2cbb8a145ceb992ae5e16633b0624800b0a6489))
* **RetailCorrector:** Polly 8.6.5 ==&gt; 8.6.6 ([d7c1a8d](https://github.com/kkm18-ru/Correctum/commit/d7c1a8d065f7c2c0b9101ea893d59d14e0ddd717))
* **RetailCorrector:** System.ServiceProcess.ServiceController 9.0.10 ==&gt; 10.0.7 ([1c1bfe6](https://github.com/kkm18-ru/Correctum/commit/1c1bfe66660d3ba8a99764699829538ff38954f9))
* **RetailCorrector:** Tomlyn 0.19.0 ==&gt; 2.3.1 ([0be5c45](https://github.com/kkm18-ru/Correctum/commit/0be5c45943eaeb267e806357f7de3158e95aa9e8))
* **tests:** coverlet.collector 6.0.4 ==&gt; 10.0.0 ([232fda0](https://github.com/kkm18-ru/Correctum/commit/232fda0f0ba2647a289623a4b9edb9be245351da))
* **tests:** Microsoft.NET.Test.Sdk ==&gt; 18.5.0 ([0cabf9b](https://github.com/kkm18-ru/Correctum/commit/0cabf9bf9499d6e59d06e686fc7346309f7034bd))
* **tests:** NUnit ==&gt; 4.5.1 ([18832cb](https://github.com/kkm18-ru/Correctum/commit/18832cb4ff4c71a527b283aa49dd8054a560a357))
* **tests:** NUnit.Analyzers ==&gt; 4.12.0 ([c3bc9c9](https://github.com/kkm18-ru/Correctum/commit/c3bc9c9de48c1edd41e6112d7e8713d2ab68b98b))
* **tests:** NUnit3TestAdapter ==&gt; 6.2.0 ([86377e8](https://github.com/kkm18-ru/Correctum/commit/86377e8a6884045eedb3bc0251eb9fda2ca747eb))

## [0.9.1](https://github.com/kkm18-ru/Correctum/compare/v0.9.0...v0.9.1) (2026-04-01)


### 🐞 Исправления

* **sqlite:** Добавлено сохранение и загрузка СНО в/из SQLite ([#38](https://github.com/kkm18-ru/Correctum/issues/38)) ([da52dce](https://github.com/kkm18-ru/Correctum/commit/da52dce8235f10031ff8e0ae97fcbb3c58322c99))

## [0.9.0](https://github.com/kkm18-ru/Correctum/compare/v0.8.0...v0.9.0) (2026-03-31)


### ✨ Нововведения

* **report:** Добавлен отчет "Наименования позиций" ([#47](https://github.com/kkm18-ru/Correctum/issues/47)) ([fb29832](https://github.com/kkm18-ru/Correctum/commit/fb29832b74798b470965acd9e71bdbd6f3cde8af))


### 🐞 Исправления

* **client-taxcom:** Исправлен доступ к константе SessionHeaderName ([fa8e0e1](https://github.com/kkm18-ru/Correctum/commit/fa8e0e141f660263a6744d08999fc34bf1d6f782))
* **client-taxcom:** Исправлен доступ к свойству SessionToken ([#48](https://github.com/kkm18-ru/Correctum/issues/48)) ([f772cd7](https://github.com/kkm18-ru/Correctum/commit/f772cd7833f3a34ddd0561b02900bd6d15565f1d))
* Добавлена поддержка архитектуры x86 ([#42](https://github.com/kkm18-ru/Correctum/issues/42)) ([9e302a6](https://github.com/kkm18-ru/Correctum/commit/9e302a67e19216431ae765fc9c8276c320b3b1b5))
* Исправлен парсинг количества в парсере PlatformOfd ([#40](https://github.com/kkm18-ru/Correctum/issues/40)) ([16cd4ad](https://github.com/kkm18-ru/Correctum/commit/16cd4adfaf3b99a5c0f13bb1fef3244adecd3ee8))
* Исправлен цикл импорта из ПлатформаОФД ([#41](https://github.com/kkm18-ru/Correctum/issues/41)) ([af6a505](https://github.com/kkm18-ru/Correctum/commit/af6a505a1e0a7e85b46d0a328c4a9a535bfe0f54))
* Исправлена обрезка строк в парсере PlatformOfd ([#39](https://github.com/kkm18-ru/Correctum/issues/39)) ([14c62fd](https://github.com/kkm18-ru/Correctum/commit/14c62fd40c71315b5fc103d29649b3ef5073a2b4))

## [0.8.0](https://github.com/ornaras/RetailCorrector/compare/v0.7.1...v0.8.0) (2026-03-01)


### ✨ Нововведения

* Добавлено централизированное логирование ([#22](https://github.com/ornaras/RetailCorrector/issues/22)) ([611d1e6](https://github.com/ornaras/RetailCorrector/commit/611d1e678f3dcf9b563056079571755400684ccd))
* Улучшена безопасность чувствительных данных ([#29](https://github.com/ornaras/RetailCorrector/issues/29)) ([e450a82](https://github.com/ornaras/RetailCorrector/commit/e450a821b6c9eff376a6a17f72df93bcb5c0bcff))


### 🐞 Исправления

* Добавлены исполнении ФН в конвертер ([e8ec72a](https://github.com/ornaras/RetailCorrector/commit/e8ec72af5f812ae4a0ba194f50a039d846fef9e6))
* Исправлены жизненные циклы API-клиентов ([#25](https://github.com/ornaras/RetailCorrector/issues/25)) ([08f1005](https://github.com/ornaras/RetailCorrector/commit/08f10056cbb8d244a24511978a90903f3266a536))
* Удалены лишние пустые символы в исполнении ФН ([b586bb2](https://github.com/ornaras/RetailCorrector/commit/b586bb217162a09bda2282e570126ae7e3645f4c))

## [0.7.1](https://github.com/ornaras/RetailCorrector/compare/v0.7.0...v0.7.1) (2026-02-12)


### 🐞 Исправления

* Восстановлен импорт из Такском-Касса ([#19](https://github.com/ornaras/RetailCorrector/issues/19)) ([b00dd19](https://github.com/ornaras/RetailCorrector/commit/b00dd19cf992668d09f68bf8709430abe78b03f5))


### ⏪ Отмена изменений

* Удалено окно обратной связи ([f61ba8a](https://github.com/ornaras/RetailCorrector/commit/f61ba8a7181a35aff879dce9a76d54ac89b5b0ce))

## [0.7.0](https://github.com/ornaras/retailcorrector/compare/0.6.0...0.7.0) (2026-01-20)


### ✨ Нововведения

* Обновлен формат заголовка группы позиций ([af2ab00](https://github.com/ornaras/retailcorrector/commit/af2ab007346c38f73f860bcd1e624146aa739ffb))

### 🐞 Исправления

* Ширина столбцов таблиц изменяемая ([a8ec190](https://github.com/ornaras/retailcorrector/commit/a8ec190601fb8ed8914b6bc12748ebe7a8e3717f))

### ⚡ Улучшение производительности

* Оптимизирован парсер Платформа ОФД ([!88](https://gitlab.com/ornaras/retailcorrector/merge_requests/88)) ([69b27ec](https://github.com/ornaras/retailcorrector/commit/69b27ecaff5caa5a2e1ec28c9de329375135e36b))

## [0.6.0](https://github.com/ornaras/retailcorrector/compare/0.5.0...0.6.0) (2026-01-16)


### ✨ Нововведения

* Добавлены ставки НДС 18%, 18/118, 22%, 22/122 ([!87](https://gitlab.com/ornaras/retailcorrector/merge_requests/87)) ([4ad54ae](https://github.com/ornaras/retailcorrector/commit/4ad54ae221f3811b776ea9ef9dd97405e3b48abe))
* Переработано окно проведения ЧК ([!86](https://gitlab.com/ornaras/retailcorrector/merge_requests/86)) ([0dae620](https://github.com/ornaras/retailcorrector/commit/0dae62091c7bcec12e79e9175445c7363d276d64))
* Добавлено оповещение об авто-определении ККТ ([!84](https://gitlab.com/ornaras/retailcorrector/merge_requests/84)) ([50cd016](https://github.com/ornaras/retailcorrector/commit/50cd0160db8620becc5bb6c3f19c6057150eea47))
* Реализовано меню для главного окна ([!83](https://gitlab.com/ornaras/retailcorrector/merge_requests/83)) ([8f3dca4](https://github.com/ornaras/retailcorrector/commit/8f3dca41b3433f9932f759bbf14bd81fed73d4ae))
* Реализовано отдельное окно для экспорта чеков ([!82](https://gitlab.com/ornaras/retailcorrector/merge_requests/82)) ([9bf672f](https://github.com/ornaras/retailcorrector/commit/9bf672f8ff7dc0d6e7eab4ecc08220818416a1ff))
* Добавлена интеграция с "ККТ Онлайн" ([!80](https://gitlab.com/ornaras/retailcorrector/merge_requests/80)) ([4930c33](https://github.com/ornaras/retailcorrector/commit/4930c33c012ba059ffb2029f564b36a101f8ae0d))
* Добавлена проверка подключения к ККТ ([db92730](https://github.com/ornaras/retailcorrector/commit/db927301f880329907efabdd9bbb8d32acf0158c))
* Открытие смены и документ производятся только в "тихом" режиме ([!79](https://gitlab.com/ornaras/retailcorrector/merge_requests/79)) ([c5ad3b9](https://github.com/ornaras/retailcorrector/commit/c5ad3b96ba4e0521d68ab3d0c786b45a7d21012c))
* Добавлено оповещение о невозможности проведения ЧК без печати ([!81](https://gitlab.com/ornaras/retailcorrector/merge_requests/81)) ([e4cd852](https://github.com/ornaras/retailcorrector/commit/e4cd8520adc6c2d180fbb665b929cb43fb5bb021))
* Добавлен параметр refresh-токена ([!77](https://gitlab.com/ornaras/retailcorrector/merge_requests/77)) ([1e8116b](https://github.com/ornaras/retailcorrector/commit/1e8116beb73dc4056b2a7cd178c97340937957c9))
* Добавлен выбор нескольких ККТ для парсинга ([!69](https://gitlab.com/ornaras/retailcorrector/merge_requests/69)) ([ce165ff](https://github.com/ornaras/retailcorrector/commit/ce165ff4e87ba79788c0f49a96acbb9fe7e84699))
* Добавлены оригинальные идентификаторы ФД ([!68](https://gitlab.com/ornaras/retailcorrector/merge_requests/68)) ([7e61e13](https://github.com/ornaras/retailcorrector/commit/7e61e139e9e3e83a8756a1985d51007f9b718210))

### 🐞 Исправления

* Исправлено проведение электронных чеков при версии конфигурации 5.17.0 и выше ([!85](https://gitlab.com/ornaras/retailcorrector/merge_requests/85)) ([e731155](https://github.com/ornaras/retailcorrector/commit/e7311557d2dde1ddc7079f6abbd78b71c6a2a8ea))
* Исправлено действие при неудачной проверке пробития эл. чеков ([2000782](https://github.com/ornaras/retailcorrector/commit/20007823eaefd2f1d4994ac6cb54c331401b5ab7))
* Исправлены координаты элементов Fptr-окна ([f946b72](https://github.com/ornaras/retailcorrector/commit/f946b7241101c661320b90566f4029dc1b0c5e48))
* Исправлен фильтр чеков по ККТ в интеграции "Платформа ОФД" ([!78](https://gitlab.com/ornaras/retailcorrector/merge_requests/78)) ([f5bc3a9](https://github.com/ornaras/retailcorrector/commit/f5bc3a9cf6ce0e7dabcdf5293220eb49a7b5abec))
* Исправлен SQLite-запрос для создания таблицы чеков ([!75](https://gitlab.com/ornaras/retailcorrector/merge_requests/75)) ([6750c1d](https://github.com/ornaras/retailcorrector/commit/6750c1da6af9d6c5d8cb238bd4e4e13daa27a4c5))
* Исправлена работа исправления ошибок ККТ ([!74](https://gitlab.com/ornaras/retailcorrector/merge_requests/74)) ([7523afe](https://github.com/ornaras/retailcorrector/commit/7523afe33025930f9499a50ff147e47f315b86be))
* Исправлена работа импорта с включенным VPN ([!73](https://gitlab.com/ornaras/retailcorrector/merge_requests/73)) ([355096c](https://github.com/ornaras/retailcorrector/commit/355096c359e72e3b51f994b4a2eb6adcc6bc022d))
* Исправлено отображение прогресса пробития чеков в Fptr-интеграции ([!72](https://gitlab.com/ornaras/retailcorrector/merge_requests/72)) ([2472f25](https://github.com/ornaras/retailcorrector/commit/2472f2549228bf9abcc81eec6eb7bec67516c621))

## [0.5.0](https://github.com/ornaras/retailcorrector/compare/0.4.0...0.5.0) (2025-12-12)


### ✨ Нововведения

* Добавлен период ожидания выгрузки ФД в ОФД ([!63](https://gitlab.com/ornaras/retailcorrector/merge_requests/63)) ([2cb2ddc](https://github.com/ornaras/retailcorrector/commit/2cb2ddcb55c4d41f6a273e2121e905115a8123c6))
* Добавлен парсер Платформа ОФД ([!60](https://gitlab.com/ornaras/retailcorrector/merge_requests/60)) ([cc8f453](https://github.com/ornaras/retailcorrector/commit/cc8f453e73cbf4394679bfa07db8feeaac2c82aa))
* Добавлен генератор чеков по истории операций эквайринга ([!59](https://gitlab.com/ornaras/retailcorrector/merge_requests/59)) ([f85d50f](https://github.com/ornaras/retailcorrector/commit/f85d50fd520ef97ac7822d293c899aa4529a24ae))
* Реализована обертка API-клиента OFD.ru ([!58](https://gitlab.com/ornaras/retailcorrector/merge_requests/58)) ([b292bbc](https://github.com/ornaras/retailcorrector/commit/b292bbc9ea7d3965cb43ba1f8f7e7eb1bc3b1ba2))
* Добавлена форма обратной связи ([!57](https://gitlab.com/ornaras/retailcorrector/merge_requests/57)) ([b5de4b3](https://github.com/ornaras/retailcorrector/commit/b5de4b310197c400f8ad2e0b84fb29ff42c56aef))
* Реализована обертка API-клиента SabyOFD ([!56](https://gitlab.com/ornaras/retailcorrector/merge_requests/56)) ([5d61950](https://github.com/ornaras/retailcorrector/commit/5d619502daa006cbd1c2aa4640be1e13d37896de))
* Добавлена настройка режима печати отчетов ККТ Atol ([39ca85d](https://github.com/ornaras/retailcorrector/commit/39ca85d9c26f731ab509a561829b79f7bdaeb270))
* Добавлено свойство "СНО" в структуру чека ([!50](https://gitlab.com/ornaras/retailcorrector/merge_requests/50)) ([f048bde](https://github.com/ornaras/retailcorrector/commit/f048bde1597ca2878c6570de85567041c0255f09))

### 🐞 Исправления

* Исправлено время жизни дескриптора SQLite-файла ([!65](https://gitlab.com/ornaras/retailcorrector/merge_requests/65)) ([111b880](https://github.com/ornaras/retailcorrector/commit/111b880bc8c4065d41aeb416d697a536f602b79a))
* Исправлено получение пути драйвера Fptr ([6fe47d4](https://github.com/ornaras/retailcorrector/commit/6fe47d40347333013059aa9dc0fa9fedd1116104))
* Восстановлена работа конфигурации ([!52](https://gitlab.com/ornaras/retailcorrector/merge_requests/52)) ([0f99c8f](https://github.com/ornaras/retailcorrector/commit/0f99c8fe286d5885c1121dcbd64cc30d7dec5954))
* Исправлено использование конфигурации интеграцией Atol ([f2b63ee](https://github.com/ornaras/retailcorrector/commit/f2b63eec882448b0520149b297b9d28939c18e84))
* Переработана интеграция с ККТ Atol ([!51](https://gitlab.com/ornaras/retailcorrector/merge_requests/51)) ([1600c4f](https://github.com/ornaras/retailcorrector/commit/1600c4f6394075dec1d66e30ac16972b8eb12390))

## [0.4.0](https://github.com/ornaras/retailcorrector/compare/0.3.0...0.4.0) (2025-11-18)


### ✨ Нововведения

* Добавлено автоматическое пере-открытие смены по переполнению 30-дневного хранилища ([!40](https://gitlab.com/ornaras/retailcorrector/merge_requests/40)) ([9d047d6](https://github.com/ornaras/retailcorrector/commit/9d047d6968124be9101825f7c16d3955c19d706f))
* Добавлена поддержка расчетов в сети "Интернет" ([!41](https://gitlab.com/ornaras/retailcorrector/merge_requests/41)) ([a4f8fc4](https://github.com/ornaras/retailcorrector/commit/a4f8fc4c8534e820de9cc239b95c3f20013a2d56))
* Файл решения мигрирован в SLN-формат ([!43](https://gitlab.com/ornaras/retailcorrector/merge_requests/43)) ([b2ea32d](https://github.com/ornaras/retailcorrector/commit/b2ea32df5930b6ea080e34d77f3f2a4bb0dd059d))
* settings: Добавлена настройка EoU-порта ([!44](https://gitlab.com/ornaras/retailcorrector/merge_requests/44)) ([4a841ad](https://github.com/ornaras/retailcorrector/commit/4a841ad473efa0d1e2324c875bb45c1cd25c8035))
* Добавлен конвертер ставок НДС для Atol ([!47](https://gitlab.com/ornaras/retailcorrector/merge_requests/47)) ([471aef7](https://github.com/ornaras/retailcorrector/commit/471aef7336fcbd7d71bb9958cf0d3d943aa70cb0))
* Добавлен десериализатор чека с ОФД.ру формата JSON ([!49](https://gitlab.com/ornaras/retailcorrector/merge_requests/49)) ([ea3dfc3](https://github.com/ornaras/retailcorrector/commit/ea3dfc3a035ea3dab016e56404fc4f437d299e86))

### 🐞 Исправления

* sqlite-export: Исправлены SQLite-запросы с использованием столбца buyer ([!45](https://gitlab.com/ornaras/retailcorrector/merge_requests/45)) ([1fc1d1f](https://github.com/ornaras/retailcorrector/commit/1fc1d1fea735babae5f65c950ef6cfbc6c1f9c73))
* Удалена зависимость от Kraig.Roslyn ([!46](https://gitlab.com/ornaras/retailcorrector/merge_requests/46)) ([b370b42](https://github.com/ornaras/retailcorrector/commit/b370b42896b287a5cf184ec88a45f89da9265c58))

## [0.3.0](https://github.com/ornaras/retailcorrector/compare/0.2.0...0.3.0) (2025-10-22)


### Нововведения

* Добавлена иконка приложения ([!32](https://gitlab.com/ornaras/retailcorrector/merge_requests/32))
* Реализован выбор ККТ для парсера OfdRu ([!33](https://gitlab.com/ornaras/retailcorrector/merge_requests/33))
* Реализована система десериализаторов чеков ([!36](https://gitlab.com/ornaras/retailcorrector/merge_requests/36))
* Добавлена регистрация кассира при отбитии чеков ([!39](https://gitlab.com/ornaras/retailcorrector/merge_requests/39))

### Исправления

* Исправлены UI Fptr-экспортера и порядок в Fptr-сскрипте ([!30](https://gitlab.com/ornaras/retailcorrector/merge_requests/30))
* Исправлено отображение группы позиций ([!31](https://gitlab.com/ornaras/retailcorrector/merge_requests/31))
* Исправлен порядок создания SQLite-базы ([!35](https://gitlab.com/ornaras/retailcorrector/merge_requests/35))
* Исправлена неисправимая ошибка "Нет больше данных" при работе Fptr-драйвера ([!38](https://gitlab.com/ornaras/retailcorrector/merge_requests/38))

## [0.2.0](https://github.com/ornaras/retailcorrector/compare/0.1.0...0.2.0) (2025-09-15)


### Нововведения

* Реализовано пробитие чеков через драйвер Atol v.10 ([!25](https://gitlab.com/ornaras/retailcorrector/merge_requests/25))
* Реализован парсер с SabyOfd ([!26](https://gitlab.com/ornaras/retailcorrector/merge_requests/26))
* Добавлено окно настройки ([!27](https://gitlab.com/ornaras/retailcorrector/merge_requests/27))
* Реализована настройка экспорта в Fptr-скрипт ([!29](https://gitlab.com/ornaras/retailcorrector/merge_requests/29))

## [0.1.0](https://github.com/ornaras/retailcorrector/commits/0.1.0) (2025-09-05)

# Changelog

## [0.9.0](https://github.com/kkm18-ru/Correctum/compare/v0.8.0...v0.9.0) (2026-03-31)


### ✨ Нововведения

* **report:** Добавлен отчет "Наименования позиций" ([#47](https://github.com/kkm18-ru/Correctum/issues/47)) ([fb29832](https://github.com/kkm18-ru/Correctum/commit/fb29832b74798b470965acd9e71bdbd6f3cde8af))


### 🐞 Исправления

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

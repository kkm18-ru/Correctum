using System.Net.Http;

namespace RetailCorrector.Utils.Integrations.Clients;

/// <summary>API-клиент ОФД Такском-Касса</summary>
/// <seealso href="https://lk-ofd.taxcom.ru/ApiHelp/">Документация интегратора</seealso>
internal class TaxcomClient(string host = "https://api-lk-ofd.taxcom.ru") : APIClient
{
    private readonly HttpClient _http = new(Handler, false) { BaseAddress = new Uri(host) };
    
    // todo 3.1. Аутентификация и авторизация 
    // todo 3.4.1. Список торговых точек (Номер страницы результатов; Кол-во результатов на странице)
    // todo 3.5.1. Список ККТ по торговой точке (ИД торговой точки)
    // todo 3.5.2. Список фискальных накопителей по ККТ (Регистрационный номер ККТ)
    // todo 3.6.1. Список смен за период по ККТ (Номер фискального накопителя; Период; Номер страницы результатов; Кол-во результатов на странице)
    // todo 3.7.1. Список документов по смене (Номер фискального накопителя; Номер смены; Номер страницы результатов; Кол-во результатов на странице)
    // todo 3.7.2. Данные документа (Номер фискального накопителя; Номер фискального документа)

    ~TaxcomClient() => _http.Dispose();
}
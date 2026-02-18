using Microsoft.Win32;
using System.IO;
using System.ServiceProcess;
using System.Xml;
using System.Xml.Linq;

namespace RetailCorrector.Utils.Integrations
{
    internal static class EoU
    {
        private static readonly string ConfPath = 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Atol\EoU\settings.xml");
        private static readonly ServiceController Service = new("EoU");
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly Lock ServiceLock = new();

        public static byte? CurrentPort
        {
            get
            {
                const string invalidFormatMessage = "Некорректный формат конфигурации EoU: {Reason}."; 
                if (!File.Exists(ConfPath))
                {
                    Logger.Error("Файл по пути \"{Path}\" отсутствует.", ConfPath);
                    return null;
                }
                var xml = new XmlDocument();
                try
                {
                    Logger.Trace("Загрузка XML из файла \"{Path}\".", ConfPath);
                    xml.Load(ConfPath);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Не удалось загрузить XML из файла \"{Path}\"", ConfPath);
                }
                var root = xml.DocumentElement;
                if (root is null)
                {
                    Logger.Error(invalidFormatMessage, "отсутствует корень конфигурации");
                    return null;
                }
                if (root.ChildNodes.Count == 0)
                {
                    Logger.Warn(invalidFormatMessage, "отсутствуют дочерние элементы корня");
                    return null;
                }
                var dev = root.ChildNodes[0];
                if (dev is null)
                {
                    Logger.Warn(invalidFormatMessage, "первый дочерний элемент корня - null");
                    return null;
                }

                var prop = dev.Cast<XmlNode>().FirstOrDefault(node => node?.Name == "port", null);
                if (prop is not null)
                {
                    Logger.Info("Получен номер используемого поледовательного порта: COM{SerialPort}", prop.InnerText);
                    return byte.Parse(prop.InnerText);
                }
                Logger.Error(invalidFormatMessage, "не удалось найти дочерний элемент с тегом port");
                return null;
            }
            set
            {
                var xml = new XDocument(new XElement("settings",
                    new XElement("device",
                        new XElement("id", 1),
                        new XElement("port", value)
                        )
                    ));
                xml.Save(ConfPath);
                Logger.Info("Сохранена конфигурация EoU.");
                new Thread(Restart) { IsBackground = true }.Start();
            }
        }

        public static IEnumerable<string> GetAvailablePorts()
        {
            const string path = @"HARDWARE\DEVICEMAP\SERIALCOMM";
            const string fullPath = $@"\\HKLM\{path}";
            const string atolServiceTemplate = @"\Device\atol-usbcom_proxy";
            
            using var localMachine = Registry.LocalMachine;
            Logger.Trace("Открытие пути \"{Path}\".", fullPath);
            using var mapSerial = localMachine.OpenSubKey(path);
            if (mapSerial is null)
            {
                Logger.Warn("Не удалось открыть путь \"{Path}\".", fullPath);
                yield break;
            }
            Logger.Trace("Получение всех ключей в \"{Path}\".", fullPath);
            var services = mapSerial.GetValueNames();
            Logger.Info("Получены следующие SerialPort-сервисы: {Services}.", services);
            foreach (var srv in services.Where(srv => srv.StartsWith(atolServiceTemplate)))
                yield return (string)mapSerial.GetValue(srv)!;
        }

        private static void Restart()
        {
            lock (ServiceLock) {
                Logger.Trace("Остановка сервиса \"{ServiceName}\".", Service.ServiceName);
                Service.Stop();
                Logger.Trace("Ожидание завершения работы сервиса \"{ServiceName}\"...", Service.ServiceName);
                Service.WaitForStatus(ServiceControllerStatus.Stopped);
                Logger.Trace("Запуск сервиса \"{ServiceName}\".", Service.ServiceName);
                Service.Start();
                Logger.Trace("Ожидание продолжения работы сервиса \"{ServiceName}\"...", Service.ServiceName);
                Service.WaitForStatus(ServiceControllerStatus.Running);
            }
            Logger.Info("Сервис \"{ServiceName}\" перезапущен.", Service.ServiceName);
        }
    }
}

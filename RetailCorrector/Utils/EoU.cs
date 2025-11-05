using Microsoft.Win32;
using System.IO;
using System.ServiceProcess;
using System.Xml;
using System.Xml.Linq;

namespace RetailCorrector.Utils
{
    internal static class EoU
    {
        private readonly static string ConfPath = 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Atol\EoU\settings.xml");
        private readonly static ServiceController Service = new("EoU");
        private readonly static Lock ServiceLock = new();

        public static byte CurrentPort
        {
            get
            {
                var xml = new XmlDocument();
                xml.Load(ConfPath);
                var root = xml.DocumentElement ?? throw new Exception("Отсутствует корневой элемент");
                if(root.ChildNodes.Count == 0)
                    throw new Exception("Отсутствуют дочерние элементы корня");
                var dev = root.ChildNodes[0] ?? throw new Exception("Первый дочерний элемент корня - null");
                foreach (XmlNode prop in dev)
                {
                    if (prop.Name != "port") continue;
                    return byte.Parse(prop.InnerText);
                }
                throw new Exception("Не удалось найти дочерний элемент с тегом port");
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
                new Thread(Restart) { IsBackground = true }.Start();
            }
        }

        public static IEnumerable<string> GetAvailablePorts()
        {
            using var hklm = Registry.LocalMachine;
            using var serialcomm = hklm.OpenSubKey(@"HARDWARE\DEVICEMAP\SERIALCOMM") ?? 
                throw new Exception(@"Не удалось открыть путь ""\\HKLM\HARDWARE\DEVICEMAP\SERIALCOMM""");
            var keys = serialcomm.GetValueNames();
            foreach (var k in keys.Where(_ => _.StartsWith(@"\Device\atol-usbcom_proxy")))
                yield return (string)serialcomm.GetValue(k)!;
        }

        private static void Restart()
        {
            lock (ServiceLock) {
                Service.Stop();
                Service.WaitForStatus(ServiceControllerStatus.Stopped);
                Service.Start();
                Service.WaitForStatus(ServiceControllerStatus.Running);
            }
        }
    }
}

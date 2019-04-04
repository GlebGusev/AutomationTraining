using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Initialize;
using Newtonsoft.Json;

namespace Task_Final
{
    public class AssemblyConfiguration : TestBase
    {
        //public void UpdateAllureConfig()
        //{
        //    var allureConfig = Initialize.AllureConfig.AllureConfig.Factory.CreateConfig(_browser, _ci);

        //    using (var file = File.CreateText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "allureConfig.json")))
        //    {
        //        var serializer = new JsonSerializer();
        //        serializer.NullValueHandling = NullValueHandling.Ignore;

        //        serializer.Serialize(file, allureConfig);
        //    }
        //}

        public Dictionary<string, string> GetCredentialsFromDataFile()
        {
            var doc = new XmlDocument();
            doc.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestData\TestData.xml"));

            var nodes = doc.SelectNodes("testData/credential");
            var dictionary = new Dictionary<string, string>();
            var count = nodes.Count;
            for (var i = 0; i < count; i++)
            {
                dictionary.Add(nodes.Item(i).Attributes.GetNamedItem("user").Value, nodes.Item(0).Attributes.GetNamedItem("password").Value);
            }

            return dictionary;
        }
    }
}

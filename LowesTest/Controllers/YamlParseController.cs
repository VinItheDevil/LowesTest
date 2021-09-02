using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Diagnostics;
using System.Threading;

namespace LowesTest.Controllers
{
    public class YamlParseController : ApiController
    {
        public class MyClass
        {
            public Dictionary<string, List<string>> Color { get; set; }
        }
        public HttpResponseMessage Post()
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            Dictionary<string, int> countDic = new Dictionary<string, int>();
            int rCount = 0, bCount = 0;
            if (httpRequest.Files.Count > 0 && httpRequest.Files.Count <= 100)
            {
                const int NumberOfRetries = 3;
                const int DelayOnRetry = 1000;
                for (int i = 1; i <= NumberOfRetries; ++i)
                {
                    try
                    {
                        foreach (string file in httpRequest.Files)
                        {
                            var postedFile = httpRequest.Files[file];
                            var filePath = HttpContext.Current.Server.MapPath("~/App_Data/");
                            filePath = Path.Combine(filePath, postedFile.FileName);
                            postedFile.SaveAs(filePath);
                            var deserializer = new DeserializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .Build();
                            var streamRead = new StreamReader(filePath);
                            var yamlObject = deserializer.Deserialize<Dictionary<string, List<string>>>(streamRead);

                           // Debug.WriteLine(yamlObject.ToString());
                            if (yamlObject.Keys.Where(x => x.Equals("color")).Any())
                            {
                                var colorList = yamlObject.Values.ToList();
                                foreach (var s in colorList[0])
                                {
                                    if (s.Equals("red"))
                                        rCount++;
                                    else if (s.Equals("blue"))
                                        bCount++;
                                }
                            }
                            streamRead.Close();
                        }

                        break; 
                    }
                    catch (IOException e) when (i <= NumberOfRetries)
                    {
        
                        Thread.Sleep(DelayOnRetry);
                    }
                }              
                countDic.Add("Red Count", rCount);
                countDic.Add("Blue Count", bCount);
                result = Request.CreateResponse(HttpStatusCode.OK, countDic);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            return result;
        }
    }
}

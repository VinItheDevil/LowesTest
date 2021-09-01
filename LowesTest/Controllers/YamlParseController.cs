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
namespace LowesTest.Controllers
{
    public class YamlParseController : ApiController
    {
        public HttpResponseMessage Post()
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            Dictionary<string, int> countDic= new Dictionary<string, int>();
            if (httpRequest.Files.Count > 0)
            {
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var filePath = HttpContext.Current.Server.MapPath("~/App_Data/" + postedFile.FileName);
                    System.IO.Directory.CreateDirectory(filePath);
                    postedFile.SaveAs(Path.Combine(filePath, postedFile.FileName));
                    var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
                    var yamlObject = (List<Object>)deserializer.Deserialize(new StreamReader(filePath));
                    int rCount = 0, bCount = 0;
                    foreach (var s in yamlObject)
                    {
                        if (s.ToString() == "red")
                            rCount++;
                        else if (s.ToString() == "blue")
                            bCount++;
                    }
                    countDic.Add("Red Count", rCount);
                    countDic.Add("Blue Count", bCount);
                    //Debug.WriteLine(rCount + " " + bCount);
                }
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

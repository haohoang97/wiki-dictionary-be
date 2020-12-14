using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Web.Http.Cors;
using Project.Models;
using System.Net.Http;
using System.Net;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using Nancy.Json;
using System.Text.Json;
using System.IO;
using System.Net.Http.Headers;

namespace Project.Controllers
{
    [Route("api/words")]
    [ApiController]
    [System.Web.Http.Cors.EnableCors("*", "*", "*")]
    public class WordsController : ControllerBase
    {
        private MongoClient client;
        private IMongoDatabase db;

        private IMongoCollection<Words> EnglishWords;
        private IMongoCollection<Words> VietNamWords;
        private IMongoCollection<Words> Relations;
        private IMongoCollection<MultiLanguageWord> MultiLanguage;
        private IEnumerable<Words> listAllWord;

        private const string key = "bfec5ff7315845009c11d495fe7d65c0";
        public WordsController()
        {
            this.client = new MongoClient("mongodb://localhost:27017/?readPreference=primary&appname=MongoDB%20Compass&ssl=false");
            this.db = client.GetDatabase("Wikipedia");

            EnglishWords = db.GetCollection<Words>("EnglishWord");
            VietNamWords = db.GetCollection<Words>("VietNamWord");
            Relations = db.GetCollection<Words>("Relation");
            MultiLanguage = db.GetCollection<MultiLanguageWord>("MultiLanguage");
        }


        // GET: api/<Words>
        [HttpGet]
        public async Task<IEnumerable<Words>> Get()
        {
            List<Words> EnglistWord = await (await EnglishWords.FindAsync(_ => true)).ToListAsync();
            List<Words> VietnamWord = await (await VietNamWords.FindAsync(_ => true)).ToListAsync();
            listAllWord = EnglistWord.Concat(VietnamWord);
            return listAllWord;
        }

        /// <summary>
        /// Tìm từ chính xác
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost("multiLanguage")]
        public Dictionary<string, object> multiLanguage([FromBody] Dictionary<string, string> obj)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            IEnumerable<MultiLanguageWord> w;
            if (obj.ContainsKey("LanguageFrom") && !string.IsNullOrEmpty(obj["LanguageFrom"]))
            {
                w = MultiLanguage.Find(s => (s.WordValue.ToLower() == obj["Value"].ToLower() && s.Language.ToLower() == obj["LanguageFrom"].ToLower())).ToListAsync().Result;
            }
            else
            {
                w = MultiLanguage.Find(s => s.WordValue.ToLower() == obj["Value"].ToLower()).ToListAsync().Result;
            }
            if (w == null || w.Count() == 0)
            {
                return result;
            }
            string assignID = w.FirstOrDefault<MultiLanguageWord>().AssignID;
            IEnumerable<MultiLanguageWord> data;
            if (obj.ContainsKey("LanguageTo") && !string.IsNullOrEmpty(obj["LanguageTo"]))
            {
                data = MultiLanguage.Find(s => (s.AssignID.ToLower() == assignID.ToLower() && s.Language.ToLower() == obj["LanguageTo"].ToLower())).ToListAsync().Result;
            }
            else
            {
                data = MultiLanguage.Find(s => s.AssignID.ToLower() == assignID.ToLower()).ToListAsync().Result;
            }
            result.Add("LanguageFrom", w.FirstOrDefault().Language);
            result.Add("Data", data);
            return result;
        }

        /// <summary>
        /// Tìm từ gần đúng
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost("multiLanguageByLanguage")]
        public Dictionary<string, object> multiLanguageByLanguage([FromBody] Dictionary<string, string> obj)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            IEnumerable<MultiLanguageWord> w;
            if (obj.ContainsKey("LanguageFrom") && !string.IsNullOrEmpty(obj["LanguageFrom"]))
            {
                w = MultiLanguage.Find(s => (s.WordValue.ToLower() == obj["Value"].ToLower() && s.Language.ToLower() == obj["LanguageFrom"].ToLower())).ToListAsync().Result;
            }
            else
            {
                w = MultiLanguage.Find(s => s.WordValue.ToLower() == obj["Value"].ToLower()).ToListAsync().Result;
            }
            if (w == null || w.Count() == 0)
            {
                if (obj.ContainsKey("LanguageFrom") && !string.IsNullOrEmpty(obj["LanguageFrom"]))
                {
                    w = MultiLanguage.Find(s => (s.WordValue.ToLower().Contains(obj["Value"].ToLower()) && s.Language.ToLower() == obj["LanguageFrom"].ToLower())).ToListAsync().Result;
                }
                else
                {
                    w = MultiLanguage.Find(s => (s.WordValue.ToLower().Contains(obj["Value"].ToLower()))).ToListAsync().Result;
                }
                if (w == null || w.Count() == 0)
                {
                    return result;
                }
            }
            string assignID = w.FirstOrDefault<MultiLanguageWord>().AssignID;
            IEnumerable<MultiLanguageWord> data;
            if (obj.ContainsKey("LanguageTo") && !string.IsNullOrEmpty(obj["LanguageTo"]))
            {
                data = MultiLanguage.Find(s => (s.AssignID.ToLower() == assignID.ToLower() && s.Language.ToLower() == obj["LanguageTo"].ToLower())).ToListAsync().Result;
            }
            else
            {
                data = MultiLanguage.Find(s => s.AssignID.ToLower() == assignID.ToLower()).ToListAsync().Result;
            }
            result.Add("LanguageFrom", w.FirstOrDefault().Language);
            result.Add("Data", data);
            return result;
        }

        /// <summary>
        /// Tìm từ theo ảnh
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost("MultiLanguageImg")]
        public async Task<IEnumerable<Dictionary<string, object>>> multiLanguageImg([FromBody] Dictionary<string, string> obj)
        {
            string base64String = obj["Value"];
            var imageParts = base64String.Split(',').ToList<string>();
            byte[] imageBytes = Convert.FromBase64String(imageParts[1]);

            using var clientImg = new HttpClient();

            Mainrequests mainrequests = new Mainrequests()
            {
                requests = new List<requests>()
                    {
                        new requests()
                        {
                            image = new image()
                            {
                                content = imageParts[1]
                            },

                            features = new List<features>()
                            {
                                new features()
                                {
                                    type = "TEXT_DETECTION",
                                }

                            }

                        }

                    }

            };


            var uri = "https://vision.googleapis.com/v1/images:annotate?key=AIzaSyB_ZA1T4aWp2qGWym3BmFgUUn-_W5lSaYs";
            clientImg.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            clientImg.BaseAddress = new Uri(uri);
            //clientImg.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            HttpContent content = new StringContent(JsonConvert.SerializeObject(mainrequests),
                                                Encoding.UTF8,
                                                "application/json");//CONTENT-TYPE header
            var Res = await clientImg.PostAsync(uri, content);
            var contentx = await Res.Content.ReadAsStringAsync();
            var dataImg = JsonConvert.DeserializeObject<ResponVision>(contentx);
            string searchText = string.Empty;
            if (dataImg != null && dataImg.responses != null && dataImg.responses.Count > 0 && dataImg.responses[0].fullTextAnnotation != null && dataImg.responses[0].fullTextAnnotation.text != null)
            {
                searchText = dataImg.responses[0].fullTextAnnotation.text.Replace("\n", "");
            }
            Dictionary<string, object> result = new Dictionary<string, object>();
            if (string.IsNullOrEmpty(searchText))
            {
                return null;
            }
            IEnumerable<MultiLanguageWord> w;
            if (obj.ContainsKey("LanguageFrom") && !string.IsNullOrEmpty(obj["LanguageFrom"]))
            {
                w = MultiLanguage.Find(s => (s.WordValue.ToLower() == searchText.ToLower() && s.Language.ToLower() == obj["LanguageFrom"].ToLower())).ToListAsync().Result;
            }
            else
            {
                w = MultiLanguage.Find(s => s.WordValue.ToLower() == searchText.ToLower()).ToListAsync().Result;
            }
            if (w == null || w.Count() == 0)
            {
                if (obj.ContainsKey("IsFindSame") && !string.IsNullOrEmpty(obj["IsFindSame"]) && obj["IsFindSame"].ToLower() == "true")
                {
                    if (obj.ContainsKey("LanguageFrom") && !string.IsNullOrEmpty(obj["LanguageFrom"]))
                    {
                        w = MultiLanguage.Find(s => (s.WordValue.ToLower().Contains(searchText.ToLower()) && s.Language.ToLower() == obj["LanguageFrom"].ToLower())).ToListAsync().Result;
                    }
                    else
                    {
                        w = MultiLanguage.Find(s => (s.WordValue.ToLower().Contains(searchText.ToLower()))).ToListAsync().Result;
                    }
                }
                if (w == null || w.Count() == 0)
                {
                    return null;
                }
            }
            string assignID = w.FirstOrDefault<MultiLanguageWord>().AssignID;
            IEnumerable<MultiLanguageWord> data;
            if (obj.ContainsKey("LanguageTo") && !string.IsNullOrEmpty(obj["LanguageTo"]))
            {
                data = MultiLanguage.Find(s => (s.AssignID.ToLower() == assignID.ToLower() && s.Language.ToLower() == obj["LanguageTo"].ToLower())).ToListAsync().Result;
            }
            else
            {
                data = MultiLanguage.Find(s => s.AssignID.ToLower() == assignID.ToLower()).ToListAsync().Result;
            }

            List<Dictionary<string, object>> listResult = new List<Dictionary<string, object>>();

            result.Add("LanguageFrom", w.FirstOrDefault().Language);
            result.Add("Data", data);
            listResult.Add(result);
            IEnumerable<Dictionary<string, object>> d = listResult;
            return d;
        }
    }
}

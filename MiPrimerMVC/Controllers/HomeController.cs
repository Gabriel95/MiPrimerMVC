using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Web.WebPages;
using BotDetect.Web.UI.Mvc;
using CaptchaMvc.HtmlHelpers;
using Domain.Entities;
using Domain.Services;
using FluentNHibernate.Conventions;
using MiPrimerMVC.Models;
using RestSharp;
namespace MiPrimerMVC.Controllers
{
    public class HomeController : Controller
    {
        readonly IReadOnlyRepository _readOnlyRepository;
        readonly IWriteOnlyRepository _writeOnlyRepository;

        public HomeController(IReadOnlyRepository readOnlyRepository, IWriteOnlyRepository writeOnlyRepository)
        {

            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
        }

        static readonly string PasswordHash = "P@@Sw0rd";
        static readonly string SaltKey = "S@LT&KEY";
        static readonly string VIKey = "@1B2c3D4e5F6g7H8";

        public static string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }

        // GET: Home
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {

            if (model.Name.Length < 3 || model.Name.Length > 50)
            {
                ViewBag.Message = "Name is minum 3 characters and maximum 50 characters";

                return View();
            }


            if (model.Password.Length < 8 || model.Password.Length > 20)
            {
                ViewBag.Message = "Password is minum 8 characters and maximum 20 characters";

                return View();
            }

            Regex r = new Regex("^[a-zA-Z0-9]*$");
            if (!r.IsMatch(model.Password))
            {
                ViewBag.Message = "Password can only contain letters and/or digits";

                return View();
            }

            if (model.Password == model.RePassword)
            {
                var toAdd = new User();

                toAdd.Name = model.Name;
                toAdd.Email = model.Email;
                toAdd.Password = Encrypt(model.Password);

                var resp = SendSimpleMessage(model.Email, model.Name);
                var justRooIt = _writeOnlyRepository.Create(toAdd);
                ViewBag.Message = "Succesfull Registration!";
            }
            else
            {
                ViewBag.Message = "Passwords Do Not Match!";

                return View();
            }

            var clasi = new UserandItemsModel
            {
                cUser = null,

                Itemses = _readOnlyRepository.GetAll<Items>().ToList()
            };

            return View("MainPage",clasi);
        }

        public static IRestResponse SendSimpleMessage(string destination, string firstname)
        {
            var client = new RestClient
            {
                BaseUrl = "https://api.mailgun.net/v2",
                Authenticator = new HttpBasicAuthenticator("api",
                    "key-8tw489mxfegaqewx93in2xo449q5p3l0")
            };
            var request = new RestRequest();
            request.AddParameter("domain",
                                "app5dcaf6d377cc4ddcb696b827eabcb975.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "NameOfAplationHere@Something.com");
            String email = "<" + destination + ">";
            request.AddParameter("to", email);
            request.AddParameter("subject", "Register Process");
            String text = "From Support: Thank You For Registration in <Insert Name Here> Your Username: " + destination;
            request.AddParameter("text", text);
            request.Method = Method.POST;
            return client.Execute(request);
        }

        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(ToLogInModel model)
        {
            var searchedUser1 = _readOnlyRepository.GetAll<User>().ToList();
            var searchedUser = searchedUser1.Find(x => x.Email == model.Email);

            if (searchedUser != null && model.Password.Equals(Decrypt(searchedUser.Password)))
            {

                    searchedUser.Password = Decrypt(searchedUser.Password);
                    Session["User"] = searchedUser;
                
            }
            else
            {
                ViewBag.Message = "User or Password Dont Match";

                return View();
            }

            return View("UserProfile",searchedUser);
        }

        public ActionResult QandAs()
        {

            var listQA = new QuestionListModel
            {
                ListQa = _readOnlyRepository.GetAll<QandA>().ToList()
            };

            listQA.ListQa.Reverse();

            return View(listQA);
        }

        public ActionResult FQandAs()
        {
            var listQA = new QuestionListModel
            {

                ListQa = _readOnlyRepository.GetAll<QandA>().ToList(),
                NewQuestion = new QuestionModel()
            };

            listQA.ListQa = listQA.ListQa.FindAll(x => Convert.ToInt32(x.Frequency) >= 5);
            listQA.ListQa.Reverse();
            if (listQA.ListQa.Count == 0)
            {
                ViewBag.Message =
                    "En este momento no tenemos preguntas frecuentes en nuestra base de datos, " +
                    "pero puedes enviarnos una y con gusto trabajaremos para contestartela";
            }

            return View("QandAs", listQA);
        }


        [HttpPost]
        [AcceptVerbs("POST", "HEAD")]
        public ActionResult QandAs(QuestionListModel model)
        {
            
            if (model.NewQuestion.User.Length < 3 || model.NewQuestion.User.Length > 20)
            {
                ViewBag.Message = "Name is minum3 character and maximum 20 character";
                model.ListQa = _readOnlyRepository.GetAll<QandA>().ToList();

                return View(model);
            }

            if (WordCounting.CountWords1(model.NewQuestion.Question) < 3 || model.NewQuestion.Question.Length > 250)
            {
                ViewBag.Message = "Question Must contain at least 3 words and maximun 250 characters";
                model.ListQa = _readOnlyRepository.GetAll<QandA>().ToList();

                return View(model);
            }
            Regex r = new Regex("^[a-zA-Z0-9]*$");
            if (!r.IsMatch(model.NewQuestion.User))
            {
                ViewBag.Message = "Name can only contain letters and/or digits";
                model.ListQa = _readOnlyRepository.GetAll<QandA>().ToList();
                return View();
            }
            var list = _readOnlyRepository.GetAll<QandA>().ToList();
            QandA exists = list.Find(x => x.Question == model.NewQuestion.Question);
            if (exists != null)
            {
                exists.Frequency = Convert.ToString(Convert.ToInt32(exists.Frequency) + 1);
                var write1 = _writeOnlyRepository.Update(exists);

            }
            else
            {
                var nq = new QandA
                {
                    Question = model.NewQuestion.Question,
                    Email = model.NewQuestion.Email,
                    Date = DateTime.UtcNow,
                    Frequency = "1"
                };

                var write = _writeOnlyRepository.Create(nq);
            }
            model.ListQa = _readOnlyRepository.GetAll<QandA>().ToList();

            return View(model);
        }

        public static class WordCounting
        {
            /// <summary>
            /// Count words with Regex.
            /// </summary>
            public static int CountWords1(string s)
            {
                MatchCollection collection = Regex.Matches(s, @"[\S]+");
                return collection.Count;
            }

            /// <summary>
            /// Count word with loop and character tests.
            /// </summary>
            public static int CountWords2(string s)
            {
                int c = 0;
                for (int i = 1; i < s.Length; i++)
                {
                    if (char.IsWhiteSpace(s[i - 1]) == true)
                    {
                        if (char.IsLetterOrDigit(s[i]) == true ||
                            char.IsPunctuation(s[i]))
                        {
                            c++;
                        }
                    }
                }
                if (s.Length > 2)
                {
                    c++;
                }
                return c;
            }
        }

        public ActionResult Rules()
        {
            return View();
        }

        public ActionResult UserProfile()
        {
            var user = (User)Session["User"];
            return View(user);
        }

        public ActionResult UserItems()
        {
            User currentUser = (User)Session["User"];

            Session["User"] = currentUser;

            var li = _readOnlyRepository.GetAll<Items>().ToList();
            var cU = new UserandItemsModel
            {
                cUser = _readOnlyRepository.GetById<User>(currentUser.Id),

                Itemses = li.FindAll(x => x.UserId == currentUser.Id)
            };

            return View(cU);
        }

        public ActionResult AddItems()
        {
            if (Session["User"] == null)
            {
                var clasi = new UserandItemsModel
                {
                    cUser = null,

                    Mes = null,

                    Itemses = _readOnlyRepository.GetAll<Items>().ToList()
                };
                return View("MainPage",clasi);
            }
            return View();
        }

        public ActionResult MainPage()
        {
            var clasi = new UserandItemsModel
            {
                cUser = null,

                Itemses = _readOnlyRepository.GetAll<Items>().ToList()
            };
            clasi.Itemses.Reverse();
            return View(clasi);
        }

        [HttpPost]
        public ActionResult AddItems(Items model)
        {
            if (WordCounting.CountWords1(model.Title) < 1 || model.Title.Length > 100)
            {
                ViewBag.Message = "Title must contain 1 word and maximun 100 characters";
                return View();
            }

            if (!model.Price.IsInt())
            {
                ViewBag.Message = "Price must be in numbers";
                return View();
            }

            if (WordCounting.CountWords1(model.Description) < 3 || model.Description.Length > 255)
            {
                ViewBag.Message = "Description must contain at least 3 words and maximun 255 characters";
                return View(model);
            }

            User currentUser = (User)Session["User"];

            Session["User"] = currentUser;

            model.UserId = currentUser.Id;

            model.Date = DateTime.Now;

            model.VideoUrl = model.VideoUrl.Replace("watch?v=", "embed/");

            model.VideoUrl = model.VideoUrl.Replace("https://", "//");


            _writeOnlyRepository.Create(model);

            var cU = _readOnlyRepository.GetById<User>(currentUser.Id);


            return View("UserProfile", cU);
        }

        public ActionResult LogOut()
        {
            Session["User"] = null;

            var clasi = new UserandItemsModel
            {
                cUser = null,

                Itemses = _readOnlyRepository.GetAll<Items>().ToList()
            };

            return View("MainPage", clasi);
        }

        public ActionResult DetailedItem(long id)
        {
            var itemm = new UserandItemsModel
            {
              JustItem  = _readOnlyRepository.First<Items>(x => x.Id == id),

              cUser = null,

              Itemses = null,

              Mes = null
            };

            Session["ID"] = itemm;

            itemm.JustItem.Views++;

            _writeOnlyRepository.Update(itemm.JustItem);

            return View(itemm);
        }
        [HttpPost]
        public ActionResult DetailedItem(UserandItemsModel model)
        {
            model.cUser = _readOnlyRepository.GetById<User>(((UserandItemsModel)Session["ID"]).JustItem.UserId);
            model.JustItem = ((UserandItemsModel) Session["ID"]).JustItem;
            if (!this.IsCaptchaValid("Captcha is not valid"))
            {
                return View(model);
            }

            Regex r = new Regex("^[a-zA-Z]+$");
            if (!r.IsMatch(model.Mes.Name))
            {
                ViewBag.Message = "Name can only contain letters";
                return View(model);
            }
            if (model.Mes.Name.Length < 3 || model.Mes.Name.Length > 50)
            {
                ViewBag.Message = "Name can only be minimum 3 character and maximum 50";
                return View(model);
            }
            if (WordCounting.CountWords1(model.Mes.Message) < 3 || model.Mes.Message.Length > 250)
            {
                ViewBag.Message = "Message Must contain at least 3 words and maximun 250 characters";
                return View(model);
            }

            var m = new Messages
            {
                Froms = model.Mes.Froms,
                Message = model.Mes.Message,
                Tos = model.cUser.Id,
                Date = DateTime.Now
            };

            _writeOnlyRepository.Create(m);
            return View(model);
       
           
        }
        public ActionResult ContactInfo()
        {
            return View();
        }

        [HttpPost]
         public ActionResult ContactInfo(MessageModel model)
        {
            Regex r = new Regex("^[a-zA-Z]+$");
            if (!r.IsMatch(model.Name))
            {
                ViewBag.Message = "Name can only contain letters";
                return View();
            }
            if (model.Name.Length < 3 || model.Name.Length > 50)
            {
                ViewBag.Message = "Name can only be minimum 3 character and maximum 50";
                return View();
            }
            if (WordCounting.CountWords1(model.Message) < 3 || model.Message.Length > 250)
            {
                ViewBag.Message = "Message Must contain at least 3 words and maximun 250 characters";
                return View();
            }

            SendSimpleMessage(null, model.Name, model.Froms, model.Message);
            ViewBag.Message = "Message Sent!";

 
                return View();
        }

        public static IRestResponse SendSimpleMessage(string destination, string name, string from, string message)
        {
            var client = new RestClient
            {
                BaseUrl = "https://api.mailgun.net/v2",
                Authenticator = new HttpBasicAuthenticator("api",
                    "key-8tw489mxfegaqewx93in2xo449q5p3l0")
            };
            var request = new RestRequest();
            request.AddParameter("domain",
                                "app5dcaf6d377cc4ddcb696b827eabcb975.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", from);
            String email = "<jgpaz5@gmail.com>";
            request.AddParameter("to", email);
            request.AddParameter("subject", "Contact");
            String text = "From Support: Thank You For Registration in <Insert Name Here> Your Username: " + destination;
            request.AddParameter("text", message);
            request.Method = Method.POST;
            return client.Execute(request);
        }

        private static Dictionary<long, string> Numerals = new Dictionary<long, string>
        {
            {1,"Music"},
            {2,"House Hold"},
            {3,"Service"},
            {4,"Vehicles"},
            {5,"Others"}
 
        };
        public ActionResult ByCategory(long id)
        {
            var its = _readOnlyRepository.GetAll<Items>().ToList();
            var its2 = new UserandItemsModel
            {
                Itemses = its.FindAll(x => x.Category == Numerals[id])
            };

            its2.Itemses.Reverse();

            return View(its2);
        }

        public ActionResult AdvancedSearch()
        {
            var sU = new UserandItemsModel
            {
                Itemses = _readOnlyRepository.GetAll<Items>().ToList()
            };
            return View(sU);
        }

        [HttpPost]
        public ActionResult AdvancedSearch(UserandItemsModel model)
        {
            var filters = _readOnlyRepository.GetAll<Items>().ToList();

            filters = filters.FindAll(x => x.Category == model.Asearch.Category);

            if (model.Asearch.Title != null)
            {
                filters = filters.FindAll(x => x.Title.Contains(model.Asearch.Title));
            }

            if (model.Asearch.Description != null)
            {
                filters = filters.FindAll(x => x.Description.Contains(model.Asearch.Description));
            }

            model.Itemses = filters;

            return View(model);
        }

        public ActionResult MostViewed()
        {
            var l = _readOnlyRepository.GetAll<Items>().ToList();
            l = l.FindAll(x => x.Views >= 10);

            var top = new List<Items>();

            if (l.Count > 5)
            {
                while (top.Count < 5 )
                {
                    var biggest = l[0];
                    for (int i = 1; i < l.Count; i++)
                    {
                        if (biggest.Views < l[i].Views)
                        {
                            biggest = l[i];
                        }
                    }
                    top.Add(biggest);
                    l.Remove(biggest);
                }

                l = top;
            }
            var toModel = new UserandItemsModel
            {
                Itemses = l
            };
            return View(toModel);
        }

        public ActionResult MostRecent()
        {
            var l = _readOnlyRepository.GetAll<Items>().ToList();
            l.Reverse();

            if (l.Count > 5)
                 l.RemoveRange(5, l.Count - 5);

            var c = new UserandItemsModel
            {
                Itemses = l
            };
            
            return View(c);
        }

        public ActionResult Suggested()
        {
         var alles = _readOnlyRepository.GetAll<Items>().ToList();
         var toModel = new List<Items>();

         if(alles.Count>10)
         {
             var list = new List<List<Items>>();

             var music = alles.FindAll(x => x.Category == "Music");

            var hh = alles.FindAll(x => x.Category == "House Hold");

            var ser = alles.FindAll(x => x.Category == "Service");

            var vehicles = alles.FindAll(x => x.Category == "Vehicles");

            var other = alles.FindAll(x => x.Category == "Others");



            music.Reverse();
            hh.Reverse();
            ser.Reverse();
            vehicles.Reverse();
            other.Reverse();

            list.Add(music);
            list.Add(hh);
            list.Add(ser);
            list.Add(vehicles);
            list.Add(other);

            
            while (toModel.Count < 10)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (list[i].Count > i)
                    {
                        toModel.Add(list[0][0]);
                        list[0].RemoveAt(0);
                    }
                }
            }

             alles = toModel;
         }
            var uai = new UserandItemsModel
            {
                Itemses = alles
            };
            return View(uai);
        }

    }


}


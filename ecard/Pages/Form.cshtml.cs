using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ecard.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ecard.Pages
{
    public class FormModel : PageModel
    {

        // WOWOCO: 1
        [BindProperty]
        public Greetings _myGreetings { get; set; }

        // WOWOCO: 2
        private DbBridge _myDbBridge { get; set; }

        // WOWOCO: 3
        private IConfiguration _myConfiguration { get; set; }

        // WOWOCO: 4
        public FormModel(DbBridge DbBridge, IConfiguration Configuration)
        {
            _myDbBridge = DbBridge;
            _myConfiguration = Configuration;

        }

        public void OnGet() { }

        [HttpPost]
        public async Task<IActionResult> OnPost()
        {

            if (await isValid())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        _myGreetings.created = DateTime.Now.ToString();
                        _myGreetings.created_ip = this.HttpContext.Connection.RemoteIpAddress.ToString();


                        _myGreetings.friendname = _myGreetings.friendname.Replace("i", "3");
                        _myGreetings.friendname = _myGreetings.friendname.Replace("She said, \"Hello!\"", "");
                        _myGreetings.senderemail = _myGreetings.senderemail.ToLowerInvariant();
                        _myGreetings.friendemail = _myGreetings.friendemail.ToUpperInvariant();

                        // DB Related add record
                        _myDbBridge.Greetings.Add(_myGreetings);
                        _myDbBridge.SaveChanges();

                        //REDIRECT to the page with a new operator (name/value pair)
                        return RedirectToPage("Form", new { id = _myGreetings.ID });
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        return RedirectToPage("Form");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("_myGreetings.reCaptcha", "Please verify you're not a robot!");
            }

            return Page();

        }

        /**
         * reCAPTHCA SERVER SIDE VALIDATION 
         * 
         *      Create an HttpClient and store the the secret/response pair
         *      Await for the sever to return a json obect 
         * */
        private async Task<bool> isValid()
        {
            var response = this.HttpContext.Request.Form["g-recaptcha-response"];
            if (string.IsNullOrEmpty(response))
                return false;

            try
            {
                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>();
                    //values.Add("secret", "6LfVpjEUAAAAAK0FdygAgh0P1gZ8QU24ildwT86r");
                    values.Add("secret", _myConfiguration["ReCaptcha:PrivateKey"]);

                    values.Add("response", response);
                    //values.Add("remoteip", this.HttpContext.Connection.RemoteIpAddress.ToString()); 

                    var query = new FormUrlEncodedContent(values);

                    var post = client.PostAsync("https://www.google.com/recaptcha/api/siteverify", query);

                    var json = await post.Result.Content.ReadAsStringAsync();

                    if (json == null)
                        return false;

                    var results = JsonConvert.DeserializeObject<dynamic>(json);

                    return results.success;
                }

            }
            catch { }

            return false;
        }

    }
}

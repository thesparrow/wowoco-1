using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ecard.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace ecard.Pages
{
    public class IndexModel : PageModel
    {

        // WOWOCO: 1
        [BindProperty]
        public Greetings _myGreetings { get; set; }

        // WOWOCO: 2
        private DbBridge _myDbBridge { get; set; }

        // WOWOCO: 3
        private IConfiguration _myConfiguration { get; set; }

        // WOWOCO: 4
        public IndexModel(DbBridge DbBridge, IConfiguration Configuration)
        {
            _myDbBridge = DbBridge;
            _myConfiguration = Configuration;

        }

        public void OnGet(string ID)
        {

            if (!string.IsNullOrEmpty(ID))
            {
                //// Decrypt the ID 
                //var decrupted_ID = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(ID));
                //var read_ID = Int32.Parse(decrupted_ID);

                ////find the ID in the database 


            }

        }

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

                        //Get new ID FROM the DB
                        var new_ID = _myGreetings.ID.ToString();

                        //encrypt the ID
                        var encrypted_ID = Encoding.UTF8.GetBytes(new_ID);


                        //convert to Base64 
                        var base64_ID = Convert.ToBase64String(encrypted_ID); //



                        //REDIRECT to the page with a new operator (name/value pair)
                        return RedirectToPage("Preview", new { id = base64_ID });
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
}

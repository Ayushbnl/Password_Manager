using Microsoft.AspNetCore.Mvc;
using Password_Manager.Models;
using System.Runtime.Caching;

namespace Password_Manager.Controllers
{

    public class PasswordController : Controller
    {
        private MemoryCache cache = MemoryCache.Default;
        private const string CacheKey = "PasswordStore";

        //1. Add user encrypted password to the list
        [HttpPost]
        public IActionResult AddPassword([FromBody] PasswordEntry passwordEntry)
        {
            var passwordStore = GetPasswordStore();
            passwordEntry.Id = passwordStore.Any() ? passwordStore.Max(p => p.Id) + 1 : 1;
            passwordStore.Add(passwordEntry);
            cache.Set(CacheKey, passwordStore, DateTimeOffset.Now.AddHours(1));
            return RedirectToAction("GetPasswords");
        }

        //2.getting the list of passwords
        [HttpGet]
        public IActionResult GetPasswords()
        {
            var passwordStore = GetPasswordStore();
            return View(passwordStore);
        }
        //3. get single item from the password store
        [HttpGet]
        public IActionResult GetPassword(int id)
        {
            var passwordStore = GetPasswordStore();
            var passwordEntry = passwordStore.FirstOrDefault(p => p.Id == id);
            return View("PasswordDetails", new Tuple<PasswordEntry?, bool>(passwordEntry, false));
        }
        // Get decrypted password from the list
        [HttpGet]
        public IActionResult GetDecryptedPassword(int id)
        {
            var passwordStore = GetPasswordStore();
            var passwordEntry = passwordStore.FirstOrDefault(p => p.Id == id);
            return View("PasswordDetails", new Tuple<PasswordEntry?, bool>(passwordEntry, true));
        }

        [HttpPost]
        public IActionResult UpdatePassword([FromBody] PasswordEntry updatedPassword)
        {
            var passwordStore = GetPasswordStore();
            var existingPassword = passwordStore.FirstOrDefault(p => p.Id == updatedPassword.Id);
            if (existingPassword != null)
            {
                // Update existing password
                existingPassword.Category = updatedPassword.Category;
                existingPassword.App = updatedPassword.App;
                existingPassword.UserName = updatedPassword.UserName;
                existingPassword.EncryptedPassword = updatedPassword.EncryptedPassword;
                cache.Set(CacheKey, passwordStore, DateTimeOffset.Now.AddHours(1));
            }
            return RedirectToAction("GetPasswords");
        }

        [HttpPost]
        public IActionResult DeletePassword(int id)
        {
            var passwordStore = GetPasswordStore();
            var passwordEntry = passwordStore.FirstOrDefault(p => p.Id == id);
            if (passwordEntry != null)
            {
                passwordStore.Remove(passwordEntry);
                cache.Set(CacheKey, passwordStore, DateTimeOffset.Now.AddHours(1));
            }
            return RedirectToAction("GetPasswords");
        }

        private List<PasswordEntry> GetPasswordStore()
        {
            var passwordStore = cache.Get(CacheKey) as List<PasswordEntry>;
            if (passwordStore == null)
            {
                passwordStore = new List<PasswordEntry>()
                {
                    new PasswordEntry { Id = 1, Category = "work", App = "Outlook", UserName = "testuser@mytest.com", EncryptedPassword = "TXlQYXNzd29yZEAxMjM=" },
                    new PasswordEntry { Id = 2, Category = "personal", App = "Facebook", UserName = "user123", EncryptedPassword = "U29ycnksb3Uga25vdyE=" },
                    new PasswordEntry { Id = 3, Category = "finance", App = "Bank", UserName = "bankuser", EncryptedPassword = "QmFua3VzZXIxMjM=" },
                    new PasswordEntry { Id = 4, Category = "social", App = "Twitter", UserName = "@twitteruser", EncryptedPassword = "VHdpdHRlcjEyMw==" },
                    new PasswordEntry { Id = 5, Category = "entertainment", App = "Netflix", UserName = "netflixuser", EncryptedPassword = "TmV0aHlJbg==" },
                    new PasswordEntry { Id = 6, Category = "shopping", App = "Amazon", UserName = "amazonuser", EncryptedPassword = "QW1hem9uMTIz" }};
                cache.Set(CacheKey, passwordStore, DateTimeOffset.Now.AddHours(1));
            }
            return passwordStore;
        }
    }

}
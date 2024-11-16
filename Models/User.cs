using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ATMBank.Models{
    public class User{
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        [JsonIgnore]
        public List<Account> Accounts { get; set; }=new List<Account>();
    }
}
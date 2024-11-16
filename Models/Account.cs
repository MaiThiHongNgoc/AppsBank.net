using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ATMBank.Models
{
    public enum AccountType
    {
        Cheking = 0,
        Saving = 1

    }
    public class Account
    {
        [Key]
        public int AccountId { get; set; }
        public int UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; } = null; //? nullable
        [EnumDataType(typeof(AccountType))]
        public AccountType? Type { get; set; }
        public float Balance { get; set; } = 0;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public float InterestRate { get; set; }
        [JsonIgnore]
        public List<Transaction>? Transactions { get; set; } = new List<Transaction>();
    }
}
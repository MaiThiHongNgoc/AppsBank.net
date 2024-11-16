using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ATMBank.Models{
    public class Transaction{
        [Key]
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        [JsonIgnore]
        public Account? Account { get; set; }
        public float Amount { get; set; }
        public DateTime Timestemp { get; set; }=DateTime.Now;
        public Boolean? Isuccessful { get; set; }
        public string? Description { get; set; }
    }
}
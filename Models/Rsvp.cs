using System.ComponentModel.DataAnnotations;

namespace BeltExam.Models
{
    public class Rsvp
    {
        [Key]
        public int RsvpId { get; set; }

        // Foreign Key
        public int UserId { get; set; }
        public int FunEventId { get; set; }

        // Nav Props
        public User Guest { get; set; }
        public FunEvent Attending { get; set; }
    }
}
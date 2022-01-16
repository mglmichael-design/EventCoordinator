using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BeltExam.Validations;

namespace BeltExam.Models
{
    public class FunEvent
    {
        [Key]
        public int FunEventId { get; set; }


        [Required(ErrorMessage = "Title is required.")]
        [Display(Name = "Title: ")]
        public string EventTitle { get; set; }

        [Required(ErrorMessage = "Time is required.")]
        [Display(Name = "Time: ")]
        public int EventTime { get; set; }
        

        [Required(ErrorMessage = "Duration is required.")]
        [Display(Name = "Tim: ")]
        public int EventDuration { get; set; }

        [Required(ErrorMessage="Is it gonna take hours or days?")]
        public string EventDurationType {get;set;}

        [Future]
        [Required(ErrorMessage = "Date is required ")]
        [Display(Name = "Date: ")]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [Display(Name = "Description: ")]
        public string EventDescription { get; set; }


        

        // Foreign Key - One to Many
        public int UserId { get; set; }
        // One to Many - A Union can only have one planner.
        public User Planner { get; set; }

        // Many to many - A union can have many guests.

        public List<Rsvp> GuestList { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
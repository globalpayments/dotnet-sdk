using System;

namespace TestBoardingAPI.Models {
    public class CompletedApplication {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime SubmittedDate { get; set; }
    }
}
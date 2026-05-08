using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetAdopt.BLL.DTOs
{
    public class FeedbackDto
    {
        public int Id { get; set; }

        public string AdopterName { get; set; } = string.Empty;

        public int PetId { get; set; }

        public string Comment { get; set; } = string.Empty;

        public int Rating { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

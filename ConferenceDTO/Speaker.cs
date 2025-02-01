using System.ComponentModel.DataAnnotations;

namespace ConferenceDTO
{
    public class Speaker
    {
        public int Id { get; set; } 
        /// <summary>
        /// The name of the speaker
        /// </summary>

        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        /// <summary>
        /// Bigraphical information about our speaker
        /// </summary>

        [StringLength(4000)]
        public string Bio { get; set; }
        /// <summary> 
        /// Where you can learn about our speaker
        /// </summary>
        [StringLength(1000)]
        public virtual string WebSite { get; set; }

    }
}

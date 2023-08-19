using System;
using System.Collections.Generic;

namespace List.Models
{
    public partial class SdDatabase
    {
        public int SicilNo { get; set; }
        public string? Ad { get; set; }
        public string? Soyad { get; set; }
        public string? Bolum { get; set; }
        public string? DagitimId { get; set; }
        public string? EkleyenKisi { get; set; }
        public DateTime? EklendigiTarih { get; set; }
        public int? Flos { get; set; }
        public DateTime? VerilisTarih { get; set; }
        public DateTime? EklendigiTarihDate => EklendigiTarih?.Date;
        public DateTime? VerilisTarihDate => VerilisTarih?.Date;
    }
}

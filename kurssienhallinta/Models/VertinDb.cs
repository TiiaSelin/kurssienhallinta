namespace VertinDb.Models
{
    public class Kurssi
    {
        public string? kurssitunnus { get; set; }
        public string? kurssinimi { get; set; }
        public string? kurssikuvaus { get; set; }
        public DateTime? kurssialoituspaiva { get; set; }
        public DateTime? kurssilopetuspaiva { get; set; }
        public string? opettajatunnus { get; set; }
        public string? tilatunnus { get; set; }
    }

    public class Opettaja
    {
        public string? opettajatunnus { get; set; }
        public string? opettajaetunimi { get; set; }
        public string? opettajasukunimi { get; set; }
        public string? opettajaaine { get; set; }
    }

    public class Tila
    {
        public string? tilatunnus { get; set; }
        public string? tilanimi { get; set; }
        public string? tilakapasiteetti { get; set; }
    }

}
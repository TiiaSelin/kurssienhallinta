namespace kurssienhallinta.Models.ViewModels
{
    public class ModifyRoomViewModel
    {
        public Tila Tila { get; set; } = new();
        public List<Tila> Tilat { get; set; } = new();
    }

}


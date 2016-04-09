namespace Models.Entities
{
    public class Patient
    {
        public Patient()
        {
            geo = new Location();
            vitals = new Vitals();
        }

        public long patientID { get; set; }
        public Location geo { get; set; }
        public Vitals vitals { get; set; }
    }
}

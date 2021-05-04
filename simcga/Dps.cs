namespace simcga
{
    public class Dps
    {
        public double Damage { get; internal set; }

        public override string ToString()
        {
            return Damage.ToString();
        }
    }
}

namespace OTUS.HM4
{
    public class F
    {
        public int i1 { get; set; }
        public int i2 { get; set; }
        public int i3 { get; set; }
        public int i4 { get; set; }
        public int i5 { get; set; }

        public static F Get() => new() { i1 = 1, i2 = 2, i3 = 3, i4 = 4, i5 = 5 };
    }
}

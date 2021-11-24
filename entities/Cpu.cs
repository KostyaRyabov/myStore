namespace myStore.entities
{
    class Cpu
    {
        public int cpu_id { get; set; }
        public string processor { get; set; }
        public string producer { get; set; }
        public short? frequency_mhz { get; set; }
        public short? max_frequency_mhz { get; set; }
        public short? num_of_cores { get; set; }
    }
}

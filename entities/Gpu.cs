using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myStore.entities
{
    class Gpu
    {
        public int gpu_id { get; set; }
        public string video_adapter { get; set; }
        public string video_card { get; set; }
        public string video_memory_type { get; set; }
        public short? video_memory_size { get; set; }
        public short? num_of_cores { get; set; }
    }
}

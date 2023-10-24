using Libs.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.DTOs
{
    public class ResListRoom
    {
        public int SoLuong { get; set; }
        public Guid Id { get; set; }
        public string RoomName { get; set; }
    }
}

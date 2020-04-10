using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StomKarta
{
    public class PacientInfo
    {
        public short Id { get; set; }
        public string Fam { get; set; }
        public string Im { get; set; }
        public string Otc { get; set; }
        public byte SexId { get; set; }
        public DateTime? DR { get; set; }
        public string Mail { get; set; }
        public string Adress { get; set; }
        public string Comment { get; set; }
        public DateTime RowUpdate { get; set; }
        public byte UserId { get; set; }
        public string NickName { get; set; }
        public string FromComment { get; set; }

        public List<string> Telefons = new List<string>(4); // with estimated capacity
        public PacientInfo(byte userId)
        {
            UserId = userId;
        }
    }
}

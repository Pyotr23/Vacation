using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontendWPF.Models
{
    public class Cell
    {
        public string Value { get; }
        public string Color { get; }

        public Cell(string value, string color)
        {
            Value = value;
            Color = color;
        }
    }
}

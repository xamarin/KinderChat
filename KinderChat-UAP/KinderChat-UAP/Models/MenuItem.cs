using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinderChat_UAP.Common;

namespace KinderChat_UAP.Models
{
    public class MenuItem
    {
        public string Icon { get; set; }

        public string Name { get; set; }

        public AlwaysExecutableCommand Command { get; set; }
    }
}

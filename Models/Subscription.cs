using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sounds_New.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public User User { get; set; }
        public User Subscriber { get; set; }
    }
}
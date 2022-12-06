using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


    public class InputResult
    {
        public int XInput = 0;
        public int YInput = 0;

        public bool HasValue => XInput != 0 || YInput != 0;
    }


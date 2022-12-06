using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

    

    public class KeyboardinputManager : IInputManager
    {
        private int _lastXImput;
        private int _lastYImput;
        
        public InputResult GetInput()
        {
            InputResult result = new InputResult();

            var xInput = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
            var yInput = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

        if (_lastXImput == 0 && _lastYImput == 0)
        {
            result.XInput = xInput;
            result.YInput = yInput;
        }

        _lastXImput = xInput;
        _lastYImput = yInput;

        return result;
        }
    }


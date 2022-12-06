using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



    public class MultipleInputManager : IInputManager   
    {
    private IInputManager[] _managers;

    public MultipleInputManager(params IInputManager[] managers)
    {
        _managers = managers;
    }
    public InputResult GetInput()
    {
        var inputResults = _managers.Select(manager => manager.GetInput());
        InputResult result = inputResults.FirstOrDefault(Input => Input.HasValue);
        return result ?? new InputResult();
    }
    }


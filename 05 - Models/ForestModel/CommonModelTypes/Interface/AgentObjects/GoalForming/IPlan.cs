﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CommonModelTypes.Interface.AgentObjects.GoalForming
{
    public  interface IPlan
    {
         ITickAction GetNextAction() ;


    }
}
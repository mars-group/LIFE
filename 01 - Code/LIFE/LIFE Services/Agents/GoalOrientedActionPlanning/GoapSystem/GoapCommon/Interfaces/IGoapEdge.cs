﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using GoapCommon.Abstract;

namespace GoapCommon.Interfaces
{
    public interface IGoapEdge  {

        IGoapNode GetSource();

        IGoapNode GetTarget();

        AbstractGoapAction GetAction();

        int GetCost();

    }
}

﻿using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RG_Tools.Helpers
{
    public class ParameterWrapper
    {
        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private Parameter _param;

        public Parameter Param
        {
            get { return _param; }
            set { _param = value; }
        }

        private bool _isinstance;
        public bool IsInstance
        {
            get { return _isinstance; }
            set { _isinstance = value; }
        }

    }
}

﻿using System;

namespace DakotaIntegratedSolutions
{
    public interface IGLNLocation
    {
        string Region { get; set; }
        string Site { get; set; }
        string Building { get; set; }
        string Floor { get; set; }
        string Room { get; set; }
        string Code { get; set; }
        string GLN { get; set; }
        DateTime Date { get; set; }
        string ToString();
        string Value();
        bool Printed { get; set; }
        bool Selected { get; set; }
        bool ToPrint
        {
            get;
            set;
        }
    }
}
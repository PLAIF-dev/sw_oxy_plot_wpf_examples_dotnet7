﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SineEase.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System;

namespace AnimationsDemo
{

    public class SineEase : IEasingFunction
    {
        public double Ease(double value)
        {
            return 1.0 - Math.Sin(Math.PI / 2.0 * (1.0 - value));
        }
    }
}
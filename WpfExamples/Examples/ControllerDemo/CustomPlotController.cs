﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomPlotController.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Represents the plot controller for the main window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using OxyPlot;

namespace ControllerDemo
{

    public class CustomPlotController : PlotController
    {
        public CustomPlotController()
        {
            this.UnbindAll();
            this.BindKeyDown(OxyKey.Left, PlotCommands.PanRight);
            this.BindKeyDown(OxyKey.Right, PlotCommands.PanLeft);
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAnimationViewModel.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Threading.Tasks;

namespace AnimationsDemo
{

    public interface IAnimationViewModel
    {
        bool SupportsEasingFunction { get; }

        Task AnimateAsync();
        Task AnimateAsync(AnimationSettings animationSettings);
    }
}
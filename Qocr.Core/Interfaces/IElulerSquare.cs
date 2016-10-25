﻿namespace Qocr.Core.Interfaces
{
    /// <summary>
    /// Значение Эйлерового квадрата.
    /// </summary>
    internal interface IElulerSquare
    {
        /// <summary>
        /// Признак того, что в данном участке изображения находится текущий Эйлеровый фрагмент.
        /// </summary>
        /// <param name="topX">Х координата угла квадрата.</param>
        /// <param name="topY">Y координата угла квадрата.</param>
        /// <param name="monomap"></param>
        /// <returns></returns>
        bool IsSquareDetected(int topX, int topY, IMonomap monomap);
    }
}
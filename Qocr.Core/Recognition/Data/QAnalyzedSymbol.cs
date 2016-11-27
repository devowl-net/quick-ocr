﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

using Qocr.Core.Interfaces;

namespace Qocr.Core.Recognition.Data
{
    /// <summary>
    /// Вся информация о символе на изображении.
    /// </summary>
    [DebuggerDisplay("{Char} - {StartPoint}")]
    public class QAnalyzedSymbol : QSymbol
    {
        /// <summary>
        /// Символ, который будет выведен по-умолчанию.
        /// </summary>
        public const char Default = '?';

        /// <summary>
        /// Создание экземпляра класса <see cref="QAnalyzedSymbol"/>.
        /// </summary>
        public QAnalyzedSymbol(QSymbol sourceSymbol, IEnumerable<QChar> recognitionChars) 
            : base(sourceSymbol.Monomap, sourceSymbol.StartPoint)
        {
            recognitionChars = recognitionChars ?? Enumerable.Empty<QChar>();
            Chars = new ReadOnlyCollection<QChar>(recognitionChars.ToList());
        }

        /// <summary>
        /// Результат распознания символа.
        /// </summary>
        public ReadOnlyCollection<QChar> Chars { get; }

        /// <summary>
        /// Символ.
        /// </summary>
        public char Char
        {
            get
            {
                var firstCharData = Chars.FirstOrDefault(chr => chr.State == QState.Ok) ??
                                    Chars.OrderBy(chr => chr.Probability).FirstOrDefault();

                return firstCharData?.Char ?? Default;
            }
        }
    }
}
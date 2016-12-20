﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Qocr.Core.Approximation;
using Qocr.Core.Data.Serialization;
using Qocr.Core.Interfaces;
using Qocr.Core.Properties;
using Qocr.Core.Recognition.Data;
using Qocr.Core.Recognition.Logic;

namespace Qocr.Core.Recognition
{
    /// <summary>
    /// Распознаватель текста.
    /// </summary>
    public class TextRecognizer
    {
        private readonly IApproximator _approximator;

        private readonly IAnalyzer _analyzer;

        private readonly IScanner _scanner;

        /// <summary>
        /// Создание экземпляра класса <see cref="TextRecognizer"/>.
        /// </summary>
        public TextRecognizer(EulerContainer container) 
            : this(new LuminosityApproximator(), container, null, null)
        {
        }

        /// <summary>
        /// Создание экземпляра класса <see cref="TextRecognizer"/>.
        /// </summary>
        public TextRecognizer(EulerContainer container, IApproximator approximator) 
            : this(approximator, container, null, null)
        {
        }

        /// <summary>
        /// Создание экземпляра класса <see cref="TextRecognizer"/>.
        /// </summary>
        public TextRecognizer(
            IApproximator approximator,
            EulerContainer container,
            IAnalyzer analyzer,
            IScanner scanner)
        {
            if (approximator == null)
            {
                throw new ArgumentNullException(nameof(approximator));
            }

            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));

                if (!container.Languages.Any())
                {
                    
                }
            }

            _approximator = approximator;
            _analyzer = analyzer ?? new DefaultAnalyzer(container);
            _scanner = scanner ?? new DefaultScanner();
        }

        /// <summary>
        /// Распознать.
        /// </summary>
        /// <param name="monomap">Ссылка на исходный <see cref="Bitmap"/>.</param>
        /// <returns></returns>
        public QReport Recognize(Bitmap monomap)
        {
            var appBitmap = _approximator.Approximate(monomap);
            return Recognize(appBitmap);
        }

        /// <summary>
        /// Распознать <see cref="IMonomap"/>.
        /// </summary>
        /// <param name="monomap">Ссылка на <see cref="IMonomap"/>.</param>
        /// <returns></returns>
        public QReport Recognize(IMonomap monomap)
        {
            // TODO Там надо анализировать как то раздробленные картинки
            // Получаем все фрагменты изображения
            IList<QSymbol> unknownFragments = _scanner.GetFragments(monomap);
            var recognizedSymbols = new ConcurrentBag<QAnalyzedSymbol>();

            /*
             * Описываю алгоритм:
             * 1. Распознаём все фрагмент которые 100% есть в базе
             * 2. Идём по не распознанным фрагментам и если справа или слева есть распознанный, то берём высоту распознанного и захватываем вверх
             * и вниз в 3 пикселя, если там есть фрагменты то плюсуем к фрагменту
             */

            // Пункт 1.
            Parallel.ForEach(
                unknownFragments.ToArray(),
                unknownFragment =>
                {
                    var existingSymbol =
                        recognizedSymbols.FirstOrDefault(
                            symbol => symbol.Euler.GetHashCode() == unknownFragment.Euler.GetHashCode());

                    if (existingSymbol == null)
                    {
                        QAnalyzedSymbol analyzedSymbol;
                        if (_analyzer.TryFindSymbol(unknownFragment, out analyzedSymbol))
                        {
                            recognizedSymbols.Add(analyzedSymbol);
                            unknownFragments.Remove(unknownFragment);
                        }
                    }
                    else
                    {
                        var newSymbol = new QAnalyzedSymbol(unknownFragment, existingSymbol.Chars);
                        recognizedSymbols.Add(newSymbol);
                    }
                });


            // Пункт 2.
            foreach (var unknownFragment in unknownFragments.Where(fragment => fragment != null).ToArray())
            {
                if (recognizedSymbols.Contains(unknownFragment))
                {
                    continue;
                }

                QAnalyzedSymbol analyzedSymbol = _analyzer.AnalyzeFragment(
                    unknownFragment,
                    unknownFragments,
                    recognizedSymbols);

                recognizedSymbols.Add(analyzedSymbol);
                unknownFragments.Remove(unknownFragment);
            }

            //// Пункт 2.
            //Parallel.ForEach(
            //    unknownFragments.ToArray(),
            //    unknownFragment =>
            //    {
            //        if (recognizedSymbols.Contains(unknownFragment))
            //        {
            //            // TODO пока по нубски
            //            return;
            //        }

            //        QAnalyzedSymbol analyzedSymbol = _analyzer.AnalyzeFragment(
            //            unknownFragment,
            //            unknownFragments,
            //            recognizedSymbols);

            //        recognizedSymbols.Add(analyzedSymbol);
            //        unknownFragments.Remove(unknownFragment);
            //    });

            return new QReport(recognizedSymbols.ToArray());
        }
    }
}